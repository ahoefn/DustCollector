# DustCollector

In this repo is a simple particle simulator I have built / am building. Once opened a cube of particles is generated and one can freely fly around these particles and start/stop the simulation. The controls are as follows:
   
 * WASD for planar motion,
 * SHIFT/CTRL for up down motion,
 * MOUSE to look around,
 * SPACEBAR to start/stop the simulation,
 * R to restart the simulation,
 * ESC to exit the window.

There are also various settings that can be adjusted by changing the values in Settings.cs. Note that these settings require that the program is recompiled. 

### Compiling

The program itself can be run by using the command

    dotnet run
    
while in the "source" folder. Otherwise, one can use

    dotnet build -c Release
    dotnet run -c Release
    
while in the "source" folder. Similarly, the tests can be run and compiled using the same commands while in the "Tests" folder.

### Code structure.

Both "source" and "Tests" contain a markdown file explaining the code structure in more detail.

### Dependencies
[OpenTK v4.9.3](https://www.nuget.org/packages/OpenTK/)

### Requirements
Graphics card must support OpenGL v4.5 or later.
