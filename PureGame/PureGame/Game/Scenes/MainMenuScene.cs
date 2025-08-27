using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PureGame.Engine.Content;
using PureGame.Engine.Core;
using PureGame.Engine.Graphics;
using PureGame.Engine.Input;

public class MainMenuScene : Scene
{
    private Texture2D[] _options = null!;
    private Texture2D _whiteTex = null!;
    private readonly Vector2 _optionSize = new Vector2(300, 80);
    private int _selected;

    public override void LoadContent()
    {
        _options = new[]
        {
            ContentManager.LoadTexture("Textures/new_game.png"),
            ContentManager.LoadTexture("Textures/settings.png"),
            ContentManager.LoadTexture("Textures/exit.png")
        };
        _whiteTex = ContentManager.LoadTexture("Textures/white.png");
    }

    public override void UnloadContent()
    {
        ContentManager.UnloadTexture("Textures/new_game.png");
        ContentManager.UnloadTexture("Textures/settings.png");
        ContentManager.UnloadTexture("Textures/exit.png");
        ContentManager.UnloadTexture("Textures/white.png");
    }

    public override void Update(double dt)
    {
        if (InputManager.IsKeyPressed(Keys.Up))
            _selected = (_selected + _options.Length - 1) % _options.Length;
        if (InputManager.IsKeyPressed(Keys.Down))
            _selected = (_selected + 1) % _options.Length;

        var mouse = InputManager.MousePosition;
        mouse.Y = Game.Camera.Height - mouse.Y; // convert to top-left origin

        for (int i = 0; i < _options.Length; i++)
        {
            var pos = OptionPosition(i);
            if (mouse.X >= pos.X && mouse.X <= pos.X + _optionSize.X &&
                mouse.Y >= pos.Y && mouse.Y <= pos.Y + _optionSize.Y)
            {
                _selected = i;
                if (InputManager.IsMouseButtonPressed(MouseButton.Left))
                    ActivateOption(i);
            }
        }

        if (InputManager.IsKeyPressed(Keys.Enter))
            ActivateOption(_selected);
    }

    private void ActivateOption(int index)
    {
        switch (index)
        {
            case 0:
                SceneManager.ChangeScene(new GameplayScene());
                break;
            case 2:
                Game.Instance.Close();
                break;
        }
    }

    private Vector2 OptionPosition(int index)
    {
        float x = (Game.Camera.Width - _optionSize.X) / 2f;
        float startY = 200f;
        float y = startY + index * (_optionSize.Y + 20f);
        return new Vector2(x, y);
    }

    public override void Draw()
    {
        var sb = Game.SpriteBatch;
        sb.Begin(Game.Camera);
        for (int i = 0; i < _options.Length; i++)
        {
            var pos = OptionPosition(i);
            if (i == _selected)
                sb.Draw(_whiteTex, pos, _optionSize, 0f, new Vector4(0.5f, 0.5f, 0.5f, 1f));
            sb.Draw(_options[i], pos, _optionSize);
        }
        sb.End();
    }
}