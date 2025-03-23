using OpenTK.Windowing.Desktop;
namespace DustCollector.Tests;

/// <summary>
/// A class containing all of the data used throughout the different tests.
/// </summary>
public class TestParams
{
    public TestParams(GameWindow window_in, int program_in)
    {
        window = window_in;
        program = program_in;
    }
    public readonly GameWindow window;
    public GameEngine.BufferHandler? bufferHandler;
    public GameEngine.Shaders.ComputeShader? shader;
    public readonly int program;
    public int? N;
}