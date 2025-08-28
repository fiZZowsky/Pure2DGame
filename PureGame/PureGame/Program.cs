using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using PureGame.Engine.Save;

namespace PureGame;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        var settings = SettingsManager.Load();
        var gw = GameWindowSettings.Default;
        var nw = new NativeWindowSettings
        {
            Size = settings.Resolution,
            Title = "Pure Game",
            StartVisible = true,
            StartFocused = true
        };

        using var game = new Engine.Core.Game(gw, nw);
        game.WindowState = settings.Fullscreen ? WindowState.Fullscreen : WindowState.Normal;
        game.Run();
    }
}