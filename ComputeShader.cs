using OpenTK.Graphics.OpenGL4;
namespace DustCollector;
class ComputeShader : Shader
{
    public ComputeShader(string computePath) : base(BufferTarget.ShaderStorageBuffer)
    {
        string computeShaderSource = File.ReadAllText(computePath);
        int computeShader = GL.CreateShader(ShaderType.ComputeShader);
        GL.ShaderSource(computeShader, computeShaderSource);

        GL.CompileShader(computeShader);
        GL.GetShader(computeShader, ShaderParameter.CompileStatus, out int succes);
        if (succes == 0)
        {
            string infoLog = GL.GetShaderInfoLog(computeShader);
            Console.WriteLine(infoLog + "No compute shader");
        }
        handle = GL.CreateProgram();
        GL.AttachShader(handle, computeShader);
        GL.LinkProgram(handle);

        GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out succes);
        if (succes == 0)
        {
            string infoLog = GL.GetProgramInfoLog(handle);
            Console.WriteLine(infoLog + "No program");
        }

        GL.DetachShader(handle, computeShader);
        GL.DeleteShader(computeShader);

        UpdateUniforms();
    }

    public void Dispatch(int x_in, int y_in, int z_in)
    {//TODO: impelment y and z 
        (int x, int y, int z) currentCount = (x_in, y_in, z_in);
        // int xCount = 0;
        // while (!(currentCount.x < Globals.WORKGROUPSIZE_X))
        // {
        //     xCount += 1;
        //     currentCount.x -= Globals.WORKGROUPSIZE_X;
        // }
        // for (int i = 0; i < xCount; i++)
        // {

        //     SetInt("offSetX", i * Globals.WORKGROUPSIZE_X);
        //     GL.DispatchCompute(Globals.WORKGROUPSIZE_X, 1, 1);
        //     // Console.WriteLine(GL.GetError().ToString());
        // }
        // SetInt("offSetX", xCount * Globals.WORKGROUPSIZE_X);
        GL.DispatchCompute(currentCount.x, currentCount.y, currentCount.z);
        // Console.WriteLine(GL.GetError().ToString());
    }
    public void SwapPositionBuffers()
    {
        Use();
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, buffers["positionsFuture"]);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, buffers["positionsCurrent"]);
        (buffers["positionsFuture"], buffers["positionsCurrent"])
    = (buffers["positionsCurrent"], buffers["positionsFuture"]);
    }
}