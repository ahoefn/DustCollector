namespace DustCollector;

// Global settings that can freely be changed
public static class Settings
{
    // Size of the window opened:
    public const int WINDOWWIDTH = 1080;
    public const int WINDOWHEIGHT = 800;
    // Visuals and control constants:
    public const float POINTSIZE = 200;
    public const float MOVSPEED = 6.0f;
    public const float MOUSESENSITIVITY = 0.003f;
    // Enables or disables a repulsive force between the particles
    public const bool COLLISSION = false;
    public const float COLLISSIONSTRENGTH = 0.01f * GRAVITYSTRENGTH;
    // The strength of gravity in ForceUpdater.comp
    public const float GRAVITYSTRENGTH = 0.1f;

    // Enable OpenGL debugging
    public const bool OPENGLDEBUGGING = false;
}
