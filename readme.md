# SDL VB.NET (.NET Core 6)

A few examples on how to start using SDL2 with VB.NET.


## File list

- Program.vb

	This is the starting point of the project.


- HelloSDL.vb

	This example demonstrates how to start SDL window & renderer.


- DrawStringExample.vb

	This  example demonstrates how to draw string with `SDL_ttf`.


- DrawBMPExample.vb

	This example demonstrates how to load a BMP file & blitting directly to window.


- DrawPNGExample.vb

	This example demonstrates how to load a PNG file & rendering on the surface.


- DrawPNGRotated.vb

	This example demonstrates how to load a TTF & PNG file & renders them rotated, each as a texture.


- PlaySoundExample.vb

	This example demonstrates how to load a WAV file & plays it with Spacebar using `SDL_mixer`.


## Prerequisites

- Microsoft Visual Studio 2022
- .NET Core 6 (LTS)


## Dependencies

This project uses this NuGet package: https://www.nuget.org/packages/Sayers.SDL2.Core/

It is compatible with .NET Core 6, x64 processor architecture.  It is also bundled with the DLLs that are required to run SDL2.

The SDL2 bindings are from: https://github.com/flibitijibibo/SDL2-CS/

In case the dependencies aren't loaded / installed, you can reinstall them with these steps:

1. On Visual Studio, open **Tools** menu --> NuGet Package Manager --> Package Manager Console,
2. Use this command:
	```
	Update-Package -reinstall
	```

[Reference](https://stackoverflow.com/questions/6876732/)

