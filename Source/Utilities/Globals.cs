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
/// Contains the paths for the different shaders, includes the global path.
/// </summary>
public static class Paths
{
    private static readonly string _GLOBALPATH = AppContext.BaseDirectory;
    public static string VERTEXPATH { get => _GLOBALPATH + "Shaders/Shader.vert"; }
    public static string FRAGMENTPATH { get => _GLOBALPATH + "Shaders/Shader.frag"; }
    public static string POSITIONUPDATERPATH { get => _GLOBALPATH + "Shaders/PositionUpdater.comp"; }
    public static string VELOCITYUPDATERPATH { get => _GLOBALPATH + "Shaders/VelocityUpdater.comp"; }
    public static string FORCEUPDATERPATHWCOLLISIONS { get => _GLOBALPATH + "Shaders/ForceUpdaterWCollisions.comp"; }
    public static string FORCEUPDATERPATHNOCOLLISIONS { get => _GLOBALPATH + "Shaders/ForceUpdaterNoCollisions.comp"; }
}