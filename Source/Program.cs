﻿namespace DustCollector;

//TODO: add explanation comments at top of each file.
class Program
{
    static void Main(string[] args)
    {
        using (var game = new Game(800, 600, "DustCollector test"))
        {
            game.Run();
        }
    }
}