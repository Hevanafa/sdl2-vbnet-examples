Imports SDL2.SDL
Imports SDL2.SDL_image
Imports SDL2.SDL_ttf
Imports System.Runtime.InteropServices

' Refs:
' https://wiki.libsdl.org/SDL2/SDL_RenderCopyEx

' Pomni (AI-generated):
' https://civitai.com/models/169335/pomni-the-amazing-digital-circus

Public Class DrawPNGRotated
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


    Sub Setup()
        SDL_Init(SDL_INIT_VIDEO)

        window_ptr = SDL_CreateWindow(
            "Rotated PNG Texture Example",
            SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED,
            320, 240,
            SDL_WindowFlags.SDL_WINDOW_SHOWN)

        renderer_ptr = SDL_CreateRenderer(
            window_ptr, -1,
            SDL_RendererFlags.SDL_RENDERER_ACCELERATED Or SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC)


        ' load TTF file
        TTF_Init()
        times = TTF_OpenFont("TIMES.TTF", 20)
        Dim error$ = TTF_GetError()

        If Not String.IsNullOrWhiteSpace(error$) Then
            Debug.Print("Error when loading TTF: {0}", error$)
        End If


        ' load PNG file
        Dim temp = IMG_Load("pomni_2.png")
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
            End Select
        Loop While SDL_PollEvent(e) = 1
    End Sub


    Sub DrawRotatedText()
        Dim white As New SDL_Color With {
            .r = 255,
            .g = 255,
            .b = 255,
            .a = 0
        }

        Dim surface_ptr = TTF_RenderText_Solid(times, "Hello SDL!", white)
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

        'SDL_RenderCopy(renderer, texture, IntPtr.Zero, dst_rect)
        SDL_RenderCopyEx(renderer_ptr, texture, IntPtr.Zero, dst_rect, 15, IntPtr.Zero, SDL_RendererFlip.SDL_FLIP_NONE)

        SDL_FreeSurface(surface_ptr)
        SDL_DestroyTexture(texture)
    End Sub


    Dim frame%
    Function Deg2Rad#(deg#)
        Deg2Rad = deg / 180 * Math.PI
    End Function

    Sub DrawRotatedTexture()
        Dim dest_rect As New SDL_Rect With {
            .x = (320 - img_w) \ 2,
            .y = 0,
            .w = img_w,
            .h = img_h
        }

        'Dim centre As New SDL_Point With {
        '    .x = 160,
        '    .y = 120
        '}

        SDL_RenderCopyEx(
            renderer_ptr, img_ptr,
            IntPtr.Zero, dest_rect,
            frame,  ' in degrees
            IntPtr.Zero, SDL_RendererFlip.SDL_FLIP_NONE)  ' centre
    End Sub


    Sub Render()
        SetColour(&H6495ED)
        SDL_RenderClear(renderer_ptr)


        DrawRotatedText()
        DrawRotatedTexture()

        SDL_RenderPresent(renderer_ptr)
    End Sub


    Sub CleanUp()
        TTF_CloseFont(times)
        TTF_Quit()

        SDL_DestroyTexture(img_ptr)

        SDL_DestroyRenderer(renderer_ptr)
        SDL_DestroyWindow(window_ptr)
        SDL_Quit()
    End Sub
End Class
