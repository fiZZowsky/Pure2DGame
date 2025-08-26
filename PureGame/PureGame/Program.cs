using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace PureGame;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        var gw = GameWindowSettings.Default;
        var nw = new NativeWindowSettings
        {
            Size = new OpenTK.Mathematics.Vector2i(1280, 720),
            Title = "Pure Game",
            StartVisible = true,
            StartFocused = true
        };

        using var game = new Engine.Core.Game(gw, nw);
        game.Run();
    }
}