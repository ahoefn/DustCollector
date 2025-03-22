using OpenTK.Graphics.OpenGL4;
using DustCollector.GameEngine;
using DustCollector.GameEngine.Shaders;
namespace DustCollector.Tests;

sealed class PositionTester : Tester
{
    public PositionTester(TestParams tP) : base(Paths.POSITIONUPDATERPATH)
    {
        tP.N = 2;
        RunTest(TwoParticles, tP);
    }

    // Test methods
    private static void TwoParticles(TestParams tP)
    {
        // Initial positions and velocities:
        float[] positions = [-1, 0, 0, 1, 0, 0];
        float[] velocities = [0, 0, 1, 0, -2, 0];

        float[] positionsOut = GetShaderOutput(tP, positions, velocities);
        float[] positionsOutExpected = GetExpectedOutput(positions, velocities);
        CollectionAssert.AreEqual(positionsOut, positionsOutExpected);
    }
    // Computation method:
    private static float[] GetShaderOutput(TestParams tP, float[] positions, float[] velocities)
    {
        if (tP.bufferHandler == null) { throw new ArgumentException("Buffer handler can not be zero while testing."); }
        if (tP.shader == null) { throw new ArgumentException("Shader can not be zero while testing."); }

        int N = positions.Length / 3;

        // Create shader buffers and run simulation:
        tP.bufferHandler.CreateStorageBuffer(GameEngine.Buffer.positionsCurrent, positions, BufferUsageHint.StreamDraw);
        tP.bufferHandler.CreateStorageBuffer(GameEngine.Buffer.positionsFuture, positions, BufferUsageHint.StreamDraw);
        tP.bufferHandler.CreateStorageBuffer(GameEngine.Buffer.velocitiesCurrent, velocities, BufferUsageHint.StreamDraw);

        tP.shader.bufferLocations.Add(0, GameEngine.Buffer.positionsCurrent);
        tP.shader.bufferLocations.Add(1, GameEngine.Buffer.positionsFuture);
        tP.shader.bufferLocations.Add(2, GameEngine.Buffer.velocitiesCurrent);

        tP.shader.SetInt("offSetX", 0);
        tP.shader.SetFloat("deltaTime", 1);

        tP.shader.Dispatch1D(N);

        // Check results:
        float[] positionsOut = tP.bufferHandler.GetBufferData(GameEngine.Buffer.positionsFuture, 3 * N);

        return positionsOut;
    }
    private static float[] GetExpectedOutput(float[] positions, float[] velocities)
    {
        int N = positions.Length / 3;
        float[] positionsOut = new float[3 * N];
        for (int i = 0; i < 3 * N; i++)
        {
            positionsOut[i] = positions[i] + velocities[i];
        }
        return positionsOut;
    }
}