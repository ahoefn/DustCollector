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
    private Renderer.GameEngine _gameEngine;
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
        _gameEngine = new Renderer.GameEngine(Size.X, Size.Y);
    }
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        //Need to clear colorbuffer every render.
        GL.Clear(ClearBufferMask.ColorBufferBit);

        float deltaTime = (float)args.Time;

        if (_gameEngine.isSimulating)
        {
            _model.Simulate(deltaTime);
        }

        //Update camera to current view and start rendering
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
        if (input.IsKeyDown(Keys.W))
        {
            _gameEngine.ChangePosition(Direction.front, Globals.MOVSPEED * deltaTime);
        }
        if (input.IsKeyDown(Keys.S))
        {
            _gameEngine.ChangePosition(Direction.front, -Globals.MOVSPEED * deltaTime);
        }
        if (input.IsKeyDown(Keys.A))
        {
            _gameEngine.ChangePosition(Direction.right, -Globals.MOVSPEED * deltaTime);
        }
        if (input.IsKeyDown(Keys.D))
        {
            _gameEngine.ChangePosition(Direction.right, Globals.MOVSPEED * deltaTime);
        }
        if (input.IsKeyDown(Keys.LeftControl))
        {
            _gameEngine.ChangePosition(Direction.up, -Globals.MOVSPEED * deltaTime);
        }
        if (input.IsKeyDown(Keys.LeftShift))
        {
            _gameEngine.ChangePosition(Direction.up, Globals.MOVSPEED * deltaTime);
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
    {//TODO: implement dispose for _model.
        base.OnUnload();
        _gameEngine.Dispose();
    }
}
