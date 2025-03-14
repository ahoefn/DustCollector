using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace DustCollector.Renderer;
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
    public void CreateVertexBuffer(string name, float[] data, BufferUsageHint hint)
    {
        Use();
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
        Use();
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
        Use();
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, location, bufferIndex);
        buffers.Add(name, bufferIndex);
    }
    public void UpdateBuffer(string name, int newBufferIndex, int location)
    {//TODO: implement automatic Buffertype here as well
        Use();
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, location, newBufferIndex);
        buffers[name] = newBufferIndex;
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
    public void SetInt(string name, int i)
    {
        if (!_uniformlocations.ContainsKey(name)) { Console.WriteLine("Error: no uniform with name " + name); }
        GL.UseProgram(handle);
        GL.Uniform1(_uniformlocations[name], i);
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