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
        var forcesUpdater = new ComputeShader(Paths.FORCEUPDATERPATH, bufferHandler);
        Assert.IsNotNull(forcesUpdater);

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

        forcesUpdater.bufferLocations.Add(0, GameEngine.Buffer.positionsCurrent);
        forcesUpdater.bufferLocations.Add(1, GameEngine.Buffer.forcesFuture);

        forcesUpdater.SetInt("offSetX", 0);
        forcesUpdater.SetInt("particleCount", N);

        forcesUpdater.Dispatch1D(N * (N - 1) / 2);

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
        var forcesUpdater = new ComputeShader(Paths.FORCEUPDATERPATH, bufferHandler);
        Assert.IsNotNull(forcesUpdater);

        //Initial positions and velocities:
        //                   |   P1  |    P2  |    P3   |   P4     |
        float[] positions = [1, 1, 0, -1, 1, 0, 1, -1, 0, -1, -1, 0];
        float[] forcesIn = new float[3 * N * (N - 1)];
        float[] forcesOutGoal = new float[3 * N * (N - 1)];

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

        //Create shader buffers and run simulation:
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.positionsCurrent, positions, BufferUsageHint.StreamDraw);
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.forcesFuture, forcesIn, BufferUsageHint.StreamDraw);

        forcesUpdater.bufferLocations.Add(0, GameEngine.Buffer.positionsCurrent);
        forcesUpdater.bufferLocations.Add(1, GameEngine.Buffer.forcesFuture);

        forcesUpdater.SetInt("offSetX", 0);
        forcesUpdater.SetInt("particleCount", N);

        forcesUpdater.Dispatch1D(N * (N - 1) / 2);

        //Check results:
        float[] forcesOut = bufferHandler.GetBufferData(GameEngine.Buffer.forcesFuture, 3 * N * (N - 1));
        CollectionAssert.AreEqual(forcesOut, forcesOutGoal);
    }
    public static void NParticlesRand(TestParams testParams)
    {
        //Make sure GL context is correct and compile shader:
        testParams.window.MakeCurrent();
        GL.UseProgram(testParams.program);
        var bufferHandler = new BufferHandler();

        if (testParams.N == null) { throw new ArgumentException("N must not be null.", nameof(testParams.N)); }
        int N = (int)testParams.N;
        var forcesUpdater = new ComputeShader(Paths.FORCEUPDATERPATH, bufferHandler);
        Assert.IsNotNull(forcesUpdater);

        //Initial positions and velocities:
        //                   |   P1  |    P2  |    P3   |   P4     |
        float[] positions = new float[3 * N];
        float[] forcesIn = new float[3 * N * (N - 1)];
        float[] forcesOutGoal = new float[3 * N * (N - 1)];
        var random = new Random();
        for (int i = 0; i < N; i++)
        {
            positions[i] = random.NextSingle();
        }

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

        //Create shader buffers and run simulation:
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.positionsCurrent, positions, BufferUsageHint.StreamDraw);
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.forcesFuture, forcesIn, BufferUsageHint.StreamDraw);

        forcesUpdater.bufferLocations.Add(0, GameEngine.Buffer.positionsCurrent);
        forcesUpdater.bufferLocations.Add(1, GameEngine.Buffer.forcesFuture);

        forcesUpdater.SetInt("offSetX", 0);
        forcesUpdater.SetInt("particleCount", N);

        forcesUpdater.Dispatch1D(N * (N - 1) / 2);

        //Check results:
        float[] forcesOut = bufferHandler.GetBufferData(GameEngine.Buffer.forcesFuture, 3 * N * (N - 1));
        CollectionAssert.AreEqual(forcesOut, forcesOutGoal, new FloatComparer(0.001f));
    }
    public static void TotalForceTester(TestParams testParams)
    {
        int N = 4;
        // Initial positions and velocities:
        //                   |   P1  |    P2  |    P3   |   P4     |
        float[] positions = [1, 1, 0, -1, 1, 0, 1, -1, 0, -1, -1, 0];
        float[] forcesIn = new float[3 * N * (N - 1)];

        // Compare total forces:
        float[] expectedTotalForce = TotalForceCalculatorDirect(positions);
        float[] shaderOutput = GetShaderOutput(positions, forcesIn, testParams);
        float[] actualTotalForce = TotalForceCalculatorFromShader(shaderOutput, N);
        CollectionAssert.AreEqual(actualTotalForce, expectedTotalForce);
    }
    public static void TotalForceTesterNpartRand(TestParams testParams)
    {
        if (testParams.N == null)
        {
            throw new ArgumentNullException(nameof(testParams.N), "testParams.N can not be null.");
        }

        int N = (int)testParams.N;
        // Initial positions and velocities:
        //                   |   P1  |    P2  |    P3   |   P4     |
        float[] positions = TestProgram.GenerateRandomArray(3 * N);
        float[] forcesIn = new float[3 * N * (N - 1)];

        // Compare total forces:
        float[] expectedTotalForce = TotalForceCalculatorDirect(positions);
        float[] shaderOutput = GetShaderOutput(positions, forcesIn, testParams);
        float[] actualTotalForce = TotalForceCalculatorFromShader(shaderOutput, N);
        CollectionAssert.AreEqual(actualTotalForce, expectedTotalForce, new FloatComparer(0.1f));
    }    // General shader methods:
    private static float[] GetShaderOutput(float[] positions, float[] forcesIn, TestParams testParams)
    {
        //Make sure GL context is correct and compile shader:
        testParams.window.MakeCurrent();
        GL.UseProgram(testParams.program);
        var bufferHandler = new BufferHandler();
        var forcesUpdater = new ComputeShader(Paths.FORCEUPDATERPATH, bufferHandler);
        Assert.IsNotNull(forcesUpdater);

        //Create shader buffers and assign to correct locations:
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.positionsCurrent, positions, BufferUsageHint.StreamDraw);
        bufferHandler.CreateStorageBuffer(GameEngine.Buffer.forcesFuture, forcesIn, BufferUsageHint.StreamDraw);

        forcesUpdater.bufferLocations.Add(0, GameEngine.Buffer.positionsCurrent);
        forcesUpdater.bufferLocations.Add(1, GameEngine.Buffer.forcesFuture);

        // Set uniforms and dispatch shader
        int N = positions.Length / 3;
        forcesUpdater.SetInt("offSetX", 0);
        forcesUpdater.SetInt("particleCount", N);

        forcesUpdater.Dispatch1D(N * (N - 1) / 2);

        // Output results:
        float[] forcesOut = bufferHandler.GetBufferData(GameEngine.Buffer.forcesFuture, 3 * N * (N - 1));
        return forcesOut;
    }


    // Force calculation methods:
    private static Vector3 ForceVecCalculator(int particleIndex1, int particleIndex2, float[] positions)
    {
        Vector3 pos1 = new Vector3(positions[3 * particleIndex1], positions[3 * particleIndex1 + 1], positions[3 * particleIndex1 + 2]);
        Vector3 pos2 = new Vector3(positions[3 * particleIndex2], positions[3 * particleIndex2 + 1], positions[3 * particleIndex2 + 2]);
        Vector3 distance = pos2 - pos1;
        Vector3 force = Vector3.Normalize(distance) / Vector3.Dot(distance, distance);
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