using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;
using PureGame.Engine.Content;
using PureGame.Engine.Core;
using PureGame.Engine.Graphics;
using PureGame.Engine.Input;
using PureGame.Engine.UI;

public class SettingsScene : Scene
{
    private Texture2D _backTex = null!;
    private Texture2D _saveTex = null!;
    private Texture2D _whiteTex = null!;
    private Texture2D[] _resolutionTex = null!;
    private LabeledCheckbox _fullscreenCheck = null!;

    private readonly Vector2i[] _resolutions =
    {
        new(800, 600),
        new(1024, 768),
        new(1280, 720),
        new(1920, 1080)
    };

    private int _selectedRes;
    private bool _pending;

    public override void LoadContent()
    {
        _backTex = ContentManager.LoadTexture("Textures/back.png");
        _saveTex = ContentManager.LoadTexture("Textures/save.png");
        _whiteTex = ContentManager.LoadTexture("Textures/white.png");

        _resolutionTex = new Texture2D[_resolutions.Length];
        for (int i = 0; i < _resolutions.Length; i++)
        {
            var r = _resolutions[i];
            _resolutionTex[i] = ContentManager.LoadTexture($"Textures/res/{r.X}x{r.Y}.png");
        }

        _fullscreenCheck = new LabeledCheckbox(_whiteTex, new Vector2(100, 100), "Fullscreen", Game.Instance.WindowState == WindowState.Fullscreen);
        var size = Game.Instance.Size;
        _selectedRes = Array.FindIndex(_resolutions, r => r == size);
        if (_selectedRes < 0) _selectedRes = 0;
        _pending = false;
    }

    public override void UnloadContent()
    {
        ContentManager.UnloadTexture("Textures/back.png");
        ContentManager.UnloadTexture("Textures/save.png");
        ContentManager.UnloadTexture("Textures/white.png");
        _fullscreenCheck.Dispose();
        for (int i = 0; i < _resolutions.Length; i++)
            ContentManager.UnloadTexture($"Textures/res/{_resolutions[i].X}x{_resolutions[i].Y}.png");
    }

    public override void Update(double dt)
    {
        var mouse = InputManager.MousePosition;

        // back button
        if (InputManager.IsMouseButtonPressed(MouseButton.Left) &&
            mouse.X >= 10 && mouse.X <= 74 && mouse.Y >= 10 && mouse.Y <= 42)
        {
            SceneManager.ChangeScene(new MainMenuScene());
            return;
        }

        // fullscreen toggle
        if (_fullscreenCheck.Update(mouse))
            _pending = true;

        // resolution cycle
        var resPos = new Vector2(100, 160);
        var resSize = new Vector2(120, 32);
        if (IsClicked(mouse, resPos, resSize))
        {
            _selectedRes = (_selectedRes + 1) % _resolutions.Length;
            _pending = true;
        }

        // save button
        if (_pending)
        {
            var savePos = new Vector2(Game.Camera.Width - 74, Game.Camera.Height - 42);
            if (IsClicked(mouse, savePos, new Vector2(64, 32)))
            {
                ApplySettings();
                _pending = false;
            }
        }
    }

    private bool IsClicked(Vector2 mouse, Vector2 pos, Vector2 size)
    {
        return mouse.X >= pos.X && mouse.X <= pos.X + size.X &&
               mouse.Y >= pos.Y && mouse.Y <= pos.Y + size.Y &&
               InputManager.IsMouseButtonPressed(MouseButton.Left);
    }

    private void ApplySettings()
    {
        Game.Instance.WindowState = _fullscreenCheck.Checked ? WindowState.Fullscreen : WindowState.Normal;
        var res = _resolutions[_selectedRes];
        Game.Instance.Size = res;
        Game.Camera.Resize(res.X, res.Y);
    }

    public override void Draw()
    {
        var sb = Game.SpriteBatch;
        sb.Begin(Game.Camera);

        // back button
        sb.Draw(_backTex, new Vector2(10, 10), new Vector2(64, 32));

        _fullscreenCheck.Draw(sb);

        // resolution option
        sb.Draw(_resolutionTex[_selectedRes], new Vector2(100, 160), new Vector2(120, 32));

        // save button if pending
        if (_pending)
            sb.Draw(_saveTex, new Vector2(Game.Camera.Width - 74, Game.Camera.Height - 42), new Vector2(64, 32));

        sb.End();
    }
}