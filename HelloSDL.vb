Imports SDL2.SDL

Public Class HelloSDL
    Dim running As Boolean

    Public Sub New()
        running = True

        Setup()

        Do
            ' basically update & render
            PollEvents()
            Render()
        Loop While running

        CleanUp()
    End Sub


    Dim window As IntPtr
    Dim renderer As IntPtr

    Enum LineFillMode
        None

        ''' <summary>
        ''' border
        ''' </summary>
        B

        ''' <summary>
        ''' border + fill
        ''' </summary>
        BF
    End Enum

    ' Ref: https://jsayers.dev/c-sdl-tutorial-part-2-creating-a-window/

    Sub SetColour(rgb%)
        Dim r = CByte(rgb \ &H10000)
        Dim g = CByte(rgb \ &H100 Mod &H100)
        Dim b = CByte(rgb Mod &H100)

        SDL_SetRenderDrawColor(renderer, r, g, b, 255)
    End Sub


    ''' <summary>
    ''' Based on QuickBASIC's LINE Statement
    ''' </summary>
    ''' <param name="colour">RGB value</param>
    Sub Line(
            x1%, y1%, x2%, y2%,
            colour%,
            Optional border_fill As LineFillMode = LineFillMode.None)  ' , B Or BF, style%

        Dim r, g, b, a As Byte
        SDL_GetRenderDrawColor(renderer, r, g, b, a)

        SetColour(colour)

        Dim rect As SDL_Rect

        If border_fill = LineFillMode.BF OrElse border_fill = LineFillMode.B Then
            Dim min_x% = Math.Min(x1, x2)
            Dim min_y% = Math.Min(y1, y2)
            Dim max_x% = Math.Max(x1, x2)
            Dim max_y% = Math.Max(y1, y2)

            rect = New SDL_Rect With {
                .x = x1, .y = y1,
                .w = max_x - min_x, .h = max_y - min_y
            }
        End If


        Select Case border_fill
            Case LineFillMode.BF
                SDL_RenderFillRect(renderer, rect)

                rect = Nothing

            Case LineFillMode.B
                SDL_RenderDrawRect(renderer, rect)

                rect = Nothing
            Case Else
                SDL_RenderDrawLine(renderer, x1, y1, x2, y2)
        End Select

        SDL_SetRenderDrawColor(renderer, r, g, b, a)
    End Sub


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
    End Sub


    Sub CleanUp()
        SDL_DestroyRenderer(renderer)
        SDL_DestroyWindow(window)
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
        ' SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255)
        SetColour(&H6495ED)
        SDL_RenderClear(renderer)

        Line(0, 0, 320, 240, &HFF0000)

        Line(30, 10, 50, 30, &H0, LineFillMode.BF)

        SDL_RenderPresent(renderer)
    End Sub
End Class
