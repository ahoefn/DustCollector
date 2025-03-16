using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace DustCollector.GameEngine.Shaders;

interface IBufferHandler
{
    public void CreateVertexBuffer(string name, float[] data, BufferUsageHint hint);

    public void CreateStorageBuffer(string name, float[] data, BufferUsageHint hint);
    public void SwapBuffers(string buffer1, string buffer2);
}

public class BufferHandler : IBufferHandler
{
    public BufferHandler()
    {
        buffers = new Dictionary<string, int>();
    }
    public Dictionary<string, int> buffers;
    public void CreateVertexBuffer(string name, float[] data, BufferUsageHint hint)
    {
        int vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(
                    BufferTarget.ArrayBuffer,
                    data.Length * sizeof(float),
                    data,
                    hint
        );
        buffers.Add(name, vertexBufferObject);
    }
    public void CreateStorageBuffer(string name, float[] data, BufferUsageHint hint)
    {
        int vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, vertexBufferObject);
        GL.BufferData(
                    BufferTarget.ShaderStorageBuffer,
                    data.Length * sizeof(float),
                    data,
                    hint
        );
        buffers.Add(name, vertexBufferObject);
    }
    public void SwapBuffers(string buffer1, string buffer2)
    {
        (buffers[buffer1], buffers[buffer2]) = (buffers[buffer2], buffers[buffer1]);
    }
}