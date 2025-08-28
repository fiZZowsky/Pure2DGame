using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PureGame.Engine.Graphics;
using PureGame.Engine.Input;

namespace PureGame.Engine.UI
{
    public sealed class LabeledCheckbox : IDisposable
    {
        private readonly Texture2D _whiteTex;
        private readonly Texture2D _labelTex;
        public Vector2 Position;
        public Vector2 BoxSize = new(32, 32);
        public float LabelSpacing = 8f;
        public bool Checked { get; private set; }

        public LabeledCheckbox(Texture2D whiteTex, Vector2 position, string text, bool initialState = false)
        {
            _whiteTex = whiteTex;
            Position = position;
            Checked = initialState;
            _labelTex = TextRenderer.Render(text, 24f, new Vector4(0f, 0f, 0f, 1f));
        }

        public Vector2 TotalSize => new(BoxSize.X + LabelSpacing + _labelTex.Width, Math.Max(BoxSize.Y, _labelTex.Height));

        public bool Update(Vector2 mouse)
        {
            var size = TotalSize;
            bool inside = mouse.X >= Position.X && mouse.X <= Position.X + size.X &&
                           mouse.Y >= Position.Y && mouse.Y <= Position.Y + size.Y;
            if (inside && InputManager.IsMouseButtonPressed(MouseButton.Left))
            {
                Checked = !Checked;
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch sb)
        {
            // border
            sb.Draw(_whiteTex, Position, BoxSize, 0f, new Vector4(0f, 0f, 0f, 1f));
            sb.Draw(_whiteTex, Position + new Vector2(2, 2), BoxSize - new Vector2(4, 4), 0f, Vector4.One);
            if (Checked)
                sb.Draw(_whiteTex, Position + new Vector2(6, 6), BoxSize - new Vector2(12, 12), 0f, new Vector4(0f, 0f, 0f, 1f));

            var labelPos = Position + new Vector2(BoxSize.X + LabelSpacing, (BoxSize.Y - _labelTex.Height) * 0.5f);
            sb.Draw(_labelTex, labelPos, new Vector2(_labelTex.Width, _labelTex.Height));
        }

        public void Dispose()
        {
            _labelTex.Dispose();
        }
    }
}
