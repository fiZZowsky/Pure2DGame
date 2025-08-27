using PureGame.Engine.Content;
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

    public static Game Instance { get; private set; } = null!;
    public static Camera2D Camera => Instance._camera;
    public static SpriteBatch SpriteBatch => Instance._spriteBatch;

    public Game(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws)
    {
        Instance = this;
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.1f, 0.1f, 0.12f, 1f);

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        Audio.AudioManager.Initialize();
        ContentManager.Initialize();

        _camera = new Camera2D(Size.X, Size.Y);
        _spriteBatch = new SpriteBatch();
        
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
        
        if (IsExiting) return;

        SceneManager.Update(args.Time);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        SceneManager.Draw();
        SwapBuffers();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        ContentManager.UnloadTexture("Textures/player.png");
        _spriteBatch?.Dispose();
        Engine.Audio.AudioManager.Dispose();
    }
}