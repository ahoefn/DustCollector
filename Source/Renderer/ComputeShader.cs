using OpenTK.Graphics.OpenGL4;
namespace DustCollector.Renderer;
public class ComputeShader : Shader
{
    public ComputeShader(string computePath) : base(BufferTarget.ShaderStorageBuffer)
    {
        //Compile shader and attach to program:
        int computeShader = CompileShader(computePath, ShaderType.ComputeShader);

        handle = GL.CreateProgram();
        GL.AttachShader(handle, computeShader);
        GL.LinkProgram(handle);

        //Check succes:
        GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int succes);
        if (succes == 0)
        {
            string infoLog = GL.GetProgramInfoLog(handle);
            throw new ArgumentException("Could not Create program for compute shader, check shader code. InfoLog: " + infoLog, nameof(computePath));
        }

        //Cleanup shader as it is not necessary anymore:
        GL.DetachShader(handle, computeShader);
        GL.DeleteShader(computeShader);

        //Create uniform dictionary:
        UpdateUniforms();
    }
    public void Dispatch(int x_in, int y_in, int z_in)
    {// If a dispatch workgroup is too big (>Globals.WORKGROUPSIZE_X), separates the different dispatches in batches. 

        //Check input sizes:
        if (y_in > Globals.WORKGROUPSIZE_Y)
        {
            throw new ArgumentOutOfRangeException(nameof(y_in), "Workgroupsize in y direction is too large, consider using Dispatch3D.");
        }
        if (z_in > Globals.WORKGROUPSIZE_Z)
        {
            throw new ArgumentOutOfRangeException(nameof(z_in), "Workgroupsize in z direction is too large, consider using Dispatch3D.");
        }

        //Dispatch:
        (int x, int y, int z) currentCount = (x_in, y_in, z_in);
        int xCount = (currentCount.x - (currentCount.x % Globals.WORKGROUPSIZE_X)) / Globals.WORKGROUPSIZE_X;

        for (int i = 0; i < xCount; i++)
        {
            SetInt("offSetX", i * Globals.WORKGROUPSIZE_X);
            GL.DispatchCompute(Globals.WORKGROUPSIZE_X, 1, 1);
        }

        SetInt("offSetX", xCount * Globals.WORKGROUPSIZE_X);
        GL.DispatchCompute(currentCount.x, currentCount.y, currentCount.z);
    }

    public void Dispatch3D(int x_in, int y_in, int z_in)
    {// Same as dispatch, but for all three dimensions 
     // NOTE: requires that offSetX, offSetY and offSetZ are ALL used explicitly in the shader code, otherwise the compiler removes them and trying to set them will give an error.
        (int x, int y, int z) moduloCount = (x_in % Globals.WORKGROUPSIZE_X, y_in % Globals.WORKGROUPSIZE_X, z_in % Globals.WORKGROUPSIZE_X);
        int xCount = (x_in - moduloCount.x) / Globals.WORKGROUPSIZE_X;
        int yCount = (y_in - moduloCount.y) / Globals.WORKGROUPSIZE_Y;
        int zCount = (z_in - moduloCount.z) / Globals.WORKGROUPSIZE_Z;

        for (int i = 0; i < xCount; i++)
        {
            SetInt("offSetX", i * Globals.WORKGROUPSIZE_X);
            for (int j = 0; j < yCount; j++)
            {
                SetInt("offSetY", j * Globals.WORKGROUPSIZE_Y);
                for (int k = 0; k < zCount; k++)
                {
                    SetInt("offSetZ", k * Globals.WORKGROUPSIZE_Z);
                    GL.DispatchCompute(Globals.WORKGROUPSIZE_X, Globals.WORKGROUPSIZE_Y, Globals.WORKGROUPSIZE_Z);
                }

                SetInt("offSetZ", zCount * Globals.WORKGROUPSIZE_Z);
                GL.DispatchCompute(Globals.WORKGROUPSIZE_X, Globals.WORKGROUPSIZE_Y, moduloCount.z);
            }

            SetInt("offSetY", yCount * Globals.WORKGROUPSIZE_X);
            GL.DispatchCompute(Globals.WORKGROUPSIZE_X, moduloCount.y, moduloCount.z);
        }

        SetInt("offSetX", xCount * Globals.WORKGROUPSIZE_X);
        GL.DispatchCompute(moduloCount.x, moduloCount.y, moduloCount.z);
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