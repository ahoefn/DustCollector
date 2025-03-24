using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using DustCollector.GameEngine;
using DustCollector.GameEngine.Shaders;
namespace DustCollector.Tests;

/// <summary>
/// Performs a simple test on the force compute shader. Creating an instance of this class runs the test.
/// </summary>
sealed class ForceTester : Tester
{
    public ForceTester(TestParams testParams) : base(Paths.FORCEUPDATERPATHNOCOLLISIONS)
    {
        testParams.N = 2;
        RunTest(TwoParticles, testParams);
        testParams.N = 4;
        RunTest(FourParticles, testParams);
        testParams.N = 800;
        RunTest(NParticlesRand, testParams);
        testParams.N = 4;
        RunTest(TotalForceTester4Part, testParams);
        testParams.N = 800;
        RunTest(TotalForceTesterNPartRand, testParams);
    }

    // Test methods:
    private static void TwoParticles(TestParams tP)
    {
        if (tP.N != 2) { throw new ArgumentException("testParams.N must be two in this test."); }
        //Initial positions and velocities:
        //                   |   P1  |    P2    |
        float[] positions = [0, 0, 1, 0, 0, -1];
        //                       Fx|Fy| Fz |
        float[] forcesOutGoal = [0, 0, -0.25f, // P1
                                 0, 0, 0.25f];// P2

        float[] forcesOut = GetShaderOutput(tP, positions);
        CollectionAssert.AreEqual(forcesOut, forcesOutGoal);
    }
    private static void FourParticles(TestParams tP)
    {
        if (tP.N != 4) { throw new ArgumentException("testParams.N must be four in this test."); }
        //Initial positions and velocities:
        //                   |   P1  |    P2  |    P3   |   P4     |
        float[] positions = [1, 1, 0, -1, 1, 0, 1, -1, 0, -1, -1, 0];
        float[] forcesOutGoal = GetExpectedOutput(positions);
        float[] forcesOut = GetShaderOutput(tP, positions);
        CollectionAssert.AreEqual(forcesOut, forcesOutGoal);
    }
    private static void NParticlesRand(TestParams tP)
    {
        if (tP.N == null) { throw new ArgumentException("N must not be null.", nameof(tP.N)); }
        int N = (int)tP.N;

        // Now a random N particle array:
        float[] positions = GenerateRandomArray(3 * N);
        float[] forcesOutGoal = GetExpectedOutput(positions);
        float[] forcesOut = GetShaderOutput(tP, positions);

        CollectionAssert.AreEqual(forcesOut, forcesOutGoal, new FloatComparer(0.001f));
    }
    private static void TotalForceTester4Part(TestParams tP)
    {
        if (tP.N != 4) { throw new ArgumentException("N must be four for this test."); }
        // Initial positions and velocities:
        //                   |   P1  |    P2  |    P3   |   P4     |
        float[] positions = [1, 1, 0, -1, 1, 0, 1, -1, 0, -1, -1, 0];

        // Compare total forces:
        float[] expectedTotalForce = TotalForceCalculatorDirect(positions);
        float[] forcesOut = GetShaderOutput(tP, positions);
        float[] actualTotalForce = TotalForceCalculatorFromShader(forcesOut, (int)tP.N);
        CollectionAssert.AreEqual(actualTotalForce, expectedTotalForce);
    }
    private static void TotalForceTesterNPartRand(TestParams tP)
    {
        if (tP.N == null)
        {
            throw new ArgumentNullException(nameof(tP.N), "testParams.N can not be null.");
        }

        int N = (int)tP.N;
        // Initial positions and velocities:
        //                   |   P1  |    P2  |    P3   |   P4     |
        float[] positions = GenerateRandomArray(3 * N);

        // Compare total forces:
        float[] expectedTotalForce = TotalForceCalculatorDirect(positions);
        float[] shaderOutput = GetShaderOutput(tP, positions);
        float[] actualTotalForce = TotalForceCalculatorFromShader(shaderOutput, N);
        CollectionAssert.AreEqual(actualTotalForce, expectedTotalForce, new FloatComparer(0.1f));
    }

    // Computation method:
    private static float[] GetShaderOutput(TestParams tP, float[] positions)
    {
        if (tP.bufferHandler == null) { throw new ArgumentException("Buffer handler can not be zero while testing."); }
        if (tP.shader == null) { throw new ArgumentException("Shader can not be zero while testing."); }

        int N = positions.Length / 3;
        var forcesOut = new float[N * (N - 1) / 2];

        //Create shader buffers and run simulation:
        tP.bufferHandler.CreateStorageBuffer(GameEngine.Buffer.positionsCurrent, positions, BufferUsageHint.StreamDraw);
        tP.bufferHandler.CreateStorageBuffer(GameEngine.Buffer.forcesFuture, forcesOut, BufferUsageHint.StreamDraw);

        tP.shader.bufferLocations.Add(0, GameEngine.Buffer.positionsCurrent);
        tP.shader.bufferLocations.Add(1, GameEngine.Buffer.forcesFuture);

        //Set uniforms and dispatch:
        tP.shader.SetInt("offSetX", 0);
        tP.shader.SetInt("particleCount", N);
        tP.shader.SetFloat("gravityStrength", Settings.GRAVITYSTRENGTH);

        tP.shader.Dispatch1D(N * (N - 1) / 2);

        //Check results:
        forcesOut = tP.bufferHandler.GetBufferData(GameEngine.Buffer.forcesFuture, 3 * N * (N - 1));
        return forcesOut;
    }
    // Expected output:
    private static float[] GetExpectedOutput(float[] positions)
    {
        int N = positions.Length / 3;
        var forcesOutGoal = new float[N * (N - 1) / 2];

        float force;
        int rowIndex;
        int dirIndex;
        int columnIndex;
        int particleColumnActual;
        for (int particleRow = 0; particleRow < N; particleRow++)
        {
            rowIndex = 3 * particleRow * (N - 1);
            for (int dir = 0; dir < 3; dir++)
            {
                dirIndex = (N - 1) * dir;
                for (int particleColumn = 0; particleColumn < N - 1; particleColumn++)
                {
                    columnIndex = particleColumn;
                    particleColumnActual = particleColumn + (particleColumn < particleRow ? 0 : 1);
                    force = ForceCalculator(particleRow, particleColumnActual, dir, positions);
                    forcesOutGoal[rowIndex + dirIndex + columnIndex] = force;
                }
            }
        }
        return forcesOutGoal;
    }
    // Force calculation methods:
    private static Vector3 ForceVecCalculator(int particleIndex1, int particleIndex2, float[] positions)
    {
        Vector3 pos1 = new Vector3(positions[3 * particleIndex1], positions[3 * particleIndex1 + 1], positions[3 * particleIndex1 + 2]);
        Vector3 pos2 = new Vector3(positions[3 * particleIndex2], positions[3 * particleIndex2 + 1], positions[3 * particleIndex2 + 2]);
        Vector3 distance = pos2 - pos1;
        Vector3 force = Settings.GRAVITYSTRENGTH * Vector3.Normalize(distance) / Vector3.Dot(distance, distance);
        return force;
    }
    private static float ForceCalculator(int particleIndex1, int particleIndex2, int dir, float[] positions)
    {
        Vector3 forceVec = ForceVecCalculator(particleIndex1, particleIndex2, positions);
        return forceVec[dir];
    }
    private static float[] TotalForceCalculatorDirect(float[] positions)
    {
        int N = positions.Length / 3;
        float[] totalForce = new float[3 * N];
        Vector3 force;
        for (int particleDest = 0; particleDest < N; particleDest++)
        {
            force = new Vector3(0, 0, 0);
            for (int particleSource = 0; particleSource < N; particleSource++)
            {
                if (!(particleSource == particleDest))
                {

                    force += ForceVecCalculator(particleDest, particleSource, positions);
                }
            }
            totalForce[3 * particleDest] = force[0];
            totalForce[3 * particleDest + 1] = force[1];
            totalForce[3 * particleDest + 2] = force[2];
        }
        return totalForce;
    }
    private static float[] TotalForceCalculatorFromShader(float[] forces, int N)
    {
        var totalForce = new float[3 * N];
        int totalForceIndex;
        int forcesIndex;
        for (int particleRow = 0; particleRow < N; particleRow++)
        {
            for (int dir = 0; dir < 3; dir++)
            {
                for (int particleColumn = 0; particleColumn < N - 1; particleColumn++)
                {
                    totalForceIndex = 3 * particleRow + dir;
                    forcesIndex = 3 * (N - 1) * particleRow + (N - 1) * dir + particleColumn;
                    totalForce[totalForceIndex] += forces[forcesIndex];
                }
            }
        }
        return totalForce;
    }
}