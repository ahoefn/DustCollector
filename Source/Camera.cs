using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using System.IO;
namespace DustCollector;

class Camera
{
    public Camera(int width, int height)
    {
        position = new Vector3(0.0f, 0.0f, 0.0f);

        pitch = 0;
        yaw = (float)Math.PI * -1 / 2;
        front = new Vector3(0.0f, 0.0f, -1.0f); //Oriented in reverse z direction
        up = new Vector3(0.0f, 1.0f, 0.0f);


        _height = height;
        _width = width;
        model = Matrix4.Identity;
        view = Matrix4.Identity;
        projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, width / (float)height, 0.1f, 100.0f);
    }
    public Matrix4 model;
    public Matrix4 view;
    public Matrix4 projection;
    public Vector3 position;
    public Vector3 up;
    public Vector3 right { get { return Vector3.Normalize(Vector3.Cross(front, up)); } }
    public Vector3 front;

    public float yaw;
    public float pitch;
    private int _width;
    private int _height;

    public void UpdateAspect(int width, int height)
    {
        _width = width;
        _height = height;
        projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, width / (float)height, 0.1f, 100.0f);
    }
    public void ChangeOrientation(Vector2 delta)
    {
        yaw += delta.X * Globals.MOUSESENSITIVITY;

        if (pitch > (float)Math.PI / 2 - 0.05f) { pitch = (float)Math.PI / 2 - 0.05f; }
        else if (pitch < -(float)Math.PI / 2 + 0.05f) { pitch = -(float)Math.PI / 2 + 0.05f; }
        else { pitch -= delta.Y * Globals.MOUSESENSITIVITY; }

        front.X = (float)Math.Cos(pitch) * (float)Math.Cos(yaw);
        front.Y = (float)Math.Sin(pitch);
        front.Z = (float)Math.Cos(pitch) * (float)Math.Sin(yaw);
        front = Vector3.Normalize(front);

        var right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
        up = Vector3.Normalize(Vector3.Cross(right, front));

    }
}