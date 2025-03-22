using DustCollector.GameEngine.Shaders;
using OpenTK.Graphics.OpenGL4;
namespace DustCollector.GameEngine;


class ParticleModel : IDisposable
{
    public ParticleModel(int particleCount_in, string positionPath, string velocityPath, string forcePath, IBufferHandler bufferHandler_in)
    {
        particleCount = particleCount_in;
        _bufferHandler = bufferHandler_in;

        // Create compute shaders:
        _positionUpdater = new ComputeShader(positionPath, _bufferHandler);
        string preAmble = $"#define PARTICLECOUNT {particleCount}\n";
        _velocityUpdater = new ComputeShader(velocityPath, preAmble, _bufferHandler);
        _forceUpdater = new ComputeShader(forcePath, _bufferHandler);
    }
    // Properties:
    public readonly int particleCount;
    private readonly ComputeShader _positionUpdater;
    private readonly ComputeShader _velocityUpdater;
    private readonly ComputeShader _forceUpdater;
    private readonly IBufferHandler _bufferHandler;

    // Methods:
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

        // Force shader uniforms:
        _forceUpdater.SetInt("particleCount", particleCount);
        _forceUpdater.SetFloat("gravityStrength", Settings.GRAVITYSTRENGTH);
        if (Settings.COLLISSIONS)
        {
            _forceUpdater.SetFloat("colissionsStrength", Settings.COLLISSIONSTRENGTH);
        }

    }




    // Generates cube of dimensions^3 particles.
    public float[] GeneratePositions(int dimensions)
    {
        if (particleCount != dimensions * dimensions * dimensions)
        {
            throw new ArgumentException("Dimensions does not match particleCount.", nameof(dimensions));
        }
        int arraySize = Globals.LOCAL_SIZE_X * (int)Math.Ceiling((float)particleCount / Globals.LOCAL_SIZE_X);
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

    // Right now does nothing but can be used to initialize particles at certain celocities.
    public float[] GenerateVelocities(int dimensions)
    {
        if (particleCount != dimensions * dimensions * dimensions)
        {
            throw new ArgumentException("Dimensions does not match particleCount.", nameof(dimensions));
        }

        int arraySize = Globals.LOCAL_SIZE_X * (int)Math.Ceiling((float)particleCount / Globals.LOCAL_SIZE_X);
        var velocities = new float[3 * arraySize];
        int currentIndex;
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                for (int k = 0; k < dimensions; k++)
                {
                    currentIndex = 3 * (i + dimensions * j + dimensions * dimensions * k);
                    // Uncomment/write below if a certain initial velocity is wanted.
                    // velocities[currentIndex] = 1.1f;
                }
            }
        }
        return velocities;
    }

    // Generate Force array for use in bufer, intialized to zero for now. 
    // Also makes sure the output array is a multiple of 64*3
    public float[] GenerateForces()
    {
        int matrixSize = 3 * Globals.LOCAL_SIZE_X * (int)Math.Ceiling((float)particleCount / Globals.LOCAL_SIZE_X);
        var forces = new float[matrixSize * matrixSize];
        return forces;
    }

    // Generates colors in cube such that they for a nice gradient:
    public float[] GenerateColors(int dimensions)
    {
        if (particleCount != dimensions * dimensions * dimensions)
        {
            throw new ArgumentException("Dimensions does not match particleCount.", nameof(dimensions));
        }

        float luminosity = Settings.LUMINOSITY; // Determines the base luminosity.
        var colors = new float[3 * particleCount];
        int currentIndex;
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                for (int k = 0; k < dimensions; k++)
                {
                    currentIndex = 3 * (i + dimensions * j + dimensions * dimensions * k);
                    colors[currentIndex] = luminosity + (1 - luminosity) * i / dimensions;
                    colors[currentIndex + 1] = luminosity + (1 - luminosity) * j / dimensions;
                    colors[currentIndex + 2] = luminosity + (1 - luminosity) * (dimensions - i - j) / dimensions;
                }
            }
        }
        return colors;
    }

    // IDisposable methods:
    public void Dispose()
    {
        _forceUpdater.Dispose();
        _positionUpdater.Dispose();
        _velocityUpdater.Dispose();
        GC.SuppressFinalize(this);
    }
}
