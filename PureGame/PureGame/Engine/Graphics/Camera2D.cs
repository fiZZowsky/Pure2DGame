using OpenTK.Mathematics;

namespace PureGame.Engine.Graphics
{
    public sealed class Camera2D
    {
        public float Width { get; private set; }
        public float Height { get; private set; }

        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Zoom { get; set; } = 1f;

        public Camera2D(int viewportWidth, int viewportHeight)
        {
            Resize(viewportWidth, viewportHeight);
        }

        public void Resize(int w, int h)
        {
            Width = w; Height = h;
        }

        public Matrix4 GetView()
        {
            var t = Matrix4.CreateTranslation(-Position.X, -Position.Y, 0f);
            var s = Matrix4.CreateScale(Zoom, Zoom, 1f);
            return t * s;
        }

        public Matrix4 GetProjection()
        {
            // oś Y w dół (0,0 w lewym górnym rogu)
            return Matrix4.CreateOrthographicOffCenter(0f, Width, Height, 0f, -1f, 1f);
        }

        public Matrix4 GetViewProjection() => GetView() * GetProjection();
    }
}