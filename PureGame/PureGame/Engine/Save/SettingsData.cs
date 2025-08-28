using OpenTK.Mathematics;

namespace PureGame.Engine.Save;

public record SettingsData(int Width = 1280, int Height = 720, bool Fullscreen = false)
{
    public Vector2i Resolution => new(Width, Height);
}