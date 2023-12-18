Imports SDL2.SDL
Imports SDL2.SDL_ttf
Imports System.Runtime.InteropServices

Public Class DrawBMPExample
    ' Refs:

    ' Marshal.PtrToStructure
    ' https://stackoverflow.com/questions/27741351/
    ' https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.ptrtostructure?view=net-8.0

    Dim running As Boolean

    Public Sub New()
        running = True

        Setup()

        Do
            PollEvents()
            Render()
        Loop While running

        CleanUp()
    End Sub


    Dim window_ptr As IntPtr  ' SDL_Window
    ' in this example, we're blitting directly to the window surface

    Dim win_surface_ptr As IntPtr  ' SDL_Surface
    Dim img_ptr As IntPtr  ' SDL_Surface


    Sub Setup()
        SDL_Init(SDL_INIT_VIDEO)

        window_ptr = SDL_CreateWindow(
            "Hello SDL",
            SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED,
            320, 240,
            SDL_WindowFlags.SDL_WINDOW_SHOWN)


        win_surface_ptr = SDL_GetWindowSurface(window_ptr)
        Debug.Print(SDL_GetError)

        Dim win_surface = Marshal.PtrToStructure(Of SDL_Surface)(win_surface_ptr)

        Dim temp = SDL_LoadBMP("meme.bmp")
        Debug.Print(SDL_GetError)

        img_ptr = SDL_ConvertSurface(temp, win_surface.format, 0)
        Debug.Print(SDL_GetError)

        SDL_FreeSurface(temp)

    End Sub

    Sub CleanUp()
        SDL_FreeSurface(img_ptr)

        SDL_DestroyWindow(window_ptr)
        SDL_Quit()
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


    Sub Render()
        Dim dest_rect As New SDL_Rect With {
            .x = 0,
            .y = 0,
            .w = 320,
            .h = 240
        }

        'SDL_BlitSurface(img, IntPtr.Zero, win_surface_ptr, IntPtr.Zero)
        SDL_BlitScaled(img_ptr, IntPtr.Zero, win_surface_ptr, dest_rect)

        SDL_UpdateWindowSurface(window_ptr)
    End Sub
End Class
