using OpenTK.Graphics.OpenGL4;
using DustCollector.GameEngine;
using DustCollector.GameEngine.Shaders;
namespace DustCollector.Tests;

public sealed class PositionTester
{
    public static void TwoParticles(TestParams testParams)
    {
        //Make sure GL context is correct and compile shader:
        testParams.window.MakeCurrent();
        GL.UseProgram(testParams.program);
        var bufferHandler = new BufferHandler();
        var positionUpdater = new ComputeShader(Paths.POSITIONUPDATERPATH, bufferHandler);
        Assert.IsNotNull(positionUpdater);

        //Initial positions and velocities:
        float[] positions = [-1, 0, 0, 1, 0, 0];
        float[] velocities = [0, 0, 1, 0, -2, 0];

        //Create shader buffers and run simulation:
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.positionsCurrent, positions, BufferUsageHint.StreamDraw);
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.positionsFuture, positions, BufferUsageHint.StreamDraw);
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.velocitiesCurrent, velocities, BufferUsageHint.StreamDraw);

        positionUpdater.bufferLocations.Add(0, GameEngine.Buffer.positionsCurrent);
        positionUpdater.bufferLocations.Add(1, GameEngine.Buffer.positionsFuture);
        positionUpdater.bufferLocations.Add(2, GameEngine.Buffer.velocitiesCurrent);

        positionUpdater.SetInt("offSetX", 0);
        positionUpdater.SetFloat("deltaTime", 1);

        positionUpdater.Dispatch1D(2);

        //Check results:
        float[] positions_out = bufferHandler.GetBufferData(GameEngine.Buffer.positionsFuture, 6);
        float[] positions_out_goal = new float[2 * 3];
        for (int i = 0; i < 6; i++)
        {
            positions_out_goal[i] = positions[i] + velocities[i];
        }
        CollectionAssert.AreEqual(positions_out_goal, positions_out);
    }

    //Same structure as TwoParticles.
}