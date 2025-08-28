using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PureGame.Engine.Content;
using PureGame.Engine.Core;
using PureGame.Engine.Graphics;
using PureGame.Engine.Input;

public class MainMenuScene : Scene
{
    private Texture2D[] _options = null!;
    private readonly Vector2 _optionSize = new Vector2(300, 70);
    private int _selected = -1;

    public override void LoadContent()
    {
        _options = new[]
        {
            ContentManager.LoadTexture("Textures/new_game.png"),
            ContentManager.LoadTexture("Textures/settings.png"),
            ContentManager.LoadTexture("Textures/exit.png")
        };
    }

    public override void UnloadContent()
    {
        ContentManager.UnloadTexture("Textures/new_game.png");
        ContentManager.UnloadTexture("Textures/settings.png");
        ContentManager.UnloadTexture("Textures/exit.png");
    }

    public override void Update(double dt)
    {
        _selected = -1;
        var mouse = InputManager.MousePosition;

        for (int i = 0; i < _options.Length; i++)
        {
            var pos = OptionPosition(i);
            var center = pos + _optionSize * 0.5f;
            var half = _optionSize * 0.25f;
            bool inside = mouse.X >= center.X - half.X && mouse.X <= center.X + half.X &&
                          mouse.Y >= center.Y - half.Y && mouse.Y <= center.Y + half.Y;
            if (inside)
            {
                _selected = i;
                if (InputManager.IsMouseButtonPressed(MouseButton.Left))
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
        for (int i = 0; i < _options.Length; i++)
        {
            float scale = i == _selected ? 1.1f : 1f;
            Vector2 size = _optionSize * scale;
            var pos = OptionPosition(i) - (size - _optionSize) * 0.5f;
            sb.Draw(_options[i], pos, size);
        }
        sb.End();
    }
}