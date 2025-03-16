using OpenTK.Mathematics;
using System.IO;
namespace DustCollector;

public enum Direction
{
    up,
    front,
    right
}
interface ICamera
{
    public void UpdateAspect(int width, int height);
    public void ChangeOrientation(Vector2 delta);
    public void ChangePosition(Direction dir, float amount);
}

public class Camera : ICamera
{
    public Camera(int width, int height)
    {
        //Initial position and view vectors:
        _position = new Vector3(0.0f, 0.0f, 0.0f);
        _pitch = 0;
        _yaw = (float)Math.PI * -1 / 2;

        _directionVecs = new DirectionVecs();
        _directionVecs.Set(Direction.up, new Vector3(0.0f, 1.0f, 0.0f));
        _directionVecs.Set(Direction.front, new Vector3(0.0f, 0.0f, -1.0f));
        //Initialize matrices:
        model = Matrix4.Identity;
        projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, width / (float)height, 0.1f, 100.0f);
    }
    public Matrix4 model;
    public Matrix4 view
    {
        get => Matrix4.LookAt(_position, _position + _directionVecs.Get(Direction.front), _directionVecs.Get(Direction.up));
    }
    public Matrix4 projection;
    private Vector3 _position;
    private float _yaw;
    private float _pitch;
    private DirectionVecs _directionVecs;
    private struct DirectionVecs
    {
        private Vector3 _up;
        private Vector3 _front;

        readonly public Vector3 Get(Direction dir)
        {
            switch (dir)
            {
                case Direction.up:
                    return _up;
                case Direction.front:
                    return _front;
                case Direction.right:
                    return Vector3.Normalize(Vector3.Cross(_front, _up));
            }
            throw new ArgumentException("Not suitable input direction, suitable inputs are up, front or right.", nameof(dir));

        }
        public void Set(Direction dir, Vector3 v)
        {
            switch (dir)
            {
                case Direction.up:
                    _up = v;
                    return;
                case Direction.front:
                    _front = v;
                    return;
            }
            throw new ArgumentException("Can only set up o front directions.", nameof(dir));

        }
    }
    public void UpdateAspect(int width, int height)
    {
        projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, width / (float)height, 0.1f, 100.0f);
    }
    public void ChangeOrientation(Vector2 delta)
    {
        _yaw += delta.X * Globals.MOUSESENSITIVITY;

        //Limit up/down orientation between +-90 degrees:
        if (_pitch > (float)Math.PI / 2 - 0.05f) { _pitch = (float)Math.PI / 2 - 0.05f; }
        else if (_pitch < -(float)Math.PI / 2 + 0.05f) { _pitch = -(float)Math.PI / 2 + 0.05f; }
        else { _pitch -= delta.Y * Globals.MOUSESENSITIVITY; }

        //Update orientation vectors:
        Vector3 front;
        front.X = (float)Math.Cos(_pitch) * (float)Math.Cos(_yaw);
        front.Y = (float)Math.Sin(_pitch);
        front.Z = (float)Math.Cos(_pitch) * (float)Math.Sin(_yaw);
        front = Vector3.Normalize(front);
        _directionVecs.Set(Direction.front, front);

        var right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
        Vector3 up = Vector3.Normalize(Vector3.Cross(right, front));
        _directionVecs.Set(Direction.up, up);
    }
    public void ChangePosition(Direction dir, float amount)
    {
        Vector3 directionVec = _directionVecs.Get(dir);
        _position += amount * directionVec;
    }
}