using OpenTK.Graphics.OpenGL4;
namespace DustCollector.Tests;

/// <summary>
/// General tester class containing the methods necessary for initializing and running tests.
/// </summary>
class Tester
{
    public Tester(string path_in)
    {
        _path = path_in;
    }
    // Utility func useful in a variety of tests:
    protected virtual void InitializeTest(TestParams testParams)
    {
        //Make sure GL context is correct and compile shader:
        testParams.window.MakeCurrent();
        GL.UseProgram(testParams.program);
        testParams.bufferHandler = new GameEngine.BufferHandler();
        testParams.shader = new GameEngine.Shaders.ComputeShader(_path, testParams.bufferHandler);
        Assert.IsNotNull(testParams.shader);
    }
    protected readonly string _path;
    protected static void EndTest(TestParams testParams)
    {
        testParams.bufferHandler?.Dispose();
        testParams.shader?.Dispose();

        GL.DeleteProgram(testParams.program);

    }
    protected void RunTest(Action<TestParams> testFunc, TestParams testParams)
    {
        InitializeTest(testParams);
        testFunc(testParams);
        EndTest(testParams);
    }
    protected static float[] GenerateRandomArray(int length)
    {
        var output = new float[length];
        var random = new Random();
        for (int i = 0; i < length; i++)
        {
            output[i] = random.NextSingle();
        }
        return output;
    }
}