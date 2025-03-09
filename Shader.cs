using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace DustCollector;

public class Shader : IDisposable
{
    public Shader(BufferTarget bufferTarget_in)
    {
        _uniformlocations = new Dictionary<string, int>();
        buffers = new Dictionary<string, int>();
        _bufferTarget = bufferTarget_in;
    }
    //Data:
    public int handle { get; protected init; }
    protected bool disposedValue = false;
    protected Dictionary<string, int> _uniformlocations;
    public Dictionary<string, int> buffers;
    private protected BufferTarget _bufferTarget;

    //Methods:
    public void UpdateUniforms()
    {
        GL.GetProgram(handle, GetProgramParameterName.ActiveUniforms, out int uniformCount);
        string key;
        int location;
        for (int i = 0; i < uniformCount; i++)
        {
            key = GL.GetActiveUniform(handle, i, out _, out _);
            location = GL.GetUniformLocation(handle, key);

            _uniformlocations.Add(key, location);
        }
    }
    public void CreateVertexBuffer(string name, float[] data, BufferUsageHint hint)
    {
        int vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(_bufferTarget, vertexBufferObject);
        GL.BufferData(
                    _bufferTarget,
                    data.Length * sizeof(float),
                    data,
                    hint
        );
        buffers.Add(name, vertexBufferObject);
    }
    public void CreateStorageBuffer(string name, float[] data, int location, BufferUsageHint hint)
    {
        int vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, vertexBufferObject);
        GL.BufferData(
                    _bufferTarget,
                    data.Length * sizeof(float),
                    data,
                    hint
        );
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, location, vertexBufferObject);
        buffers.Add(name, vertexBufferObject);

    }
    public void ShareBuffer(string name, int bufferIndex, int location)
    {//TODO: implement automatic Buffertype here as well
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, location, bufferIndex);
        buffers.Add(name, bufferIndex);
    }


    public void Use()
    {
        GL.UseProgram(handle);
    }

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        if (!_uniformlocations.ContainsKey(name)) { Console.WriteLine("Error: no uniform with name " + name); }
        GL.UseProgram(handle);
        GL.UniformMatrix4(_uniformlocations[name], true, ref matrix);
    }

    public void SetFloat(string name, float f)
    {
        if (!_uniformlocations.ContainsKey(name)) { Console.WriteLine("Error: no uniform with name " + name); }
        GL.UseProgram(handle);
        GL.Uniform1(_uniformlocations[name], f);
    }
    public void SetVec3(string name, Vector3 v)
    {
        if (!_uniformlocations.ContainsKey(name)) { Console.WriteLine("Error: no uniform with name " + name); }
        GL.UseProgram(handle);
        GL.Uniform3(_uniformlocations[name], v);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            GL.DeleteProgram(handle);
            disposedValue = true;
        }
    }

    ~Shader()
    {
        if (!disposedValue)
        {
            Console.WriteLine("GPU Resource leak. Did you forget to call Dispose()?");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

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
    public void CreatePositionColorArray(float[] positions, float[] colors)
    {
        CreateVertexBuffer("positions", positions, BufferUsageHint.StreamDraw);
        CreateVertexBuffer("colors", colors, BufferUsageHint.StreamDraw);
        CreateVertexArray("positionsColors");

        BindBufferToArray("positions", "positionsColors", 0, 3);
        BindBufferToArray("colors", "positionsColors", 1, 3);
    }
    public void BindBufferToArray(string buffer, string array, int location, int stride)
    {
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