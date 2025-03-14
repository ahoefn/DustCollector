namespace DustCollector;
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