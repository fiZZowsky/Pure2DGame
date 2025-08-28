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

        private bool _mouseDownPrev;
        private bool _pressStartedHere;

        public Button(Texture2D texture, Vector2 position, Vector2 size)
        {
            _texture = texture;
            Position = position;
            Size = size;
        }

        private static bool PointInRect(Vector2 p, Vector2 tl, Vector2 size)
        {
            return p.X >= tl.X && p.X < tl.X + size.X &&
                   p.Y >= tl.Y && p.Y < tl.Y + size.Y;
        }

        public bool Update(Vector2 mouse)
        {
            Hovered = PointInRect(mouse, Position, Size);
            bool isDown = InputManager.IsMouseButtonDown(MouseButton.Left);
            if (isDown && !_mouseDownPrev)
                _pressStartedHere = Hovered;

            bool clicked = (_mouseDownPrev && !isDown) && _pressStartedHere && Hovered;
            if (!isDown)
                _pressStartedHere = false;

            _mouseDownPrev = isDown;
            return clicked;
        }

        public void Draw(SpriteBatch sb)
        {
            float s = Hovered ? HoverScale : 1f;
            Vector2 size = Size * s;
            Vector2 pos = Position - (size - Size) * 0.5f;
            sb.Draw(_texture, pos, size);
        }
    }
}