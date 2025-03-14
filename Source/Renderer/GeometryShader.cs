using OpenTK.Graphics.OpenGL4;
namespace DustCollector.Renderer;
public class GeometryShader : Shader
{
    public GeometryShader(string vertexPath, string fragmentPath) : base(BufferTarget.ArrayBuffer)
    {
        int vertexShader, fragmentShader;

        //Read shaders from text sources:
        string vertexShaderSource = File.ReadAllText(vertexPath);
        vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);

        string fragmentShaderSource = File.ReadAllText(fragmentPath);
        fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);

        //Compile text into actual shaders:
        GL.CompileShader(vertexShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int succes);
        if (succes == 0)
        {
            string infoLog = GL.GetShaderInfoLog(vertexShader);
            Console.WriteLine(infoLog + "No vertex Shader");
        }

        GL.CompileShader(fragmentShader);
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out succes);
        if (succes == 0)
        {
            string infoLog = GL.GetShaderInfoLog(fragmentShader);
            Console.WriteLine(infoLog + "No fragmentshader");
        }

        //Link two shaders together in a single program:
        handle = GL.CreateProgram();
        GL.AttachShader(handle, vertexShader);
        GL.AttachShader(handle, fragmentShader);

        GL.LinkProgram(handle);
        GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out succes);
        if (succes == 0)
        {
            string infoLog = GL.GetProgramInfoLog(handle);
            Console.WriteLine(infoLog + "No program");
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
    public void Render(int particleCount, Camera camera)
    {
        Use();
        SetMatrix4("model", camera.model);
        SetMatrix4("view", camera.view);
        SetMatrix4("projection", camera.projection);

        // GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        GL.BindVertexArray(vertexArrays["positionsColorsCurrent"]);
        GL.DrawArrays(PrimitiveType.Points, 0, particleCount);


    }
    public void CreatePositionColorArrays(float[] positions, float[] colors)
    {
        CreateVertexBuffer("positionsCurrent", positions, BufferUsageHint.StreamDraw);
        CreateVertexBuffer("positionsFuture", positions, BufferUsageHint.StreamDraw);
        CreateVertexBuffer("colors", colors, BufferUsageHint.StreamDraw);

        CreateVertexArray("positionsColorsCurrent");
        BindBufferToArray("positionsCurrent", "positionsColorsCurrent", 0, 3);
        BindBufferToArray("colors", "positionsColorsCurrent", 1, 3);

        CreateVertexArray("positionsColorsFuture");
        BindBufferToArray("positionsFuture", "positionsColorsFuture", 0, 3);
        BindBufferToArray("colors", "positionsColorsFuture", 1, 3);
    }
    public void SwapPositionBuffers()
    {
        (buffers["positionsFuture"], buffers["positionsCurrent"])
        = (buffers["positionsCurrent"], buffers["positionsFuture"]);
        (vertexArrays["positionsColorsFuture"], vertexArrays["positionsColorsCurrent"])
        = (vertexArrays["positionsColorsCurrent"], vertexArrays["positionsColorsFuture"]);
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
