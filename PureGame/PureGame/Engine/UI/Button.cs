using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PureGame.Engine.Graphics;
using PureGame.Engine.Input;

namespace PureGame.Engine.UI
{
    public sealed class Button
    {
        private readonly Texture2D _texture;
        public Vector2 Position;
        public Vector2 Size;
        public float HoverScale = 1.1f;
        public bool Hovered { get; private set; }

        public Button(Texture2D texture, Vector2 position, Vector2 size)
        {
            _texture = texture;
            Position = position;
            Size = size;
        }

        public bool Update(Vector2 mouse)
        {
            float scale = Hovered ? HoverScale : 1f;
            Vector2 size = Size * scale;
            Vector2 pos = Position - (size - Size) * 0.5f;
            bool inside = mouse.X >= pos.X && mouse.X <= pos.X + size.X &&
                          mouse.Y >= pos.Y && mouse.Y <= pos.Y + size.Y;
            Hovered = inside;
            return inside && InputManager.IsMouseButtonPressed(MouseButton.Left);
        }

        public void Draw(SpriteBatch sb)
        {
            float scale = Hovered ? HoverScale : 1f;
            Vector2 size = Size * scale;
            Vector2 pos = Position - (size - Size) * 0.5f;
            sb.Draw(_texture, pos, size);
        }
    }
}