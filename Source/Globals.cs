namespace DustCollector;

public static class Globals
{
    public const float POINTSIZE = 200;
    public const float MOVSPEED = 6.0f;
    public const float MOUSESENSITIVITY = 0.003f;
    //TODO: Implement dynamically choosing workgroupsize: 
    public static int WORKGROUPSIZE_X = 65535 - 100;
    public static int WORKGROUPSIZE_Y = 65535 - 1;
    public static int WORKGROUPSIZE_Z = 65535 - 100;
}