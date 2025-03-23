using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace DustCollector.GameEngine;

/// <summary>
/// Class that handles the communication between the Game/GameWindow and the actual OpenGL shaders and buffers that deal with simulation and rendering. In particular, contains a GeometricShader and a particle model for these tasks.
/// </summary>
public class GameEngine : ICamera, IDisposable
{
    public GameEngine(int width, int height)
    {
        //Global GL Settings, should not be changed anywhere else.
        GL.PointSize(Settings.POINTSIZE);
        GL.ClearColor(0.02f, 0.01f, 0.10f, 1.0f);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.Blend);
        GL.Enable(EnableCap.ProgramPointSize);

        // Initializations:
        _camera = new Camera(width, height);
        _bufferHandler = new BufferHandler();
        int particleCount = Settings.CUBESIZE * Settings.CUBESIZE * Settings.CUBESIZE;

        // Check if we want the physics engine with or without collission force:
        if (Settings.COLLISSIONS)
        {
            _model = new ParticleModel(particleCount, Paths.POSITIONUPDATERPATH, Paths.VELOCITYUPDATERPATH,
                                        Paths.FORCEUPDATERPATHWCOLLISIONS, _bufferHandler);

        }
        else
        {
            _model = new ParticleModel(particleCount, Paths.POSITIONUPDATERPATH, Paths.VELOCITYUPDATERPATH,
                                        Paths.FORCEUPDATERPATHNOCOLLISIONS, _bufferHandler);
        }

        _shader = new Shaders.GeometryShader(Paths.VERTEXPATH, Paths.FRAGMENTPATH, _bufferHandler);

        InitializeBuffers();
        InitializeShaders();
    }

    // Properties:
    private readonly Shaders.GeometryShader _shader;
    private readonly ParticleModel _model;
    private readonly BufferHandler _bufferHandler;
    private readonly Camera _camera;
    public bool isSimulating = false;

    // Methods: 
    private void InitializeBuffers()
    {
        // Generate initial data:
        float[] positions = _model.GeneratePositions(Settings.CUBESIZE);
        float[] colors = _model.GenerateColors(Settings.CUBESIZE);
        float[] velocities = _model.GenerateVelocities(Settings.CUBESIZE);
        float[] forces = _model.GenerateForces();

        // Create buffers:
        _bufferHandler.CreateStorageBuffer(Buffer.positionsCurrent, positions, BufferUsageHint.StreamDraw);
        _bufferHandler.CreateStorageBuffer(Buffer.positionsFuture, positions, BufferUsageHint.StreamDraw);

        _bufferHandler.CreateStorageBuffer(Buffer.velocitiesCurrent, velocities, BufferUsageHint.StreamDraw);
        _bufferHandler.CreateStorageBuffer(Buffer.velocitiesFuture, velocities, BufferUsageHint.StreamDraw);

        _bufferHandler.CreateStorageBuffer(Buffer.forcesCurrent, forces, BufferUsageHint.StreamDraw);
        _bufferHandler.CreateStorageBuffer(Buffer.forcesFuture, forces, BufferUsageHint.StreamDraw);

        _bufferHandler.CreateStorageBuffer(Buffer.colors, colors, BufferUsageHint.StaticRead);
    }

    private void InitializeShaders()
    {
        // Geometry shader:
        // Set globals:
        _shader.SetFloat("POINTSIZE", Settings.POINTSIZE);

        // Create VertexArray:
        _shader.BindBufferToArray(Buffer.positionsCurrent, 0, 3);
        _shader.BindBufferToArray(Buffer.colors, 1, 3);

        // Compute shaders:
        _model.InitializeShaders();
    }
    //Render methods:
    public void Render(float deltaTime)
    {
        if (isSimulating)
        {
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
            //Swap render and simulation buffers
            _bufferHandler.SwapBuffers(Buffer.positionsCurrent, Buffer.positionsFuture);
            _bufferHandler.SwapBuffers(Buffer.velocitiesCurrent, Buffer.velocitiesFuture);
            _bufferHandler.SwapBuffers(Buffer.forcesCurrent, Buffer.forcesFuture);

            // Need to rebind the buffers to the vertex Array after swapping:
            _shader.BindBufferToArray(Buffer.positionsCurrent, 0, 3);
            _shader.BindBufferToArray(Buffer.colors, 1, 3);

            // Simulate next step
            _model.Simulate(deltaTime);
        }
        //Update camera to current view and start rendering
        _shader.Render(_model.particleCount, _camera);


    }


    //Camera methods:
    public void UpdateAspect(int width, int height)
    {
        _camera.UpdateAspect(width, height);
    }
    public void ChangeOrientation(Vector2 delta)
    {
        _camera.ChangeOrientation(delta);
    }
    public void ChangePosition(Direction dir, float amount)
    {
        _camera.ChangePosition(dir, amount);
    }

    public void Dispose()
    {
        _shader.Dispose();
        _model.Dispose();
        _bufferHandler.Dispose();
        GC.SuppressFinalize(this);
    }
}