Imports SDL2.SDL
Imports SDL2.SDL_ttf
Imports System.Runtime.InteropServices

' Refs:

' Render TTF
' Easy example:
' https://stackoverflow.com/questions/36198732
' Example with text width & height
' https://stackoverflow.com/questions/22886500
' Example from C++:
' https://thenumb.at/cpp-course/sdl2/07/07.html

' Marshal.PtrToStructure
' https://stackoverflow.com/questions/27741351/
' https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.ptrtostructure?view=net-8.0

Public Class DrawStringExample
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


    Dim window As IntPtr
    Dim renderer As IntPtr

    Sub SetColour(rgb%)
        Dim r = CByte(rgb \ &H10000)
        Dim g = CByte(rgb \ &H100 Mod &H100)
        Dim b = CByte(rgb Mod &H100)

        SDL_SetRenderDrawColor(renderer, r, g, b, 255)
    End Sub

    Dim times As IntPtr


    Sub Setup()
        SDL_Init(SDL_INIT_VIDEO)

        window = SDL_CreateWindow(
            "Hello SDL",
            SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED,
            320, 240,
            SDL_WindowFlags.SDL_WINDOW_SHOWN)

        renderer = SDL_CreateRenderer(
            window, -1,
            SDL_RendererFlags.SDL_RENDERER_ACCELERATED Or SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC)


        TTF_Init()
        times = TTF_OpenFont("TIMES.TTF", 20)
        Dim error$ = TTF_GetError()

        If Not String.IsNullOrWhiteSpace(error$) Then
            Debug.Print("Error when loading TTF: {0}", error$)
        End If
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
        SetColour(&H6495ED)
        SDL_RenderClear(renderer)

        Dim white As New SDL_Color With {
            .r = 255,
            .g = 255,
            .b = 255,
            .a = 0
        }

        Dim surface_ptr = TTF_RenderText_Solid(times, "Hello!", white)
        Dim surface = Marshal.PtrToStructure(Of SDL_Surface)(surface_ptr)

        Dim txt_width = surface.w
        Dim txt_height = surface.h

        Dim dst_rect As New SDL_Rect With {
            .x = 0,
            .y = 0,
            .w = txt_width,
            .h = txt_height
        }

        Dim texture = SDL_CreateTextureFromSurface(renderer, surface_ptr)

        SDL_RenderCopy(renderer, texture, IntPtr.Zero, dst_rect)

        SDL_FreeSurface(surface_ptr)
        SDL_DestroyTexture(texture)

        SDL_RenderPresent(renderer)
    End Sub


    Sub CleanUp()
        TTF_CloseFont(times)
        TTF_Quit()

        SDL_DestroyRenderer(renderer)
        SDL_DestroyWindow(window)
        SDL_Quit()
    End Sub
End Class
