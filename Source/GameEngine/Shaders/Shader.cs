using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace DustCollector.GameEngine.Shaders;
public class Shader : IDisposable, IBufferHandler
{
    public Shader(BufferHandler bufferHandler_in)
    {
        _uniformlocations = new Dictionary<string, int>();
        _bufferHandler = bufferHandler_in;
    }
    //Data:
    public int handle { get; protected init; }
    protected bool disposedValue = false;
    protected Dictionary<string, int> _uniformlocations;
    protected BufferHandler _bufferHandler;

    //Methods:
    public static int CompileShader(string path, ShaderType type)
    {
        //Compile:
        string shaderSource = File.ReadAllText(path);
        int shaderHandle = GL.CreateShader(type);
        GL.ShaderSource(shaderHandle, shaderSource);
        GL.CompileShader(shaderHandle);

        //Check succes:
        GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out int succes);
        if (succes == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shaderHandle);
            throw new ArgumentException("Could not compile shader, possibly due to invalid path. InfoLog: " + infoLog, nameof(path));
        }
        return shaderHandle;
    }
    public static int CompileShader(string path, string preAmble, ShaderType type)
    {
        //Compile:
        string shaderSource = "#version 450 core\n" + preAmble + File.ReadAllText(path);
        int shaderHandle = GL.CreateShader(type);
        GL.ShaderSource(shaderHandle, shaderSource);
        GL.CompileShader(shaderHandle);

        //Check succes:
        GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out int succes);
        if (succes == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shaderHandle);
            throw new ArgumentException("Could not compile shader, possibly due to invalid path. InfoLog: " + infoLog, nameof(path));
        }
        return shaderHandle;
    }
    public void UpdateUniforms()
    {
        Use();
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
        Use();
        GL.UniformMatrix4(_uniformlocations[name], true, ref matrix);
    }

    public void SetFloat(string name, float f)
    {
        if (!_uniformlocations.ContainsKey(name)) { Console.WriteLine("Error: no uniform with name " + name); }
        Use();
        GL.Uniform1(_uniformlocations[name], f);
    }
    public void SetInt(string name, int i)
    {
        if (!_uniformlocations.ContainsKey(name)) { Console.WriteLine("Error: no uniform with name " + name); }
        Use();
        GL.Uniform1(_uniformlocations[name], i);
    }
    public void SetVec3(string name, Vector3 v)
    {
        if (!_uniformlocations.ContainsKey(name)) { Console.WriteLine("Error: no uniform with name " + name); }
        Use();
        GL.Uniform3(_uniformlocations[name], v);
    }

    // 
    // BufferHandler interface:
    public void CreateVertexBuffer(Buffer buffer, float[] data, BufferUsageHint hint)
    {
        _bufferHandler.CreateVertexBuffer(buffer, data, hint);
    }

    public void CreateStorageBuffer(Buffer buffer, float[] data, BufferUsageHint hint)
    {
        _bufferHandler.CreateStorageBuffer(buffer, data, hint);
    }
    public void SwapBuffers(Buffer buffer1, Buffer buffer2)
    {
        _bufferHandler.SwapBuffers(buffer1, buffer2);
    }
    public int GetBufferHandle(Buffer buffer)
    {
        return _bufferHandler.GetBufferHandle(buffer);
    }
    public void AddBuffer(Buffer buffer, int bufferInt)
    {
        _bufferHandler.AddBuffer(buffer, bufferInt);
    }
    public void RemoveBuffer(Buffer buffer)
    {
        _bufferHandler.RemoveBuffer(buffer);
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
            Console.WriteLine("GPU Resource leak in shader. Did you forget to call Dispose()?");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}