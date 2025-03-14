using OpenTK.Graphics.OpenGL4;
namespace DustCollector.Tests;

public sealed class GravityTester
{
    public static void TwoParticles(TestParams testParams)
    {
        //Make sure GL context is correct and compile shader:
        testParams.window.MakeCurrent();
        GL.UseProgram(testParams.program);
        var velocityUpdater = new Renderer.ComputeShader("Shaders/VelocityUpdater.comp");
        Assert.IsNotNull(velocityUpdater);

        //Initial positions and velocities:
        float[] positions = [-1, 0, 0, 1, 0, 0];
        float[] velocities = [0, 0, 0, 0, 0, 0];

        //Create shader buffers and run simulation:
        velocityUpdater.CreateStorageBuffer("positions", positions, 0, BufferUsageHint.DynamicDraw);
        velocityUpdater.CreateStorageBuffer("velocities", velocities, 1, BufferUsageHint.DynamicDraw);

        velocityUpdater.SetInt("offSetX", 0);
        velocityUpdater.SetFloat("deltaTime", 1);

        velocityUpdater.Dispatch(1, 1, 1);

        //Check results:
        float[] positions_out = [-10, -10, -10, -10, -10, -10];
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, velocityUpdater.buffers["positions"]);
        GL.GetBufferSubData(BufferTarget.ShaderStorageBuffer, 0, 6 * sizeof(float), positions_out);
        CollectionAssert.AreEqual(positions, positions_out);

        float velocityShift = 0.1f * 1 / 4;
        float[] velocities_expected = [velocityShift, 0, 0, -velocityShift, 0, 0];
        float[] velocities_out = [-10, -10, -10, -10, -10, -10];
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, velocityUpdater.buffers["velocities"]);
        GL.GetBufferSubData(BufferTarget.ShaderStorageBuffer, 0, 6 * sizeof(float), velocities_out);
        CollectionAssert.AreEqual(velocities_out, velocities_expected);
    }

    //Same structure as TwoParticles.
    public static void FourParticles(TestParams testParams)
    {
        string currentError;
        testParams.window.MakeCurrent();
        GL.UseProgram(testParams.program);
        currentError = GL.GetError().ToString();
        var velocityUpdater = new Renderer.ComputeShader("/Shaders/VelocityUpdater.comp");
        Assert.IsNotNull(velocityUpdater);
        float[] positions = [-1, 0, 0, 1, 0, 0, 1, 0, 0, 0, -1, 0];
        float[] velocities = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

        velocityUpdater.CreateStorageBuffer("positions", positions, 0, BufferUsageHint.DynamicDraw);
        currentError = GL.GetError().ToString();
        velocityUpdater.CreateStorageBuffer("velocities", velocities, 1, BufferUsageHint.DynamicDraw);
        currentError = GL.GetError().ToString();

        velocityUpdater.SetInt("offSetX", 0);
        currentError = GL.GetError().ToString();
        velocityUpdater.SetFloat("deltaTime", 1);
        currentError = GL.GetError().ToString();

        velocityUpdater.Dispatch(4 * (4 + 1) / 2, 1, 1);
        currentError = GL.GetError().ToString();

        float[] positions_out = new float[3 * 4];
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, velocityUpdater.buffers["positions"]);
        currentError = GL.GetError().ToString();
        GL.GetBufferSubData(BufferTarget.ShaderStorageBuffer, 0, positions_out.Length * sizeof(float), positions_out);
        currentError = GL.GetError().ToString();
        CollectionAssert.AreEqual(positions, positions_out);

        float velocityShift = 0.1f * 1 / 4;
        float[] velocities_expected = [velocityShift, 0, 0, -velocityShift, 0, 0];
        float[] velocities_out = new float[3 * 4];
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, velocityUpdater.buffers["velocities"]);
        currentError = GL.GetError().ToString();
        GL.GetBufferSubData(BufferTarget.ShaderStorageBuffer, 0, velocities_out.Length * sizeof(float), velocities_out);
        currentError = GL.GetError().ToString();
        CollectionAssert.AreEqual(velocities_out, velocities_expected);
        Console.WriteLine(currentError);
    }
}
