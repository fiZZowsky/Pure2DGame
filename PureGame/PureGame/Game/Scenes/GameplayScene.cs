using OpenTK.Mathematics;
using PureGame.Engine.Content;
using PureGame.Engine.Core;
using PureGame.Engine.Graphics;

public class GameplayScene : Scene
{
    private Texture2D _playerTex;

    public override void OnEnter()
    {
        _playerTex = ContentManager.LoadTexture("Textures/player.png");
        var player = CreateEntity("Player");
        player.Position = new Vector2(100, 100);
        player.AddComponent(new PlayerController());
        //Engine.Audio.AudioManager.PreloadSfx("player_jump.wav");
    }

    public override void UnloadContent()
    {
        ContentManager.UnloadTexture("Textures/player.png");
    }
}

internal class PlayerController : Component
{
    public override void Update(double dt)
    {
        // Prosty ruch – przykład (WSAD):
        var speed = 200f;
        var m = OpenTK.Mathematics.Vector2.Zero;

        if (PureGame.Engine.Input.InputManager.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W)) m.Y -= 1;
        if (PureGame.Engine.Input.InputManager.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S)) m.Y += 1;
        if (PureGame.Engine.Input.InputManager.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A)) m.X -= 1;
        if (PureGame.Engine.Input.InputManager.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D)) m.X += 1;

        if (m.LengthSquared > 0) m = m.Normalized();

        Entity.Position += m * speed * (float)dt;
    }
}