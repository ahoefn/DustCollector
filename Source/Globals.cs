namespace DustCollector;

//Global numbers:
public static class Globals
{
    // The maximum workgroup size as specified by the OpenGL specs
    public const int WORKGROUPSIZE_X = 65535;
    public const int WORKGROUPSIZE_Y = 65535;
    public const int WORKGROUPSIZE_Z = 65535;

    // Shader constants:
    public const int LOCAL_SIZE_X = 64;

}

//Global paths:
public static class Paths
{
    public const string VERTEXPATH = "GameEngine/Shaders/GSLS/Shader.vert";
    public const string FRAGMENTPATH = "GameEngine/Shaders/GSLS/Shader.frag";
    public const string POSITIONUPDATERPATH = "GameEngine/Shaders/GSLS/PositionUpdater.comp";
    public const string VELOCITYUPDATERPATH = "GameEngine/Shaders/GSLS/VelocityUpdater.comp";
    public const string FORCEUPDATERPATHWCOLLISIONS = "GameEngine/Shaders/GSLS/ForceUpdaterWCollisions.comp";
    public const string FORCEUPDATERPATHNOCOLLISIONS = "GameEngine/Shaders/GSLS/ForceUpdaterNoCollisions.comp";
}