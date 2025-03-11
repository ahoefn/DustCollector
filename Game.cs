using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using System.IO;
namespace DustCollector;


public class Game : GameWindow
{
    public Game(int width, int height, string title)
    : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title })
    { }
    //Properties:
    private GeometryShader _shader;
    private ParticleModel _model;
    private Stopwatch _timer;
    private Camera _camera;
    private bool _firstMouse = true;
    private Vector2 _prevMousePos;
    private float _frameCount = 0;
    private float _avgFrameRate = 0;
    private bool _isSimulating = false;
    //Methods:
    protected override void OnLoad()
    {
        base.OnLoad();

        CursorState = CursorState.Grabbed;

        _timer = new Stopwatch();
        _timer.Start();
        _camera = new Camera(Size.X, Size.Y);

        //Set GL settings
        GL.PointSize(Globals.POINTSIZE);
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.Blend);

        GL.Enable(EnableCap.ProgramPointSize);



        //Create shader and buffers:
        _model = new ParticleModel("Shaders/PositionUpdater.comp", "Shaders/VelocityUpdater.comp");
        _shader = new GeometryShader("Shaders/Shader.vert", "Shaders/Shader.frag");
        _shader.SetFloat("POINTSIZE", Globals.POINTSIZE);

        int dimensions = 5;
        _model.particleCount = dimensions * dimensions * dimensions;
        float[] positions = _model.GeneratePositions(dimensions);
        float[] colors = _model.GenerateColors(dimensions);
        float[] velocities = _model.GenerateVelocities(dimensions);

        _shader.CreatePositionColorArrays(positions, colors);
        _model.InitializeBuffers(_shader.buffers["positionsCurrent"], _shader.buffers["positionsFuture"], velocities);
        _model.velocityUpdater.SetFloat("offSetX", 0);
    }
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
        float deltaTime = (float)args.Time;
        _avgFrameRate = (_avgFrameRate * _frameCount + deltaTime) / (_frameCount + 1);
        _frameCount += 1;

        if (_isSimulating)
        {
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
            _model.velocityUpdater.Use();
            _model.velocityUpdater.SetFloat("deltaTime", deltaTime);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _model.velocityUpdater.buffers["positionsCurrent"]);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, _model.velocityUpdater.buffers["velocities"]);
            GL.DispatchCompute(_model.particleCount, _model.particleCount, 1);

            Console.WriteLine(GL.GetError().ToString());



            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
            _model.positionUpdater.Use();
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _model.velocityUpdater.buffers["positionsCurrent"]);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, _model.positionUpdater.buffers["positionsFuture"]);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 2, _model.positionUpdater.buffers["velocities"]);
            _model.positionUpdater.SetFloat("deltaTime", deltaTime);
            _model.positionUpdater.Dispatch(_model.particleCount, 1, 1);
            Console.WriteLine(GL.GetError().ToString());
        }

        _shader.Use();
        // _camera.model = Matrix4.CreateRotationX((float)timeValue / 5);
        _camera.view = Matrix4.LookAt(_camera.position, _camera.position + _camera.front, _camera.up);
        _shader.SetMatrix4("model", _camera.model);
        _shader.SetMatrix4("view", _camera.view);
        _shader.SetMatrix4("projection", _camera.projection);

        GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        GL.BindVertexArray(_shader.vertexArrays["positionsColorsCurrent"]);
        GL.DrawArrays(PrimitiveType.Points, 0, _model.particleCount);

        SwapBuffers();
        _shader.SwapPositionBuffers();
        _model.SwapPositionBuffers();

    }
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        if (!IsFocused) { return; }

        KeyboardState input = KeyboardState;
        base.OnUpdateFrame(args);

        //Keyboardinputs: 
        if (input.IsKeyDown(Keys.Escape)) { Close(); }
        float deltaTime = (float)args.Time;
        if (input.IsKeyDown(Keys.W)) { _camera.position += _camera.front * Globals.MOVSPEED * deltaTime; }
        if (input.IsKeyDown(Keys.S)) { _camera.position -= _camera.front * Globals.MOVSPEED * deltaTime; }
        if (input.IsKeyDown(Keys.A)) { _camera.position -= _camera.right * Globals.MOVSPEED * deltaTime; }
        if (input.IsKeyDown(Keys.D)) { _camera.position += _camera.right * Globals.MOVSPEED * deltaTime; }
        if (input.IsKeyDown(Keys.LeftControl)) { _camera.position -= _camera.up * Globals.MOVSPEED * deltaTime; }
        if (input.IsKeyDown(Keys.LeftShift)) { _camera.position += _camera.up * Globals.MOVSPEED * deltaTime; }
        if (input.IsKeyPressed(Keys.Space))
        {
            if (_isSimulating)
            {
                Console.WriteLine("Framerate was: " + _avgFrameRate);
                _avgFrameRate = 0;
                _frameCount = 0;
            }

            _isSimulating = !_isSimulating;

        }
        //Mouse Inputs:
        if (_firstMouse)
        {
            _prevMousePos = new Vector2(MouseState.X, MouseState.Y);
            _firstMouse = false;

        }
        else
        {
            var deltaVec = new Vector2(MouseState.X - _prevMousePos.X, MouseState.Y - _prevMousePos.Y);
            _prevMousePos = MouseState.Position;
            _camera.ChangeOrientation(deltaVec);
        }
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
        _camera.UpdateAspect(e.Width, e.Height);
    }
    protected override void OnUnload()
    {
        base.OnUnload();
        _shader.Dispose();
    }
}