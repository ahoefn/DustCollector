using DustCollector.GameEngine.Shaders;
using OpenTK.Graphics.OpenGL4;
namespace DustCollector.GameEngine;


class ParticleModel : IBufferHandler
{
    public ParticleModel(int particleCount_in, string positionPath, string velocityPath, string forcePath, BufferHandler bufferHandler_in)
    {
        particleCount = particleCount_in;
        //Create compute shaders:
        _positionUpdater = new ComputeShader(positionPath, bufferHandler_in);
        string preAmble = $"#define PARTICLECOUNT {particleCount}\n";
        _velocityUpdater = new ComputeShader(velocityPath, preAmble, bufferHandler_in);
        _forceUpdater = new ComputeShader(forcePath, bufferHandler_in);
        _bufferHandler = bufferHandler_in;
    }
    //Properties:
    public readonly int particleCount;
    private readonly ComputeShader _positionUpdater;
    private readonly ComputeShader _velocityUpdater;
    private readonly ComputeShader _forceUpdater;
    private readonly BufferHandler _bufferHandler;

    //Methods:
    public void Simulate(float deltaTime)
    {
        // Update positions:
        _positionUpdater.SetFloat("deltaTime", deltaTime);
        _positionUpdater.Dispatch1D(particleCount);

        // Update velocities:
        _velocityUpdater.SetFloat("deltaTime", deltaTime);
        _velocityUpdater.Dispatch1D(particleCount);

        // Update forces:
        _forceUpdater.Dispatch1D(particleCount * (particleCount - 1) / 2);
    }
    public void InitializeShaders()
    {
        if (particleCount == 0) { Console.WriteLine("Warning, particle count is zero when initializing shader."); }

        // Add buffers:
        _positionUpdater.bufferLocations.Add(0, Buffer.positionsCurrent);
        _positionUpdater.bufferLocations.Add(1, Buffer.positionsFuture);
        _positionUpdater.bufferLocations.Add(2, Buffer.velocitiesCurrent);

        _velocityUpdater.bufferLocations.Add(0, Buffer.velocitiesCurrent);
        _velocityUpdater.bufferLocations.Add(1, Buffer.velocitiesFuture);
        _velocityUpdater.bufferLocations.Add(2, Buffer.forcesCurrent);

        _forceUpdater.bufferLocations.Add(0, Buffer.positionsCurrent);
        _forceUpdater.bufferLocations.Add(1, Buffer.forcesFuture);

        // Set particle count in shaders:
        _forceUpdater.SetInt("particleCount", particleCount);
    }

    //Generates cube of dimensions^3 particles.
    public float[] GeneratePositions(int dimensions)
    {
        if (particleCount != dimensions * dimensions * dimensions)
        {
            throw new ArgumentException("Dimensions does not match particleCount.", nameof(dimensions));
        }
        int arraySize = Globals.LOCALS_SIZE_X * (int)Math.Ceiling((float)particleCount / Globals.LOCALS_SIZE_X);
        var particles = new float[3 * arraySize];
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                for (int k = 0; k < dimensions; k++)
                {
                    int index = 3 * (i + dimensions * j + dimensions * dimensions * k);
                    particles[index] = i - dimensions / 2;
                    particles[index + 1] = j - dimensions / 2;
                    particles[index + 2] = -k - dimensions / 4;
                }
            }
        }
        return particles;
    }

    //Right now does nothing but can be used to initialize particles at certain celocities.
    public float[] GenerateVelocities(int dimensions)
    {
        if (particleCount != dimensions * dimensions * dimensions)
        {
            throw new ArgumentException("Dimensions does not match particleCount.", nameof(dimensions));
        }

        int arraySize = Globals.LOCALS_SIZE_X * (int)Math.Ceiling((float)particleCount / Globals.LOCALS_SIZE_X);
        var velocities = new float[3 * arraySize];
        int currentIndex;
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                for (int k = 0; k < dimensions; k++)
                {
                    currentIndex = 3 * (i + dimensions * j + dimensions * dimensions * k);
                    //Uncomment/write below if a certain initial velocity is wanted.
                    // velocities[currentIndex] = 1.1f;
                }
            }
        }
        return velocities;
    }

    //Generate Force array for use in bufer, intialized to zero for now. Also makes sure the output array is a multiple of 64*3
    public float[] GenerateForces()
    {
        int matrixSize = 3 * Globals.LOCALS_SIZE_X * (int)Math.Ceiling((float)particleCount / Globals.LOCALS_SIZE_X);
        var forces = new float[matrixSize * matrixSize];
        return forces;
    }

    //Generates colors in cube such that they for a nice gradient:
    public float[] GenerateColors(int dimensions)
    {
        if (particleCount != dimensions * dimensions * dimensions)
        {
            throw new ArgumentException("Dimensions does not match particleCount.", nameof(dimensions));
        }
        var colors = new float[3 * particleCount];
        int currentIndex;
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                for (int k = 0; k < dimensions; k++)
                {
                    currentIndex = 3 * (i + dimensions * j + dimensions * dimensions * k);
                    colors[currentIndex] = (float)i / dimensions;
                    colors[currentIndex + 1] = (float)j / dimensions;
                    colors[currentIndex + 2] = (float)(dimensions - i - j) / dimensions;
                }
            }
        }
        return colors;
    }

    // public void SwapPositionBuffers()
    // {
    //     _positionUpdater.SwapBuffers("positionsCurrent", "positionsFuture");
    //     _velocityUpdater.UpdateBuffer("positionsCurrent", _positionUpdater.buffers["positionsCurrent"], 0);
    // }
    // BufferHandler interface:
    public void CreateVertexBuffer(Buffer buffer, float[] data, BufferUsageHint hint)
    {
        _bufferHandler.CreateVertexBuffer(buffer, data, hint);
    }

    public void CreateStorageBuffer(Buffer buffer, float[] data, BufferUsageHint hint)
    {
        _bufferHandler.CreateStorageBuffer(buffer, data, hint);
    }
    public void SwapBuffers(Buffer buffer1, Buffer buffer2)
    {
        _bufferHandler.SwapBuffers(buffer1, buffer2);
    }
    public int GetBufferHandle(Buffer buffer)
    {
        return _bufferHandler.GetBufferHandle(buffer);
    }
    public void AddBuffer(Buffer buffer, int bufferInt)
    {
        _bufferHandler.AddBuffer(buffer, bufferInt);
    }
    public void RemoveBuffer(Buffer buffer)
    {
        _bufferHandler.RemoveBuffer(buffer);
    }



}
