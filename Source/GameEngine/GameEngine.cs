using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace DustCollector.Renderer;

interface IRenderer
{
    public void Render();

}
public class GameEngine : ICamera, IRenderer, IDisposable
{
    public GameEngine(int width, int height)
    {
        //Global GL Settings, should not be changed anywhere else.
        GL.PointSize(Globals.POINTSIZE);
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.Blend);
        GL.Enable(EnableCap.ProgramPointSize);

        _camera = new Camera(width, height);
        _model = new ParticleModel(Paths.POSITIONUPDATERPATH, Paths.VELOCITYUPDATERPATH);
        _shader = new GeometryShader(Paths.VERTEXPATH, Paths.FRAGMENTPATH);

        InitializeShaders();
    }
    private void InitializeShaders()
    {
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
    }
    private GeometryShader _shader;
    private ParticleModel _model;
    private Camera _camera;
    public bool isSimulating = false;

    //Render methods:
    public void Render()
    {

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
    }
}