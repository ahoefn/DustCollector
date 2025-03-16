using OpenTK.Graphics.OpenGL4;
namespace DustCollector.GameEngine.Shaders;
public class GeometryShader : Shader
{
    public GeometryShader(string vertexPath, string fragmentPath, BufferHandler bufferHandler)
    : base(BufferTarget.ArrayBuffer, bufferHandler)
    {
        //Compile shaders and link to program:
        int vertexShader = CompileShader(vertexPath, ShaderType.VertexShader);
        int fragmentShader = CompileShader(fragmentPath, ShaderType.FragmentShader);

        handle = GL.CreateProgram();
        GL.AttachShader(handle, vertexShader);
        GL.AttachShader(handle, fragmentShader);

        GL.LinkProgram(handle);

        //Check succes:
        GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int succes);
        if (succes == 0)
        {
            string infoLog = GL.GetProgramInfoLog(handle);
            throw new ArgumentException("Could not create geometry shader, check shader code. InfoLog: " + infoLog, nameof(fragmentPath) + " " + nameof(vertexPath));
        }

        //Cleanup shaders as we won't need them from now on:
        GL.DetachShader(handle, vertexShader);
        GL.DetachShader(handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        //Now create uniform dictionary:
        UpdateUniforms();

        vertexArrays = new Dictionary<string, int>();
    }
    //Data:
    public Dictionary<string, int> vertexArrays;

    //Methods:
    public void Render(int particleCount, Camera camera, int vertexArray)
    {
        Use();

        //Update uniform matrices:
        SetMatrix4("model", camera.model);
        SetMatrix4("view", camera.view);
        SetMatrix4("projection", camera.projection);

        //Draw:
        GL.BindVertexArray(vertexArray);
        GL.DrawArrays(PrimitiveType.Points, 0, particleCount);
    }
    public void BindBufferToArray(string buffer, string array, int location, int stride)
    {
        Use();
        GL.BindBuffer(BufferTarget.ArrayBuffer, buffers[buffer]);
        GL.BindVertexArray(vertexArrays[array]);
        GL.VertexAttribPointer(
            location,
            stride,
            VertexAttribPointerType.Float,
            false,
            stride * sizeof(float),
            0
        );
        GL.EnableVertexAttribArray(location);
    }
    public void CreateVertexArray(string name)
    {
        int vertexArrayObject = GL.GenVertexArray();
        vertexArrays.Add(name, vertexArrayObject);
    }
}
