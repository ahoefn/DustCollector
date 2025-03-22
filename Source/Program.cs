namespace DustCollector;

class Program
{
    static void Main(string[] args)
    {
        if (Globals.OPENGLDEBUGGING)
        {
            using var game = new Game(1080, 800, "DustCollector test", debug: true);
            game.Run();
        }
        else
        {
            using var game = new Game(1080, 800, "DustCollector test");
            game.Run();
        }
    }
}