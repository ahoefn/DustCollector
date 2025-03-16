using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace DustCollector.GameEngine;

interface IBufferHandler
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
    ForcesCurrent,
    ForcesFuture,
    Colors
}

public class BufferHandler : IBufferHandler
{
    public BufferHandler()
    {
        _buffers = new Dictionary<Buffer, int>();
    }
    private readonly Dictionary<Buffer, int> _buffers;
    public void CreateVertexBuffer(Buffer name, float[] data, BufferUsageHint hint)
    {
        int vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(
                    BufferTarget.ArrayBuffer,
                    data.Length * sizeof(float),
                    data,
                    hint
        );
        _buffers.Add(name, vertexBufferObject);
    }
    public void CreateStorageBuffer(Buffer name, float[] data, BufferUsageHint hint)
    {
        int vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, vertexBufferObject);
        GL.BufferData(
                    BufferTarget.ShaderStorageBuffer,
                    data.Length * sizeof(float),
                    data,
                    hint
        );
        _buffers.Add(name, vertexBufferObject);
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
        _buffers.Remove(bufferName);
    }
}