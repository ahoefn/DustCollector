﻿namespace DustCollector;

/// <summary>
/// Entry point of the program, creates a Game instance and runs it.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        if (Settings.OPENGLDEBUGGING)
        {
            using var game = new Game(Settings.WINDOWWIDTH, Settings.WINDOWHEIGHT, "DustCollector test", debug: true);
            game.Run();
        }
        else
        {
            using var game = new Game(Settings.WINDOWWIDTH, Settings.WINDOWHEIGHT, "DustCollector test");
            game.Run();
        }
    }
}