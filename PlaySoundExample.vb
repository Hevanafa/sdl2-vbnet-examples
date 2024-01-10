Imports SDL2.SDL
Imports SDL2.SDL_ttf
Imports SDL2.SDL_mixer
Imports System.Runtime.InteropServices

' Refs:
' Sound Effects and Music
' https://lazyfoo.net/tutorials/SDL/21_sound_effects_and_music/index.php

' SDL_mixer API documentation
' https://wiki.libsdl.org/SDL2_mixer/CategoryAPI

Public Class PlaySoundExample
    Dim running As Boolean

    Public Sub New()
        running = True

        If Not Setup() Then Exit Sub

        Do
            PollEvents()
            Render()
        Loop While running

        CleanUp()
    End Sub


    Dim window_ptr As IntPtr
    Dim renderer_ptr As IntPtr

    ''' <summary>
    ''' TTF_Font
    ''' </summary>
    Dim consolas As IntPtr

    Sub SetColour(rgb%)
        Dim r = CByte(rgb \ &H10000)
        Dim g = CByte(rgb \ &H100 Mod &H100)
        Dim b = CByte(rgb Mod &H100)

        SDL_SetRenderDrawColor(renderer_ptr, r, g, b, 255)
    End Sub


    Function Setup() As Boolean
        If SDL_Init(SDL_INIT_VIDEO Or SDL_INIT_AUDIO) < 0 Then
            Console.WriteLine(
                "SDL couldn't start!" + vbCrLf +
                "Reason: " + SDL_GetError())

            Return False
        End If

        window_ptr = SDL_CreateWindow(
            "Hello SDL",
            SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED,
            320, 240,
            SDL_WindowFlags.SDL_WINDOW_SHOWN)

        renderer_ptr = SDL_CreateRenderer(
            window_ptr, -1,
            SDL_RendererFlags.SDL_RENDERER_ACCELERATED Or SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC)


        InitFont()
        InitAudio()

        Return True
    End Function


    Sub InitFont()
        Dim font_dir$ = Environment.GetFolderPath(Environment.SpecialFolder.Fonts)

        TTF_Init()
        consolas = TTF_OpenFont(font_dir + "\consola.ttf", 16)
        Dim error$ = TTF_GetError()

        If Not String.IsNullOrWhiteSpace(error$) Then
            Debug.Print("Error when loading TTF: {0}", error$)
        End If
    End Sub

    ''' <summary>
    ''' Mix_Chunk
    ''' </summary>
    Dim snare_sfx_ptr As IntPtr

    Function InitAudio() As Boolean
        If Mix_OpenAudio(44100, MIX_DEFAULT_FORMAT, 2, 2048) < 0 Then
            Console.WriteLine(
                "Couldn't open audio mixer." + vbCrLf +
                "Reason: " + Mix_GetError())

            Return False
        End If

        snare_sfx_ptr = Mix_LoadWAV("snare.wav")
        If snare_sfx_ptr = IntPtr.Zero Then
            Console.WriteLine("Failed to load snare SFX.  Reason: " + Mix_GetError())
            Return False
        End If

        Return True
    End Function


    Sub PollEvents()
        Dim e As SDL_Event

        Do
            Select Case e.type
                Case SDL_EventType.SDL_QUIT
                    running = False

                Case SDL_EventType.SDL_KEYDOWN
                    If e.key.keysym.sym = SDL_Keycode.SDLK_SPACE Then _
                        PlaySound()

            End Select
        Loop While SDL_PollEvent(e) = 1
    End Sub


    Sub PlaySound()
        Mix_PlayChannel(-1, snare_sfx_ptr, 0)
    End Sub


    Sub Render()
        SetColour(&H6495ED)
        SDL_RenderClear(renderer_ptr)

        Dim white As New SDL_Color With {
            .r = 255,
            .g = 255,
            .b = 255,
            .a = 0
        }

        Dim surface_ptr = TTF_RenderText_Solid(consolas, "Press Spacebar to play sound", white)
        Dim surface = Marshal.PtrToStructure(Of SDL_Surface)(surface_ptr)

        Dim txt_width = surface.w
        Dim txt_height = surface.h

        Dim dst_rect As New SDL_Rect With {
            .x = 0,
            .y = 0,
            .w = txt_width,
            .h = txt_height
        }

        Dim texture = SDL_CreateTextureFromSurface(renderer_ptr, surface_ptr)

        SDL_RenderCopy(renderer_ptr, texture, IntPtr.Zero, dst_rect)

        SDL_FreeSurface(surface_ptr)
        SDL_DestroyTexture(texture)

        SDL_RenderPresent(renderer_ptr)
    End Sub


    Sub CleanUp()
        TTF_CloseFont(consolas)
        consolas = IntPtr.Zero

        Mix_FreeChunk(snare_sfx_ptr)
        snare_sfx_ptr = IntPtr.Zero

        SDL_DestroyRenderer(renderer_ptr)
        SDL_DestroyWindow(window_ptr)
        renderer_ptr = IntPtr.Zero
        window_ptr = IntPtr.Zero

        ' Quit SDL subsystems
        TTF_Quit()
        Mix_Quit()
        SDL_Quit()
    End Sub
End Class
