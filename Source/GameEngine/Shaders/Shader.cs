using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace DustCollector.GameEngine.Shaders;

/// <summary>
/// General shader class containing methods shared between the compute and geometryshader classes.
/// </summary>
public class Shader : IDisposable
{
    public Shader(IBufferHandler bufferHandler_in)
    {
        _uniformlocations = new Dictionary<string, int>();
        _bufferHandler = bufferHandler_in;
    }
    // Properties:
    public int handle { get; protected init; }
    protected bool disposedValue = false;
    protected Dictionary<string, int> _uniformlocations;
    protected IBufferHandler _bufferHandler;

    // Methods:
    public static int CompileShader(string path, ShaderType type)
    {
        // Compile:
        string shaderSource = File.ReadAllText(path);
        int shaderHandle = GL.CreateShader(type);
        GL.ShaderSource(shaderHandle, shaderSource);
        GL.CompileShader(shaderHandle);

        // Check succes:
        GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out int succes);
        if (succes == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shaderHandle);
            throw new ArgumentException("Could not compile shader, possibly due to invalid path. InfoLog: " + infoLog, nameof(path));
        }
        return shaderHandle;
    }

    // Allows for preamble to 
    public static int CompileShader(string path, string preAmble, ShaderType type)
    {
        // Compile:
        string shaderSource = "#version 450 core\n" + preAmble + File.ReadAllText(path);
        int shaderHandle = GL.CreateShader(type);
        GL.ShaderSource(shaderHandle, shaderSource);
        GL.CompileShader(shaderHandle);

        // Check succes:
        GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out int succes);
        if (succes == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shaderHandle);
            throw new ArgumentException("Could not compile shader, possibly due to invalid path. InfoLog: " + infoLog, nameof(path));
        }
        return shaderHandle;
    }

    // Methods:
    public void Use()
    {
        GL.UseProgram(handle);
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

    // Set Uniforms:
    public void SetMatrix4(string name, Matrix4 matrix)
    {
        if (!_uniformlocations.ContainsKey(name)) { Console.WriteLine("Error: no uniform with name " + name); }
        Use();
        GL.UniformMatrix4(_uniformlocations[name], true, ref matrix);
    }

    public void SetFloat(string name, float f)
    {
        if (!_uniformlocations.ContainsKey(name))
        {
            throw new ArgumentException("No uniform of that name exists in this shader.", name);
        }
        Use();
        GL.Uniform1(_uniformlocations[name], f);
    }
    public void SetInt(string name, int i)
    {
        if (!_uniformlocations.ContainsKey(name))
        {
            throw new ArgumentException("No uniform of that name exists in this shader.", name);
        }
        Use();
        GL.Uniform1(_uniformlocations[name], i);
    }
    public void SetVec3(string name, Vector3 v)
    {
        if (!_uniformlocations.ContainsKey(name))
        {
            throw new ArgumentException("No uniform of that name exists in this shader.", name);
        }
        Use();
        GL.Uniform3(_uniformlocations[name], v);
    }

    // Dispose methods:
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