namespace DustCollector;

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