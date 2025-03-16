namespace DustCollector;

//Global numbers:
public static class Globals
{
    public const float POINTSIZE = 200;
    public const float MOVSPEED = 6.0f;
    public const float MOUSESENSITIVITY = 0.003f;
    //TODO: Implement dynamically choosing workgroupsize: 
    public const int WORKGROUPSIZE_X = 65535;
    public const int WORKGROUPSIZE_Y = 65535;
    public const int WORKGROUPSIZE_Z = 65535;
}

//Global paths:
public static class Paths
{
    public const string VERTEXPATH = "GameEngine/Shaders/GSLS/Shader.vert";
    public const string FRAGMENTPATH = "GameEngine/Shaders/GSLS/Shader.frag";
    public const string VELOCITYUPDATERPATH = "GameEngine/Shaders/GSLS/VelocityUpdater.comp";
    public const string POSITIONUPDATERPATH = "GameEngine/Shaders/GSLS/PositionUpdater.comp";
}