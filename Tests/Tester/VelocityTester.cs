using OpenTK.Graphics.OpenGL4;
namespace DustCollector.Tests;

/// <summary>
/// Performs a simple test on the velocity compute shader. Creating an instance of this class runs the test.
/// </summary>
sealed class VelocityTester : Tester
{
    public VelocityTester(TestParams tP) : base(Paths.VELOCITYUPDATERPATH)
    {
        tP.N = 2;
        RunTest(TwoParticles, tP);
        tP.N = 4;
        RunTest(FourParticles, tP);
        tP.N = 800;
        RunTest(NParticlesRand, tP);
    }

    // Test methods:
    private static void TwoParticles(TestParams testParams)
    {
        if (testParams.N != 2) { throw new ArgumentException("testParams.N must be two in this test."); }

        //Initial positions and velocities:
        //                   |   P1  |    P2    |
        float[] velocities = [0, 0, 1, 0, -2, 0];
        //                Fx|Fy|Fz|
        float[] forces = [1, 2, 0,  // P1
                         -3, -5, 0];// P2

        float[] velocitiesOut = GetShaderOutput(testParams, velocities, forces);
        float[] velocitiesOutExpected = GetExpectedOutput(velocities, forces);
        CollectionAssert.AreEqual(velocitiesOutExpected, velocitiesOut);
    }

    //Same structure as TwoParticles.
    private static void FourParticles(TestParams testParams)
    {
        if (testParams.N != 4) { throw new ArgumentException("testParams.N must be four in this test."); }

        //Initial positions and velocities:
        //                   |   P1  |    P2  |    P3  |    P4
        float[] velocities = [1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0];
        //               |  Fix   |   Fiy |  Fizz  |
        float[] forces = [0, 0, 1, 2, 0, 3, 0, 0, 1, // P1
                          0, 2, 0, 0, 1, 2, 0, 3, 0, // P2
                          0, 1, 0, 2, 3, 5, 0, 0, 7, // P3
                          0, 0, 1, 1, 0, 0, 0, 0, 1];// P4


        float[] velocitiesOut = GetShaderOutput(testParams, velocities, forces);
        float[] velocitiesOutExpected = GetExpectedOutput(velocities, forces);
        CollectionAssert.AreEqual(velocitiesOut, velocitiesOutExpected);
    }
    private static void NParticlesRand(TestParams testParams)
    {
        //Make sure GL context is correct and compile shader:
        if (testParams.N == null) { throw new ArgumentException("N must not be null.", nameof(testParams.N)); }
        int N = (int)testParams.N;

        float[] velocities = GenerateRandomArray(3 * N);
        float[] forces = GenerateRandomArray(3 * N * (N - 1));

        float[] velocitiesOut = GetShaderOutput(testParams, velocities, forces);
        float[] velocitiesOutExpected = GetExpectedOutput(velocities, forces);
        CollectionAssert.AreEqual(velocitiesOut, velocitiesOutExpected);
    }

    // Computation method
    private static float[] GetShaderOutput(TestParams tP, float[] velocities, float[] forces)
    {
        if (tP.bufferHandler == null) { throw new ArgumentException("Buffer handler can not be zero while testing."); }
        if (tP.shader == null) { throw new ArgumentException("Shader can not be zero while testing."); }

        int N = velocities.Length / 3;
        //Create shader buffers and run simulation:
        tP.bufferHandler.CreateStorageBuffer(GameEngine.Buffer.velocitiesCurrent, velocities, BufferUsageHint.StreamDraw);
        tP.bufferHandler.CreateStorageBuffer(GameEngine.Buffer.velocitiesFuture, velocities, BufferUsageHint.StreamDraw);
        tP.bufferHandler.CreateStorageBuffer(GameEngine.Buffer.forcesCurrent, forces, BufferUsageHint.StreamDraw);

        tP.shader.bufferLocations.Add(0, GameEngine.Buffer.velocitiesCurrent);
        tP.shader.bufferLocations.Add(1, GameEngine.Buffer.velocitiesFuture);
        tP.shader.bufferLocations.Add(2, GameEngine.Buffer.forcesCurrent);

        tP.shader.SetInt("offSetX", 0);
        tP.shader.SetFloat("deltaTime", 1);

        tP.shader.Dispatch1D(N);

        // Get results:
        float[] velocities_out = tP.bufferHandler.GetBufferData(GameEngine.Buffer.velocitiesFuture, 3 * N);
        return velocities_out;
    }
    // Expected output
    private static float[] GetExpectedOutput(float[] velocities, float[] forces)
    {
        int N = velocities.Length / 3;
        float[] velocities_out = new float[N * 3];
        for (int particleNumber = 0; particleNumber < N; particleNumber++)
        {
            for (int dir = 0; dir < 3; dir++)
            {
                int index = 3 * particleNumber + dir;
                velocities_out[index] = velocities[index];
                for (int innerParticle = 0; innerParticle < N - 1; innerParticle++)
                {
                    int forceRowStart = 3 * (N - 1) * particleNumber;
                    int forceColumn = (N - 1) * dir + innerParticle;
                    velocities_out[index] += forces[forceRowStart + forceColumn];
                }
            }
        }

        // Make pass to match maximum velocity:
        int MAXVELOCITY = 10; // Must be the same as in VelocityUpdater.comp
        for (int i = 0; i < 3 * N; i++)
        {
            velocities_out[i] = Math.Max(velocities_out[i], -MAXVELOCITY);
            velocities_out[i] = Math.Min(velocities_out[i], MAXVELOCITY);
        }
        return velocities_out;
    }

    // Tester override to include header:
    protected override void InitializeTest(TestParams tP)
    {
        //Make sure GL context is correct and compile shader:
        tP.window.MakeCurrent();
        GL.UseProgram(tP.program);
        tP.bufferHandler = new GameEngine.BufferHandler();
        string preAmble = $"#define PARTICLECOUNT {tP.N}\n";
        tP.shader = new GameEngine.Shaders.ComputeShader(_path, preAmble, tP.bufferHandler);
        Assert.IsNotNull(tP.shader);
        Assert.IsNotNull(tP.bufferHandler);
    }

}
