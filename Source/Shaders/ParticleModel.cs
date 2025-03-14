using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace DustCollector;


class ParticleModel
{
    public ParticleModel(string positionLocation, string velocityLocation)
    {
        positionUpdater = new ComputeShader(positionLocation);
        velocityUpdater = new ComputeShader(velocityLocation);
    }
    //Properties:
    public int particleCount = 0;
    public ComputeShader positionUpdater;
    public ComputeShader velocityUpdater;

    //Methods:
    public void Simulate(float deltaTime)
    {
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
        velocityUpdater.Use();
        velocityUpdater.SetFloat("deltaTime", deltaTime);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, velocityUpdater.buffers["positionsCurrent"]);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, velocityUpdater.buffers["velocities"]);
        velocityUpdater.Dispatch(particleCount * (particleCount - 1) / 2, 1, 1);


        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
        positionUpdater.Use();
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, velocityUpdater.buffers["positionsCurrent"]);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, positionUpdater.buffers["positionsFuture"]);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 2, positionUpdater.buffers["velocities"]);
        positionUpdater.SetFloat("deltaTime", deltaTime);
        positionUpdater.Dispatch(particleCount, 1, 1);
    }



    public float[] GeneratePositions(int dimensions)
    {//Generates particles spaced out in a cube

        if (particleCount != dimensions * dimensions * dimensions)
        {
            Console.WriteLine("dimensions does not match particleCount\n");
        }

        var particles = new float[3 * particleCount];
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
    public Particles GenerateVertices(int dimensions)
    {

        if (particleCount != dimensions * dimensions * dimensions)
        {
            Console.WriteLine("dimensions does not match particleCount\n");
        }

        var particles = new Particles(particleCount);
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                for (int k = 0; k < dimensions; k++)
                {
                    int index = i + dimensions * j + dimensions * dimensions * k;
                    int spacing = 10;
                    particles.SetPosition(index, spacing * (i - dimensions / 2), spacing * (j - dimensions / 2), spacing * (k - dimensions / 2));
                    // particles.SetVelocity(index, (float)i / dimensions, (float)j / dimensions, (float)(dimensions - i - j) / dimensions);
                    particles.SetVelocity(index, 0, 0, 0);

                }
            }
        }
        return particles;
    }
    public float[] GenerateVelocities(int dimensions)
    {
        if (particleCount != dimensions * dimensions * dimensions)
        {
            Console.WriteLine("dimensions does not match particleCount\n");
        }
        var velocities = new float[3 * particleCount];
        int currentIndex;
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                for (int k = 0; k < dimensions; k++)
                {
                    currentIndex = 3 * (i + dimensions * j + dimensions * dimensions * k);
                    // velocities[currentIndex] = 1.1f;
                }
            }
        }
        return velocities;
    }
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
    public void InitializeBuffers(int positionsCurrent, int positionsFuture, float[] velocities)
    {
        positionUpdater.ShareBuffer("positionsCurrent", positionsCurrent, 0);
        positionUpdater.ShareBuffer("positionsFuture", positionsFuture, 1);
        positionUpdater.CreateStorageBuffer("velocities", velocities, 2, BufferUsageHint.StreamDraw);

        velocityUpdater.ShareBuffer("positionsCurrent", positionsCurrent, 0);
        velocityUpdater.ShareBuffer("velocities", positionUpdater.buffers["velocities"], 1);
    }
    public void SwapPositionBuffers()
    {
        positionUpdater.SwapPositionBuffers();
        velocityUpdater.UpdateBuffer("positionsCurrent", positionUpdater.buffers["positionsCurrent"], 0);
    }
}