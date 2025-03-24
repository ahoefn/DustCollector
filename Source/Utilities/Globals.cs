namespace DustCollector;

/// <summary>
/// Global constants, currently only OpenGL related.
/// </summary>
public static class Globals
{
    // The maximum workgroup size as specified by the OpenGL specs
    public const int WORKGROUPSIZE_X = 65535;
    public const int WORKGROUPSIZE_Y = 65535;
    public const int WORKGROUPSIZE_Z = 65535;

    // Shader constants:
    public const int LOCAL_SIZE_X = 64;

}

/// <summary>
/// Contains the relative paths of all the shaders.
/// </summary>
public static class Paths
{
    public const string VERTEXPATH = "Shaders/Shader.vert";
    public const string FRAGMENTPATH = "Shaders/Shader.frag";
    public const string POSITIONUPDATERPATH = "Shaders/PositionUpdater.comp";
    public const string VELOCITYUPDATERPATH = "Shaders/VelocityUpdater.comp";
    public const string FORCEUPDATERPATHWCOLLISIONS = "Shaders/ForceUpdaterWCollisions.comp";
    public const string FORCEUPDATERPATHNOCOLLISIONS = "Shaders/ForceUpdaterNoCollisions.comp";
}