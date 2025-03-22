namespace DustCollector;

// Global settings that can freely be changed
public static class Settings
{
    // Size of the window opened:
    public const int WINDOWWIDTH = 1080;
    public const int WINDOWHEIGHT = 800;

    // Particle properties:
    public const float POINTSIZE = 80;
    public const float LUMINOSITY = 0.8f; // Determines base brightness of particles.

    // Size of the cube of particles that is generated, total number of particles will be CUBESIZE^3.
    public const int CUBESIZE = 10;

    // Control constants:
    public const float MOVSPEED = 6.0f;
    public const float MOUSESENSITIVITY = 0.003f;

    // The strength of gravity in ForceUpdater.comp
    public const float GRAVITYSTRENGTH = 0.07f;

    // Enable or disable a repulsive force between the particles
    public const bool COLLISSIONS = true;
    public const float COLLISSIONSTRENGTH = 0.5f * GRAVITYSTRENGTH;

    // Enable OpenGL debugging
    public const bool OPENGLDEBUGGING = false;
}
