// Note: It seems like a proper testing environment (such as MSTest) can unfortunately not be used with OpenTK
// as OpenTK always requires you to run in the main thread, which during testing can not be guaranteed
// (at least with MSTest).

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
namespace DustCollector.Tests;

/// <summary>
/// Entry point for the test program, creates a the classes in the "Tester" folder which run the tests.
/// </summary>
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
        new PositionTester(testParams);

        // Velocity tests:
        new VelocityTester(testParams);

        //Force tests:
        new ForceTester(testParams);
    }

}