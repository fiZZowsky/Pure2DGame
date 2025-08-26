using PureGame.Engine.Core;
using OpenTK.Windowing.GraphicsLibraryFramework;
using static PureGame.Engine.Input.InputManager;

public class MainMenuScene : Scene
{
    public override void OnEnter()
    {
        // Tworzymy encję z komponentem, który reaguje na klawisze
        var e = CreateEntity("MenuController");
        e.AddComponent(new MenuController());
    }

    public override void Update(double dt)
    {
        // np. tło animowane albo logika menu na poziomie sceny
    }
}

internal class MenuController : Component
{
    public override void Update(double dt)
    {
        if (IsKeyPressed(Keys.Enter))
        {
            // przejście do gameplay
            SceneManager.ChangeScene(new GameplayScene());
        }
    }
}