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
        string preAmble = $"#define PARTICLECOUNT 2\n";
        var velocityUpdater = new ComputeShader(Paths.VELOCITYUPDATERPATH, preAmble, bufferHandler);
        Assert.IsNotNull(velocityUpdater);

        //Initial positions and velocities:
        float[] velocities = [0, 0, 1, 0, -2, 0];
        float[] forces = [0, 0, 0, 1, 2, 0,
                         -3, -5, 0, 0, 0, 0];

        //Create shader buffers and run simulation:
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.velocitiesCurrent, velocities, BufferUsageHint.StreamDraw);
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.velocitiesFuture, velocities, BufferUsageHint.StreamDraw);
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.forcesCurrent, forces, BufferUsageHint.StreamDraw);

        velocityUpdater.bufferLocations.Add(0, GameEngine.Buffer.velocitiesCurrent);
        velocityUpdater.bufferLocations.Add(1, GameEngine.Buffer.velocitiesFuture);
        velocityUpdater.bufferLocations.Add(2, GameEngine.Buffer.forcesCurrent);

        velocityUpdater.SetInt("offSetX", 0);
        velocityUpdater.SetFloat("deltaTime", 1);

        velocityUpdater.Dispatch1D(2);

        //Check results:
        float[] velocities_out = bufferHandler.GetBufferData(GameEngine.Buffer.velocitiesFuture, 6);
        float[] velocities_out_goal = new float[2 * 3];
        for (int i = 0; i < 2; i++)
        {
            for (int k = 0; k < 3; k++)
            {
                velocities_out_goal[3 * i + k] = velocities[3 * i + k];
                for (int j = 0; j < 2 - 1; j++)
                {
                    velocities_out_goal[3 * i + k] += forces[6 * i + 3 * (i <= j ? 1 : 0) + k];
                }
            }
        }
        CollectionAssert.AreEqual(velocities_out_goal, velocities_out);
    }

    //Same structure as TwoParticles.
}