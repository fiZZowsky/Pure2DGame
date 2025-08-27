using PureGame.Engine.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace PureGame.Engine.Core;

public class Game : GameWindow
{
    private double _accumulator;
    private const double FixedDelta = 1.0 / 60.0; // 60 FPS update
    private Camera2D _camera;
    private SpriteBatch _spriteBatch;
    private Texture2D _texPlayer;

    public Game(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.1f, 0.1f, 0.12f, 1f);

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        Engine.Audio.AudioManager.Initialize();
        Engine.Content.ContentManager.Initialize();

        _camera = new Camera2D(Size.X, Size.Y);
        _spriteBatch = new SpriteBatch();
        
        //_texPlayer = new Texture2D("./Engine/Content/player.png");

        SceneManager.ChangeScene(new MainMenuScene());
        //Engine.Audio.AudioManager.PreloadSfx("ui_click.wav");
        //Engine.Audio.AudioManager.PlayMusic("bgm_menu.mp3", loop: true, volume: 0.6f);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);
        _camera.Resize(Size.X, Size.Y);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        _accumulator += args.Time;

        // Update input
        Input.InputManager.Update(KeyboardState, MouseState);

        if (Input.InputManager.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
            Close();

        while (_accumulator >= FixedDelta)
        {
            // TODO: SceneManager.FixedUpdate(FixedDelta);
            _accumulator -= FixedDelta;
        }

        // TODO: SceneManager.Update(args.Time);

        if (IsExiting) return;

        SceneManager.Update(args.Time);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        _spriteBatch.Begin(_camera);
        _spriteBatch.Draw(
            _texPlayer,
            position: new Vector2(200, 150),
            size: new Vector2(128, 128),
            rotationRad: 0f,
            color: new Vector4(1f, 1f, 1f, 1f)
        );
        _spriteBatch.End();

        SwapBuffers();

        SceneManager.Draw();
        SwapBuffers();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        _texPlayer?.Dispose();
        _spriteBatch?.Dispose();
        Engine.Audio.AudioManager.Dispose();
    }
}