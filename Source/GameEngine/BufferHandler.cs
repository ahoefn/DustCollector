using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace DustCollector.GameEngine;

interface IBufferHandler : IDisposable
{
    public void CreateVertexBuffer(Buffer buffer, float[] data, BufferUsageHint hint);

    public void CreateStorageBuffer(Buffer buffer, float[] data, BufferUsageHint hint);
    public void SwapBuffers(Buffer buffer1, Buffer buffer2);
    public int GetBufferHandle(Buffer buffer);
    public void AddBuffer(Buffer buffer, int bufferHandle);
    public void RemoveBuffer(Buffer buffer);
}

public enum Buffer
{
    positionsCurrent,
    positionsFuture,
    velocitiesCurrent,
    velocitiesFuture,
    forcesCurrent,
    forcesFuture,
    colors
}

public class BufferHandler : IBufferHandler
{
    public BufferHandler()
    {
        _buffers = new Dictionary<Buffer, int>();
    }
    // Properties:
    private readonly Dictionary<Buffer, int> _buffers;
    private bool _disposedValue = false;

    // Methods:
    public void CreateVertexBuffer(Buffer buffer, float[] data, BufferUsageHint hint)
    {
        int vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(
                    BufferTarget.ArrayBuffer,
                    data.Length * sizeof(float),
                    data,
                    hint
        );
        _buffers.Add(buffer, vertexBufferObject);
    }
    public void CreateStorageBuffer(Buffer buffer, float[] data, BufferUsageHint hint)
    {
        int vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, vertexBufferObject);
        GL.BufferData(
                    BufferTarget.ShaderStorageBuffer,
                    data.Length * sizeof(float),
                    data,
                    hint
        );
        _buffers.Add(buffer, vertexBufferObject);
    }
    public void SwapBuffers(Buffer buffer1, Buffer buffer2)
    {
        (_buffers[buffer1], _buffers[buffer2]) = (_buffers[buffer2], _buffers[buffer1]);
    }
    public int GetBufferHandle(Buffer bufferName)
    {
        return _buffers[bufferName];
    }
    public void AddBuffer(Buffer bufferName, int bufferInt)
    {
        _buffers.Add(bufferName, bufferInt);
    }
    public void RemoveBuffer(Buffer bufferName)
    {
        GL.DeleteBuffer(_buffers[bufferName]);
        _buffers.Remove(bufferName);
    }
    public float[] GetBufferData(Buffer buffer, int size)
    {
        float[] output = new float[size];
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _buffers[buffer]);
        GL.GetBufferSubData(BufferTarget.ShaderStorageBuffer, 0, size * sizeof(float), output);
        return output;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            foreach ((Buffer buffer, int handle) in _buffers)
            {
                RemoveBuffer(buffer);
            }
            _disposedValue = true;
        }
    }

    ~BufferHandler()
    {
        if (!_disposedValue)
        {
            Console.WriteLine("GPU Resource leak in buffer handler. Did you forget to call Dispose()?");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
