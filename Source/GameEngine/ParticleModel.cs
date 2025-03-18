using DustCollector.GameEngine.Shaders;
using OpenTK.Graphics.OpenGL4;
namespace DustCollector.GameEngine;


class ParticleModel : IBufferHandler
{
    public ParticleModel(string positionPath, string velocityPath, BufferHandler bufferHandler_in)
    {
        //Create compute shaders:
        _positionUpdater = new Shaders.ComputeShader(positionPath, bufferHandler_in);
        _velocityUpdater = new Shaders.ComputeShader(velocityPath, bufferHandler_in);
        _bufferHandler = bufferHandler_in;
    }
    //Properties:
    public int particleCount = 0;
    private readonly ComputeShader _positionUpdater;
    private readonly ComputeShader _velocityUpdater;
    private readonly BufferHandler _bufferHandler;

    //Methods:
    public void Simulate(float deltaTime)
    {
        //Update velocities:
        // GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
        // _velocityUpdater.SetFloat("deltaTime", deltaTime);
        // _velocityUpdater.Dispatch1D(particleCount * (particleCount - 1) / 2);

        //Update positions:
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
        _positionUpdater.SetFloat("deltaTime", deltaTime);
        _positionUpdater.Dispatch1D(particleCount);
    }
    public void InitializeShaders()
    {
        _positionUpdater.bufferLocations.Add(0, Buffer.positionsCurrent);
        _positionUpdater.bufferLocations.Add(1, Buffer.positionsFuture);
        _positionUpdater.bufferLocations.Add(2, Buffer.velocitiesCurrent);

        _velocityUpdater.bufferLocations.Add(0, Buffer.positionsCurrent);
        _velocityUpdater.bufferLocations.Add(1, Buffer.velocitiesCurrent);
        _velocityUpdater.bufferLocations.Add(2, Buffer.velocitiesCurrent);
    }
    //Generates cube of dimensions^3 particles.
    public float[] GeneratePositions(int dimensions)
    {

        if (particleCount != dimensions * dimensions * dimensions)
        {
            Console.WriteLine("dimensions does not match particleCount\n");
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
            Console.WriteLine("dimensions does not match particleCount\n");
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
                    velocities[currentIndex] = 1.1f;
                }
            }
        }
        return velocities;
    }

    //Generates colors in cube such that they for a nice gradient:
    public float[] GenerateColors(int dimensions)
    {
        if (particleCount != dimensions * dimensions * dimensions)
        {
            Console.WriteLine("dimensions does not match particleCount\n");
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
