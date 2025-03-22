// Note: It seems like a proper testing environment (such as MSTest) can unfortunately not be used with OpenTK
// as OpenTK always requires you to run in the main thread, which during testing can not be guaranteed
// (at least with MSTest).

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
namespace DustCollector.Tests;

//Input struct for test functions:
public struct TestParams
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

//Main program:
class TestProgram
{
    private static void Main(string[] args)
    {

        // Setup:
        var windowSettings = new NativeWindowSettings()
        {
            StartVisible = false
        };
        var window = new GameWindow(new GameWindowSettings(), windowSettings);
        int program = GL.CreateProgram();
        var testParams = new TestParams(window, program);
        testParams.N = 800;

        // Position tests:
        PositionTester.TwoParticles(testParams);

        // Velocity tests:
        VelocityTester.TwoParticles(testParams);
        VelocityTester.FourParticles(testParams);
        VelocityTester.NParticlesRand(testParams);

        //Force tests:
        new ForceTester(testParams);
    }

}