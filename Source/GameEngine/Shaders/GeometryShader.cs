using OpenTK.Graphics.OpenGL4;
namespace DustCollector.GameEngine.Shaders;
public class GeometryShader : Shader
{
    public GeometryShader(string vertexPath, string fragmentPath, IBufferHandler bufferHandler)
    : base(bufferHandler)
    {
        // Compile shaders and link to program:
        int vertexShader = CompileShader(vertexPath, ShaderType.VertexShader);
        int fragmentShader = CompileShader(fragmentPath, ShaderType.FragmentShader);

        handle = GL.CreateProgram();
        GL.AttachShader(handle, vertexShader);
        GL.AttachShader(handle, fragmentShader);
        GL.LinkProgram(handle);

        // Check succes:
        GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int succes);
        if (succes == 0)
        {
            string infoLog = GL.GetProgramInfoLog(handle);
            throw new ArgumentException("Could not create geometry shader, check shader code. InfoLog: " + infoLog, nameof(fragmentPath) + " " + nameof(vertexPath));
        }

        // Cleanup shaders as we won't need them from now on:
        GL.DetachShader(handle, vertexShader);
        GL.DetachShader(handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        // Now create uniform dictionary:
        UpdateUniforms();

        _vertexArray = GL.GenVertexArray();
    }
    // Properties:
    private readonly int _vertexArray;

    // Methods:
    public void Render(int particleCount, Camera camera)
    {
        Use();

        // Update uniform matrices:
        SetMatrix4("model", camera.model);
        SetMatrix4("view", camera.view);
        SetMatrix4("projection", camera.projection);
        // Draw:
        GL.BindVertexArray(_vertexArray);
        GL.DrawArrays(PrimitiveType.Points, 0, particleCount);
    }

    // Binds the vertex buffer object to the vertex array object:
    public void BindBufferToArray(Buffer buffer, int location, int stride)
    {
        Use();
        int bufferHandle = _bufferHandler.GetBufferHandle(buffer);
        GL.BindBuffer(BufferTarget.ArrayBuffer, bufferHandle);
        GL.BindVertexArray(_vertexArray);
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
}
