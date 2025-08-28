using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PureGame.Engine.Content;
using PureGame.Engine.Core;
using PureGame.Engine.Graphics;
using PureGame.Engine.Input;
using PureGame.Engine.UI;

public class MainMenuScene : Scene
{
    private Texture2D _background = null!;
    private Button[] _options = null!;
    private readonly Vector2 _optionSize = new Vector2(300, 70);
    private int _selected = -1;

    public override void LoadContent()
    {
        _background = ContentManager.LoadTexture("Textures/main_menu_background.png");

        var textures = new[]
        {
            ContentManager.LoadTexture("Textures/new_game.png"),
            ContentManager.LoadTexture("Textures/settings.png"),
            ContentManager.LoadTexture("Textures/exit.png")
        };

        _options = new Button[textures.Length];
        for (int i = 0; i < textures.Length; i++)
        {
            var pos = OptionPosition(i);
            _options[i] = new Button(textures[i], pos, _optionSize);
        }
    }

    public override void UnloadContent()
    {
        ContentManager.UnloadTexture("Textures/main_menu_background.png");
        ContentManager.UnloadTexture("Textures/new_game.png");
        ContentManager.UnloadTexture("Textures/settings.png");
        ContentManager.UnloadTexture("Textures/exit.png");
    }

    public override void Update(double dt)
    {
        var mouse = InputManager.MousePosition;

        for (int i = 0; i < _options.Length; i++)
        {
            if (_options[i].Update(mouse))
            {
                ActivateOption(i);
                break;
            }
        }
    }

    private void ActivateOption(int index)
    {
        switch (index)
        {
            case 0:
                SceneManager.ChangeScene(new GameplayScene());
                break;
            case 1:
                SceneManager.ChangeScene(new SettingsScene());
                break;
            case 2:
                Game.Instance.Close();
                System.Environment.Exit(0);
                break;
        }
    }

    private Vector2 OptionPosition(int index)
    {
        float margin = 50f;
        float startY = 200f;
        float y = startY + index * (_optionSize.Y + 20f);
        return new Vector2(margin, y);
    }

    public override void Draw()
    {
        var sb = Game.SpriteBatch;
        sb.Begin(Game.Camera);
        sb.Draw(_background, Vector2.Zero, new Vector2(Game.Camera.Width, Game.Camera.Height));
        for (int i = 0; i < _options.Length; i++)
        {
            _options[i].Draw(sb);
        }
        sb.End();
    }
}