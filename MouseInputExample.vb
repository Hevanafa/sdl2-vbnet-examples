Imports SDL2.SDL
Imports SDL2.SDL_image
Imports SDL2.SDL_ttf
Imports System.Runtime.InteropServices

' Refs:
' SDL Mouse Click (Stackoverflow)
' https://stackoverflow.com/questions/35165716

' SDL_MouseButtonEvent struct
' https://wiki.libsdl.org/SDL2/SDL_MouseButtonEvent

' Clownfish image by LLENJIN:
' https://llen.itch.io/fish-sprite

Public Class MouseInputExample
    Dim running As Boolean

    Public Sub New()
        running = True

        Setup()

        Do
            frame += 1

            PollEvents()
            Render()
        Loop While running

        CleanUp()
    End Sub


    Dim window_ptr As IntPtr
    Dim window_w = 480, window_h = 320
    Dim renderer_ptr As IntPtr

    Sub SetColour(rgb%)
        Dim r = CByte(rgb \ &H10000)
        Dim g = CByte(rgb \ &H100 Mod &H100)
        Dim b = CByte(rgb Mod &H100)

        SDL_SetRenderDrawColor(renderer_ptr, r, g, b, 255)
    End Sub

    ''' <summary>
    ''' TTF_Font
    ''' </summary>
    Dim times As IntPtr

    ''' <summary>
    ''' SDL_Texture
    ''' </summary>
    Dim img_ptr As IntPtr
    Dim img_w%, img_h%

    Dim fish As New List(Of SDL_Point)


    Sub Setup()
        SDL_Init(SDL_INIT_VIDEO)

        window_ptr = SDL_CreateWindow(
            "Mouse Input Example",
            SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED,
            window_w, window_h,
            SDL_WindowFlags.SDL_WINDOW_SHOWN)

        renderer_ptr = SDL_CreateRenderer(
            window_ptr, -1,
            SDL_RendererFlags.SDL_RENDERER_ACCELERATED Or SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC)


        InitFont()
        InitFish
    End Sub


    Function InitFont()
        Dim font_dir$ = Environment.GetFolderPath(Environment.SpecialFolder.Fonts)

        TTF_Init()
        times = TTF_OpenFont(font_dir + "\TIMES.TTF", 20)
        Dim error$ = TTF_GetError()

        If Not String.IsNullOrWhiteSpace(error$) Then
            Debug.Print("Error when loading TTF: {0}", error$)
            Return False
        End If

        Return True
    End Function

    Sub InitFish()
        Dim temp = IMG_Load("clownfish.png")
        Dim temp_png = Marshal.PtrToStructure(Of SDL_Surface)(temp)
        img_w = temp_png.w
        img_h = temp_png.h
        img_ptr = SDL_CreateTextureFromSurface(renderer_ptr, temp)

        SDL_FreeSurface(temp)
        Debug.Print(SDL_GetError)
    End Sub


    Sub PollEvents()
        Dim e As SDL_Event

        Do
            Select Case e.type
                Case SDL_EventType.SDL_QUIT
                    running = False

                ' Spawn fish by left clicking
                Case SDL_EventType.SDL_MOUSEBUTTONDOWN
                    If e.button.button = SDL_BUTTON_LEFT Then
                        fish.Add(New SDL_Point With {
                                 .x = e.button.x,
                                 .y = e.button.y})
                    End If
            End Select
        Loop While SDL_PollEvent(e) = 1
    End Sub


    Sub DrawHint()
        Dim white As New SDL_Color With {
            .r = 255,
            .g = 255,
            .b = 255,
            .a = 0
        }

        Dim surface_ptr = TTF_RenderText_Solid(times, "Click to spawn a fish", white)
        Dim surface = Marshal.PtrToStructure(Of SDL_Surface)(surface_ptr)

        Dim txt_width = surface.w
        Dim txt_height = surface.h

        Dim dst_rect As New SDL_Rect With {
            .x = 0,
            .y = window_h - txt_height,
            .w = txt_width,
            .h = txt_height
        }

        Dim texture = SDL_CreateTextureFromSurface(renderer_ptr, surface_ptr)

        SDL_RenderCopy(renderer_ptr, texture, IntPtr.Zero, dst_rect)

        SDL_FreeSurface(surface_ptr)
        SDL_DestroyTexture(texture)
    End Sub


    Dim frame%
    Function Deg2Rad#(deg#)
        Deg2Rad = deg / 180 * Math.PI
    End Function

    Sub DrawFish(pos As SDL_Point)
        Dim dest_rect As New SDL_Rect With {
            .x = pos.x - img_w \ 2,
            .y = pos.y - img_h \ 2,
            .w = img_w,
            .h = img_h
        }

        SDL_RenderCopyEx(
            renderer_ptr, img_ptr,
            IntPtr.Zero, dest_rect,
            frame,  ' in degrees
            IntPtr.Zero, SDL_RendererFlip.SDL_FLIP_NONE)  ' centre
    End Sub


    Sub Render()
        SetColour(&H6495ED)
        SDL_RenderClear(renderer_ptr)


        DrawHint()

        For Each f In fish
            DrawFish(f)
        Next

        SDL_RenderPresent(renderer_ptr)
    End Sub


    Sub CleanUp()
        TTF_CloseFont(times)
        times = IntPtr.Zero

        SDL_DestroyTexture(img_ptr)
        img_ptr = IntPtr.Zero

        SDL_DestroyRenderer(renderer_ptr)
        SDL_DestroyWindow(window_ptr)
        renderer_ptr = IntPtr.Zero
        window_ptr = IntPtr.Zero

        ' Quit SDL subsystems
        TTF_Quit()
        SDL_Quit()
    End Sub
End Class
