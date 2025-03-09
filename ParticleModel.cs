using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace DustCollector;



class ParticleModel : Shader
{
    public ParticleModel(string computePath) : base()
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

    public int particleCount = 0;
    public float[] GeneratePositions(int dimensions)
    {//Generates particles spaced out in a cube

        if (particleCount != dimensions * dimensions * dimensions)
        {
            Console.WriteLine("dimensions does not match particleCount\n");
        }

        var particles = new float[3 * particleCount];
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                for (int k = 0; k < dimensions; k++)
                {
                    int index = 3 * (i + dimensions * j + dimensions * dimensions * k);
                    particles[index] = i - dimensions / 2;
                    particles[index + 1] = j - dimensions / 2;
                    particles[index + 2] = k - dimensions / 2;
                }
            }
        }
        return particles;
    }


    public Particles GenerateVertices(int dimensions)
    {

        if (particleCount != dimensions * dimensions * dimensions)
        {
            Console.WriteLine("dimensions does not match particleCount\n");
        }

        var particles = new Particles(particleCount);
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                for (int k = 0; k < dimensions; k++)
                {
                    int index = i + dimensions * j + dimensions * dimensions * k;
                    particles.SetPosition(index, i - dimensions / 2, j - dimensions / 2, k - dimensions / 2);
                    particles.SetVelocity(index, (float)i / dimensions, (float)j / dimensions, (float)(dimensions - i - j) / dimensions);
                }
            }
        }
        return particles;
    }

    public float[] GenerateColors(int dimensions)
    {
        if (particleCount != dimensions * dimensions * dimensions)
        {
            Console.WriteLine("dimensions does not match particleCount\n");
        }
        var colors = new float[3 * particleCount];
        int currentIndex;
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                for (int k = 0; k < dimensions; k++)
                {
                    currentIndex = 3 * (i + dimensions * j + dimensions * dimensions * k);
                    colors[currentIndex] = (float)i / dimensions;
                    colors[currentIndex + 1] = (float)j / dimensions;
                    colors[currentIndex + 2] = (float)(dimensions - i - j) / dimensions;
                }
            }
        }
        return colors;
    }
}