using OpenTK.Mathematics;
namespace DustCollector;
struct Particles
{
    public Particles(int count_in)
    {
        count = count_in;
        data = new float[6 * count];
    }
    public float[] data;
    int count;
    public float x(int index)
    {
        return data[6 * index];
    }
    public float y(int index)
    {
        return data[6 * index + 1];
    }
    public float z(int index)
    {
        return data[6 * index + 2];
    }
    public float vx(int index)
    {
        return data[6 * index + 3];
    }
    public float vy(int index)
    {
        return data[6 * index + 4];
    }
    public float vz(int index)
    {
        return data[6 * index + 5];
    }
    public Vector3 postition(int index)
    {
        return new Vector3(x(index), y(index), z(index));
    }
    public Vector3 velocity(int index)
    {
        return new Vector3(vx(index), vy(index), vz(index));
    }
    public void SetPosition(int index, float x_in, float y_in, float z_in)
    {
        data[6 * index] = x_in;
        data[6 * index + 1] = y_in;
        data[6 * index + 2] = z_in;
    }
    public void SetVelocity(int index, float vx_in, float vy_in, float vz_in)
    {
        data[6 * index + 3] = vx_in;
        data[6 * index + 4] = vy_in;
        data[6 * index + 5] = vz_in;
    }
}
