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

        //Global GL Settings, should not be changed anywhere else.
        GL.PointSize(Globals.POINTSIZE);
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.Blend);
        GL.Enable(EnableCap.ProgramPointSize);

        CursorState = CursorState.Grabbed;
        _timer = new Stopwatch();
        _timer.Start();
        _camera = new Camera(Size.X, Size.Y);

        InitializeShaders();
    }
    private void InitializeShaders()
    {
        //Create shader and buffers:
        _model = new ParticleModel(Paths.POSITIONUPDATERPATH, Paths.VELOCITYUPDATERPATH);
        _shader = new GeometryShader(Paths.VERTEXPATH, Paths.FRAGMENTPATH);

        //Set globals:
        _shader.SetFloat("POINTSIZE", Globals.POINTSIZE);

        //Create initial data:
        int dimensions = 3;
        _model.particleCount = dimensions * dimensions * dimensions;
        float[] positions = _model.GeneratePositions(dimensions);
        float[] colors = _model.GenerateColors(dimensions);
        float[] velocities = _model.GenerateVelocities(dimensions);

        //Create buffers and vertex arrays:
        _shader.CreatePositionColorArrays(positions, colors);
        _model.InitializeBuffers(_shader.buffers["positionsCurrent"], _shader.buffers["positionsFuture"], velocities);
        _model.velocityUpdater.SetFloat("offSetX", 0);

    }
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        //Need to clear colorbuffer every render.
        GL.Clear(ClearBufferMask.ColorBufferBit);

        float deltaTime = (float)args.Time;

        if (_isSimulating)
        {
            _model.Simulate(deltaTime);
        }

        //Update camera to current view and start rendering
        _camera.view = Matrix4.LookAt(_camera.position, _camera.position + _camera.front, _camera.up);
        _shader.Render(_model.particleCount, _camera);

        //Swap render and simulation buffers
        SwapBuffers();
        _shader.SwapPositionBuffers();
        _model.SwapPositionBuffers();

        //Update framerate:
        _avgFrameRate = (_avgFrameRate * _frameCount + deltaTime) / (_frameCount + 1);
        _frameCount += 1;


    }
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        if (!IsFocused) { return; }

        base.OnUpdateFrame(args);

        float deltaTime = (float)args.Time;
        KeyboardInputHandler(deltaTime);
        MouseInputHandler();
    }
    private void KeyboardInputHandler(float deltaTime)
    {
        KeyboardState input = KeyboardState;

        if (input.IsKeyDown(Keys.Escape)) { Close(); }

        //Movement keys:
        if (input.IsKeyDown(Keys.W)) { _camera.position += _camera.front * Globals.MOVSPEED * deltaTime; }
        if (input.IsKeyDown(Keys.S)) { _camera.position -= _camera.front * Globals.MOVSPEED * deltaTime; }
        if (input.IsKeyDown(Keys.A)) { _camera.position -= _camera.right * Globals.MOVSPEED * deltaTime; }
        if (input.IsKeyDown(Keys.D)) { _camera.position += _camera.right * Globals.MOVSPEED * deltaTime; }
        if (input.IsKeyDown(Keys.LeftControl))
        {
            _camera.position -= _camera.up * Globals.MOVSPEED * deltaTime;
        }
        if (input.IsKeyDown(Keys.LeftShift))
        {
            _camera.position += _camera.up * Globals.MOVSPEED * deltaTime;
        }

        //Start/stop simulation and show framerate:
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
    }
    private void MouseInputHandler()
    {
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
    {//TODO: implement dispose for _model.
        base.OnUnload();
        _shader.Dispose();
    }
}