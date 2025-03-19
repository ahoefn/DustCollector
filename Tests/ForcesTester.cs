using OpenTK.Graphics.OpenGL4;
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

        velocityUpdater.Dispatch1D(N);

        //Check results:
        float[] forcesOut = bufferHandler.GetBufferData(GameEngine.Buffer.forcesFuture, 3 * N * (N - 1));
        CollectionAssert.AreEqual(forcesOut, forcesOutGoal);
    }

}