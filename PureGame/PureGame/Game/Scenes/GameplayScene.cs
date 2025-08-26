using PureGame.Engine.Core;
using OpenTK.Mathematics;

public class GameplayScene : Scene
{
    public override void OnEnter()
    {
        var player = CreateEntity("Player");
        player.Position = new Vector2(100, 100);
        player.AddComponent(new PlayerController());
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