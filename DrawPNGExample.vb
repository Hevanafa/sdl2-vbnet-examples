Imports System.Runtime.InteropServices
Imports SDL2.SDL
Imports SDL2.SDL_image

Public Class DrawPNGExample
    ' Refs:
    ' IMG_Load
    ' https://discourse.libsdl.org/t/how-to-rendercopy-a-surface-into-the-window/26943

    Dim running As Boolean

    Public Sub New()
        running = True

        Setup()

        Do
            PollEvents()
            Render()

            t += 1
        Loop While running

        CleanUp()
    End Sub


    Dim window_ptr As IntPtr  ' SDL_Window
    Dim renderer_ptr As IntPtr  ' SDL_Renderer

    Dim img_ptr As IntPtr  ' SDL_Texture
    Dim img_w%, img_h%

    Dim t%  ' frames


    Sub Setup()
        SDL_Init(SDL_INIT_VIDEO)

        window_ptr = SDL_CreateWindow(
            "Hello SDL",
            SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED,
            320, 240,
            SDL_WindowFlags.SDL_WINDOW_SHOWN)

        renderer_ptr = SDL_CreateRenderer(
            window_ptr, -1,
            SDL_RendererFlags.SDL_RENDERER_ACCELERATED Or SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC)


        ' load PNG file
        Dim temp = IMG_Load("pomni.png")
        Dim temp_png = Marshal.PtrToStructure(Of SDL_Surface)(temp)
        img_w = temp_png.w
        img_h = temp_png.h
        img_ptr = SDL_CreateTextureFromSurface(renderer_ptr, temp)

        SDL_FreeSurface(temp)
        Debug.Print(SDL_GetError)
    End Sub


    Sub CleanUp()
        SDL_DestroyTexture(img_ptr)

        SDL_DestroyRenderer(renderer_ptr)
        SDL_DestroyWindow(window_ptr)
        SDL_Quit()
    End Sub


    Sub SetColour(rgb%)
        Dim r = CByte(rgb \ &H10000)
        Dim g = CByte(rgb \ &H100 Mod &H100)
        Dim b = CByte(rgb Mod &H100)

        SDL_SetRenderDrawColor(renderer_ptr, r, g, b, 255)
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

    Function Deg2Rad#(deg#)
        Deg2Rad = deg / 180 * Math.PI
    End Function

    Sub Cls()
        SDL_RenderClear(renderer_ptr)
    End Sub

    Sub Flush()
        SDL_RenderPresent(renderer_ptr)
    End Sub


    Sub Render()
        SetColour(&H6495ED)
        Cls

        Dim left% = (320 - img_w) / 2 + img_w / 8 * Math.Sin(Deg2Rad(t))

        Dim dest_rect As New SDL_Rect With {
            .x = left,
            .y = 0,
            .w = img_w,
            .h = img_h
        }

        SDL_RenderCopy(renderer_ptr, img_ptr, IntPtr.Zero, dest_rect)

        Flush()
    End Sub
End Class
