using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace DustCollector;

public class Shader : IDisposable
{
    public Shader()
    {
        _uniformlocations = new Dictionary<string, int>();
        buffers = new Dictionary<string, int>();
    }
    //Data:
    public int handle { get; protected init; }
    protected bool disposedValue = false;
    protected Dictionary<string, int> _uniformlocations;
    public Dictionary<string, int> buffers;

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
    public GeometryShader(string vertexPath, string fragmentPath) : base()
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
    public void CreateGeneralArray(string name, float[] data, int stride)
    {
        CreateVertexBuffer(name + "Buffer", data);
        CreateVertexArray(name + "Array", name + "Buffer");
        //Note: the following implicitly depends on which VertexBufferObject is currently bound
        GL.VertexAttribPointer(
            0, //Location
            stride, //Size, vec3 so three values
            VertexAttribPointerType.Float, //Size of each element
            false, //Is data normalized?
            stride * sizeof(float), //Stride = size diff between data entries
            0 //Offset
        );
        GL.EnableVertexAttribArray(0);
    }

    public void CreateParticleArray(string name, float[] data)
    {
        CreateVertexBuffer(name + "Buffer", data);
        CreateVertexArray(name + "Array", name + "Buffer");
        //Note: the following implicitly depends on which VertexBufferObject is currently bound
        GL.VertexAttribPointer(
            0, //Location
            3, //Size, vec3 so three values
            VertexAttribPointerType.Float, //Size of each element
            false, //Is data normalized?
            6 * sizeof(float), //Stride = size diff between data entries
            0 //Offset
        );
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(
            1,
            3,
            VertexAttribPointerType.Float,
            false,
            6 * sizeof(float),
            3 * sizeof(float)
        );
        GL.EnableVertexAttribArray(1);
    }
    public void CreateVertexBuffer(string name, float[] data)
    {
        int vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(
                    BufferTarget.ArrayBuffer,
                    data.Length * sizeof(float),
                    data,
                    BufferUsageHint.DynamicDraw
        );
        buffers.Add(name, vertexBufferObject);
    }
    public void CreateVertexArray(string name, string vertexBuffer)
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, buffers[vertexBuffer]);
        int vertexArrayObject = GL.GenVertexArray();
        vertexArrays.Add(name, vertexArrayObject);
    }
}