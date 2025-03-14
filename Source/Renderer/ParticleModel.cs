using OpenTK.Graphics.OpenGL4;
namespace DustCollector.Renderer;
class ParticleModel
{
    public ParticleModel(string positionLocation, string velocityLocation)
    {
        //Create compute shaders:
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
        //Update velocities:
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
        _velocityUpdater.Use();
        _velocityUpdater.SetFloat("deltaTime", deltaTime);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _velocityUpdater.buffers["positionsCurrent"]);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, _velocityUpdater.buffers["velocities"]);
        _velocityUpdater.Dispatch(particleCount * (particleCount - 1) / 2, 1, 1);

        //Update positions:
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
        _positionUpdater.Use();
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _velocityUpdater.buffers["positionsCurrent"]);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, _positionUpdater.buffers["positionsFuture"]);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 2, _positionUpdater.buffers["velocities"]);
        _positionUpdater.SetFloat("deltaTime", deltaTime);
        _positionUpdater.Dispatch(particleCount, 1, 1);
    }

    //Generates cube of dimensions^3 particles.
    public float[] GeneratePositions(int dimensions)
    {

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

    //Right now does nothing but can be used to initialize particles at certain celocities.
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
                    //Uncomment/write below if a certain initial velocity is wanted.
                    // velocities[currentIndex] = 1.1f;
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
    public void InitializeBuffers(int positionsCurrent, int positionsFuture, float[] velocities)
    {
        //Initializebuffer for positionUpdater:
        _positionUpdater.ShareBuffer("positionsCurrent", positionsCurrent, 0);
        _positionUpdater.ShareBuffer("positionsFuture", positionsFuture, 1);
        _positionUpdater.CreateStorageBuffer("velocities", velocities, 2, BufferUsageHint.StreamDraw);

        //And now for velocityUpdater:
        _velocityUpdater.ShareBuffer("positionsCurrent", positionsCurrent, 0);
        _velocityUpdater.ShareBuffer("velocities", _positionUpdater.buffers["velocities"], 1);
    }
    public void SwapPositionBuffers()
    {
        _positionUpdater.SwapPositionBuffers();
        _velocityUpdater.UpdateBuffer("positionsCurrent", _positionUpdater.buffers["positionsCurrent"], 0);
    }
}
