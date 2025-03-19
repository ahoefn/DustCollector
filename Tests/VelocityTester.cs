using OpenTK.Graphics.OpenGL4;
using DustCollector.GameEngine;
using DustCollector.GameEngine.Shaders;
namespace DustCollector.Tests;

public sealed class VelocityTester
{
    public static void TwoParticles(TestParams testParams)
    {
        //Make sure GL context is correct and compile shader:
        testParams.window.MakeCurrent();
        GL.UseProgram(testParams.program);
        var bufferHandler = new BufferHandler();

        int N = 2;
        string preAmble = $"#define PARTICLECOUNT {N}\n";
        var velocityUpdater = new ComputeShader(Paths.VELOCITYUPDATERPATH, preAmble, bufferHandler);
        Assert.IsNotNull(velocityUpdater);

        //Initial positions and velocities:
        //                   |   P1  |    P2    |
        float[] velocities = [0, 0, 1, 0, -2, 0];
        //                Fx|Fy|Fz|
        float[] forces = [1, 2, 0,  // P1
                         -3, -5, 0];// P2

        //Create shader buffers and run simulation:
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.velocitiesCurrent, velocities, BufferUsageHint.StreamDraw);
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.velocitiesFuture, velocities, BufferUsageHint.StreamDraw);
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.forcesCurrent, forces, BufferUsageHint.StreamDraw);

        velocityUpdater.bufferLocations.Add(0, GameEngine.Buffer.velocitiesCurrent);
        velocityUpdater.bufferLocations.Add(1, GameEngine.Buffer.velocitiesFuture);
        velocityUpdater.bufferLocations.Add(2, GameEngine.Buffer.forcesCurrent);

        velocityUpdater.SetInt("offSetX", 0);
        velocityUpdater.SetFloat("deltaTime", 1);

        velocityUpdater.Dispatch1D(N);

        //Check results:
        float[] velocities_out = bufferHandler.GetBufferData(GameEngine.Buffer.velocitiesFuture, 3 * N);
        float[] velocities_out_goal = new float[N * 3];
        for (int particleNumber = 0; particleNumber < N; particleNumber++)
        {
            for (int dir = 0; dir < 3; dir++)
            {
                int index = 3 * particleNumber + dir;
                velocities_out_goal[index] = velocities[index];
                for (int innerParticle = 0; innerParticle < N - 1; innerParticle++)
                {
                    int forceRowStart = 3 * (N - 1) * particleNumber;
                    int forceColumn = (N - 1) * dir + innerParticle;
                    velocities_out_goal[index] += forces[forceRowStart + forceColumn];
                }
            }
        }
        CollectionAssert.AreEqual(velocities_out_goal, velocities_out);
    }

    //Same structure as TwoParticles.
    public static void FourParticles(TestParams testParams)
    {
        //Make sure GL context is correct and compile shader:
        int N = 4;
        testParams.window.MakeCurrent();
        GL.UseProgram(testParams.program);
        var bufferHandler = new BufferHandler();
        string preAmble = $"#define PARTICLECOUNT {N}\n";
        var velocityUpdater = new ComputeShader(Paths.VELOCITYUPDATERPATH, preAmble, bufferHandler);
        Assert.IsNotNull(velocityUpdater);

        //Initial positions and velocities:
        //                   |   P1  |    P2  |    P3  |    P4
        float[] velocities = [1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0];
        //               |  Fix   |   Fiy |  Fizz  |
        float[] forces = [0, 0, 1, 2, 0, 3, 0, 0, 1, // P1
                          0, 2, 0, 0, 1, 2, 0, 3, 0, // P2
                          0, 1, 0, 2, 3, 5, 0, 0, 7, // P3
                          0, 0, 1, 1, 0, 0, 0, 0, 1];// P4

        //Create shader buffers and run simulation:
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.velocitiesCurrent, velocities, BufferUsageHint.StreamDraw);
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.velocitiesFuture, velocities, BufferUsageHint.StreamDraw);
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.forcesCurrent, forces, BufferUsageHint.StreamDraw);

        velocityUpdater.bufferLocations.Add(0, GameEngine.Buffer.velocitiesCurrent);
        velocityUpdater.bufferLocations.Add(1, GameEngine.Buffer.velocitiesFuture);
        velocityUpdater.bufferLocations.Add(2, GameEngine.Buffer.forcesCurrent);

        velocityUpdater.SetInt("offSetX", 0);
        velocityUpdater.SetFloat("deltaTime", 1);

        velocityUpdater.Dispatch1D(N);

        //Check results:
        float[] velocities_out = bufferHandler.GetBufferData(GameEngine.Buffer.velocitiesFuture, 3 * N);
        float[] velocities_out_goal = new float[N * 3];
        for (int particleNumber = 0; particleNumber < N; particleNumber++)
        {
            for (int dir = 0; dir < 3; dir++)
            {
                int index = 3 * particleNumber + dir;
                velocities_out_goal[index] = velocities[index];
                for (int innerParticle = 0; innerParticle < N - 1; innerParticle++)
                {
                    int forceRowStart = 3 * (N - 1) * particleNumber;
                    int forceColumn = (N - 1) * dir + innerParticle;
                    velocities_out_goal[index] += forces[forceRowStart + forceColumn];
                }
            }
        }
        CollectionAssert.AreEqual(velocities_out_goal, velocities_out);
    }
    public static void NParticlesRand(TestParams testParams)
    {
        //Make sure GL context is correct and compile shader:
        if (testParams.N == null) { throw new ArgumentException("N must not be null.", nameof(testParams.N)); }
        int N = (int)testParams.N;
        testParams.window.MakeCurrent();
        GL.UseProgram(testParams.program);
        var bufferHandler = new BufferHandler();
        string preAmble = $"#define PARTICLECOUNT {N}\n";
        var velocityUpdater = new ComputeShader(Paths.VELOCITYUPDATERPATH, preAmble, bufferHandler);
        Assert.IsNotNull(velocityUpdater);

        var random = new Random();
        //Initial positions and velocities:
        var velocities = new float[N * 3];
        for (int i = 0; i < velocities.Length; i++)
        {
            velocities[i] = random.NextSingle();
        }
        var forces = new float[N * (N - 1) * 3];
        for (int i = 0; i < forces.Length; i++)
        {
            forces[i] = random.NextSingle();
        }
        //Create shader buffers and run simulation:
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.velocitiesCurrent, velocities, BufferUsageHint.StreamDraw);
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.velocitiesFuture, velocities, BufferUsageHint.StreamDraw);
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.forcesCurrent, forces, BufferUsageHint.StreamDraw);

        velocityUpdater.bufferLocations.Add(0, GameEngine.Buffer.velocitiesCurrent);
        velocityUpdater.bufferLocations.Add(1, GameEngine.Buffer.velocitiesFuture);
        velocityUpdater.bufferLocations.Add(2, GameEngine.Buffer.forcesCurrent);

        velocityUpdater.SetInt("offSetX", 0);
        velocityUpdater.SetFloat("deltaTime", 1);

        velocityUpdater.Dispatch1D(N);

        //Check results:
        float[] velocities_out = bufferHandler.GetBufferData(GameEngine.Buffer.velocitiesFuture, 3 * N);
        float[] velocities_out_goal = new float[N * 3];
        for (int particleNumber = 0; particleNumber < N; particleNumber++)
        {
            for (int dir = 0; dir < 3; dir++)
            {
                int index = 3 * particleNumber + dir;
                velocities_out_goal[index] = velocities[index];
                for (int innerParticle = 0; innerParticle < N - 1; innerParticle++)
                {
                    int forceRowStart = 3 * (N - 1) * particleNumber;
                    int forceColumn = (N - 1) * dir + innerParticle;
                    velocities_out_goal[index] += forces[forceRowStart + forceColumn];
                }
            }
        }
        CollectionAssert.AreEqual(velocities_out_goal, velocities_out, new FloatComparer(0.00001f));
    }

}
