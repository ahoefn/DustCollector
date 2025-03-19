using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using DustCollector.GameEngine;
using DustCollector.GameEngine.Shaders;
namespace DustCollector.Tests;

public sealed class ForceTester
{
    public static void TwoParticles(TestParams testParams)
    {
        //Make sure GL context is correct and compile shader:
        testParams.window.MakeCurrent();
        GL.UseProgram(testParams.program);
        var bufferHandler = new BufferHandler();

        int N = 2;
        var velocityUpdater = new ComputeShader(Paths.FORCEUPDATERPATH, bufferHandler);
        Assert.IsNotNull(velocityUpdater);

        //Initial positions and velocities:
        //                   |   P1  |    P2    |
        float[] positions = [0, 0, 1, 0, 0, -1];
        float[] forcesIn = new float[3 * N * (N - 1)];


        //                       Fx|Fy| Fz |
        float[] forcesOutGoal = [0, 0, -0.25f, // P1
                                 0, 0, 0.25f];// P2


        //Create shader buffers and run simulation:
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.positionsCurrent, positions, BufferUsageHint.StreamDraw);
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.forcesFuture, forcesIn, BufferUsageHint.StreamDraw);

        velocityUpdater.bufferLocations.Add(0, GameEngine.Buffer.positionsCurrent);
        velocityUpdater.bufferLocations.Add(1, GameEngine.Buffer.forcesFuture);

        velocityUpdater.SetInt("offSetX", 0);
        velocityUpdater.SetInt("particleCount", N);

        velocityUpdater.Dispatch1D(N * (N - 1) / 2);

        //Check results:
        float[] forcesOut = bufferHandler.GetBufferData(GameEngine.Buffer.forcesFuture, 3 * N * (N - 1));
        CollectionAssert.AreEqual(forcesOut, forcesOutGoal);
    }
    public static void FourParticles(TestParams testParams)
    {
        //Make sure GL context is correct and compile shader:
        testParams.window.MakeCurrent();
        GL.UseProgram(testParams.program);
        var bufferHandler = new BufferHandler();

        int N = 4;
        var velocityUpdater = new ComputeShader(Paths.FORCEUPDATERPATH, bufferHandler);
        Assert.IsNotNull(velocityUpdater);

        //Initial positions and velocities:
        //                   |   P1  |    P2  |    P3   |   P4     |
        float[] positions = [1, 1, 0, -1, 1, 0, 1, -1, 0, -1, -1, 0];
        float[] forcesIn = new float[3 * N * (N - 1)];
        float[] forcesOutGoal = new float[3 * N * (N - 1)];

        Vector3 force;
        Vector3 positionOut;
        Vector3 positionIn;
        Vector3 distance;
        for (int particleOut = 0; particleOut < N; particleOut++)
        {
            force = new Vector3(0, 0, 0);
            positionOut = new Vector3(positions[3 * particleOut], positions[3 * particleOut + 1], positions[3 * particleOut + 2]);
            for (int particleIn = 0; particleIn < N; particleIn++)
            {
                positionOut = new Vector3(positions[3 * particleOut], positions[3 * particleOut + 1], positions[3 * particleOut + 2]);
                distance = new Vector3(positions[3 * particleOut])
            }
        }




        //Create shader buffers and run simulation:
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.positionsCurrent, positions, BufferUsageHint.StreamDraw);
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.forcesFuture, forcesIn, BufferUsageHint.StreamDraw);

        velocityUpdater.bufferLocations.Add(0, GameEngine.Buffer.positionsCurrent);
        velocityUpdater.bufferLocations.Add(1, GameEngine.Buffer.forcesFuture);

        velocityUpdater.SetInt("offSetX", 0);
        velocityUpdater.SetInt("particleCount", N);

        velocityUpdater.Dispatch1D(N * (N - 1) / 2);

        //Check results:
        float[] forcesOut = bufferHandler.GetBufferData(GameEngine.Buffer.forcesFuture, 3 * N * (N - 1));
        CollectionAssert.AreEqual(forcesOut, forcesOutGoal);
    }
}