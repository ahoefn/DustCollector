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
    }

    // Properties:
    private GameEngine.Renderer _Renderer;
    private Stopwatch _timer;
    private bool _firstMouse = true;
    private Vector2 _prevMousePos;
    private float _frameCount = 0;
    private float _avgFrameRate = 0;
    //Methods:
    protected override void OnLoad()
    {
        base.OnLoad();

        CursorState = CursorState.Grabbed;
        _timer = new Stopwatch();
        _timer.Start();
        _Renderer = new GameEngine.Renderer(Size.X, Size.Y);
    }
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        float deltaTime = (float)args.Time;

        _Renderer.Render(deltaTime);

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
            _Renderer.ChangePosition(GameEngine.Direction.front, Globals.MOVSPEED * deltaTime);
        }
        if (input.IsKeyDown(Keys.S))
        {
            _Renderer.ChangePosition(GameEngine.Direction.front, -Globals.MOVSPEED * deltaTime);
        }
        if (input.IsKeyDown(Keys.A))
        {
            _Renderer.ChangePosition(GameEngine.Direction.right, -Globals.MOVSPEED * deltaTime);
        }
        if (input.IsKeyDown(Keys.D))
        {
            _Renderer.ChangePosition(GameEngine.Direction.right, Globals.MOVSPEED * deltaTime);
        }
        if (input.IsKeyDown(Keys.LeftControl))
        {
            _Renderer.ChangePosition(GameEngine.Direction.up, -Globals.MOVSPEED * deltaTime);
        }
        if (input.IsKeyDown(Keys.LeftShift))
        {
            _Renderer.ChangePosition(GameEngine.Direction.up, Globals.MOVSPEED * deltaTime);
        }

        //Start/stop simulation and show framerate:
        if (input.IsKeyPressed(Keys.Space))
        {
            if (_Renderer.isSimulating)
            {
                Console.WriteLine("Framerate was: " + _avgFrameRate);
                _avgFrameRate = 0;
                _frameCount = 0;
            }
            _Renderer.isSimulating = !_Renderer.isSimulating;
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
            _Renderer.ChangeOrientation(deltaVec);
        }
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
        _Renderer.UpdateAspect(e.Width, e.Height);
    }
    protected override void OnUnload()
    {
        base.OnUnload();
        _Renderer.Dispose();
    }
}
