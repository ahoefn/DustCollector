using OpenTK.Graphics.OpenGL4;
namespace DustCollector.Renderer;
class ParticleModel
{
    public ParticleModel(string positionLocation, string velocityLocation)
    {
        _positionUpdater = new ComputeShader(positionLocation);
        _velocityUpdater = new ComputeShader(velocityLocation);
    }
    //Properties:
    public int particleCount = 0;
    private ComputeShader _positionUpdater;
    private ComputeShader _velocityUpdater;

    //Methods:
    public void Simulate(float deltaTime)
    {
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
        _velocityUpdater.Use();
        _velocityUpdater.SetFloat("deltaTime", deltaTime);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _velocityUpdater.buffers["positionsCurrent"]);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, _velocityUpdater.buffers["velocities"]);
        _velocityUpdater.Dispatch(particleCount * (particleCount - 1) / 2, 1, 1);


        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
        _positionUpdater.Use();
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _velocityUpdater.buffers["positionsCurrent"]);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, _positionUpdater.buffers["positionsFuture"]);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 2, _positionUpdater.buffers["velocities"]);
        _positionUpdater.SetFloat("deltaTime", deltaTime);
        _positionUpdater.Dispatch(particleCount, 1, 1);
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
        _positionUpdater.ShareBuffer("positionsCurrent", positionsCurrent, 0);
        _positionUpdater.ShareBuffer("positionsFuture", positionsFuture, 1);
        _positionUpdater.CreateStorageBuffer("velocities", velocities, 2, BufferUsageHint.StreamDraw);

        _velocityUpdater.ShareBuffer("positionsCurrent", positionsCurrent, 0);
        _velocityUpdater.ShareBuffer("velocities", _positionUpdater.buffers["velocities"], 1);
    }
    public void SwapPositionBuffers()
    {
        _positionUpdater.SwapPositionBuffers();
        _velocityUpdater.UpdateBuffer("positionsCurrent", _positionUpdater.buffers["positionsCurrent"], 0);
    }
}
