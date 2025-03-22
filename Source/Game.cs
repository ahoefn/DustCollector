using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
namespace DustCollector;
public class Game : GameWindow
{
    public Game(int width, int height, string title)
    : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title })
    {
        _timer = new Stopwatch();
        _timer.Start();
        _gameEngine = new GameEngine.Renderer(Size.X, Size.Y);
    }

    // If Game is called with debug : true, enable OpenGL debugging:
    public Game(int width, int height, string title, bool debug)
   : base(GameWindowSettings.Default, new NativeWindowSettings()
   {
       ClientSize = (width, height),
       Title = title,
       Flags = ContextFlags.Debug
   })
    {
        GL.DebugMessageCallback(Debugger.DebugMessageDelegate, IntPtr.Zero);
        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);

        _timer = new Stopwatch();
        _timer.Start();
        _gameEngine = new GameEngine.Renderer(Size.X, Size.Y);
    }

    // Properties:
    private GameEngine.Renderer _gameEngine;
    private Stopwatch _timer;
    private bool _firstMouse = true;
    private Vector2 _prevMousePos;
    private float _frameCount = 0;
    private float _avgFrameRate = 0;

    // Methods:
    protected override void OnLoad()
    {
        base.OnLoad();

        CursorState = CursorState.Grabbed;

    }
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        float deltaTime = (float)args.Time;
        _gameEngine.Render(deltaTime);

        //Update framerate:
        _avgFrameRate = (_avgFrameRate * _frameCount + deltaTime) / (_frameCount + 1);
        _frameCount += 1;

        //Need to swap GLFW window buffers:
        SwapBuffers();
        GL.Clear(ClearBufferMask.ColorBufferBit);
    }
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        if (!IsFocused) { return; }

        base.OnUpdateFrame(args);

        // Handle inputs:
        float deltaTime = (float)args.Time;
        KeyboardInputHandler(deltaTime);
        MouseInputHandler();
    }
    private void KeyboardInputHandler(float deltaTime)
    {
        KeyboardState input = KeyboardState;

        if (input.IsKeyDown(Keys.Escape)) { Close(); }

        //Movement keys:
        if (input.IsKeyDown(Keys.W))
        {
            _gameEngine.ChangePosition(GameEngine.Direction.front, Settings.MOVSPEED * deltaTime);
        }
        if (input.IsKeyDown(Keys.S))
        {
            _gameEngine.ChangePosition(GameEngine.Direction.front, -Settings.MOVSPEED * deltaTime);
        }
        if (input.IsKeyDown(Keys.A))
        {
            _gameEngine.ChangePosition(GameEngine.Direction.right, -Settings.MOVSPEED * deltaTime);
        }
        if (input.IsKeyDown(Keys.D))
        {
            _gameEngine.ChangePosition(GameEngine.Direction.right, Settings.MOVSPEED * deltaTime);
        }
        if (input.IsKeyDown(Keys.LeftControl))
        {
            _gameEngine.ChangePosition(GameEngine.Direction.up, -Settings.MOVSPEED * deltaTime);
        }
        if (input.IsKeyDown(Keys.LeftShift))
        {
            _gameEngine.ChangePosition(GameEngine.Direction.up, Settings.MOVSPEED * deltaTime);
        }

        //Start/stop simulation and show framerate:
        if (input.IsKeyPressed(Keys.Space))
        {
            if (_gameEngine.isSimulating)
            {
                Console.WriteLine("Framerate was: " + _avgFrameRate);
                _avgFrameRate = 0;
                _frameCount = 0;
            }
            _gameEngine.isSimulating = !_gameEngine.isSimulating;
        }

        //Restart renderer on R:
        if (input.IsKeyPressed(Keys.R))
        {
            RestartRenderer();
        }
    }
    private void MouseInputHandler()
    {
        // Need to initialize mouse position:
        if (_firstMouse)
        {
            _prevMousePos = new Vector2(MouseState.X, MouseState.Y);
            _firstMouse = false;
        }
        else
        {
            var deltaVec = new Vector2(MouseState.X - _prevMousePos.X, MouseState.Y - _prevMousePos.Y);
            _prevMousePos = MouseState.Position;
            _gameEngine.ChangeOrientation(deltaVec);
        }
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
        _gameEngine.UpdateAspect(e.Width, e.Height);
    }
    protected override void OnUnload()
    {
        base.OnUnload();
        _gameEngine.Dispose();
    }
    private void RestartRenderer()
    {
        _gameEngine.Dispose();
        _gameEngine = new GameEngine.Renderer(Size.X, Size.Y);
    }
}
