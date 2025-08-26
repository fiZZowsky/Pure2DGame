using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PureGame.Engine.Graphics
{
    public sealed class SpriteBatch : IDisposable
    {
        private readonly Shader _shader;
        private int _vao, _vbo, _ebo;
        private Matrix4 _vp;

        // statyczny quad: pos(0..1), uv(0..1)
        private static readonly float[] Vert =
        {
            //  aPos.x aPos.y  aTex.x aTex.y
             0f, 0f,  0f, 0f,
             1f, 0f,  1f, 0f,
             1f, 1f,  1f, 1f,
             0f, 1f,  0f, 1f,
        };
        private static readonly uint[] Ind = { 0, 1, 2, 2, 3, 0 };

        public SpriteBatch()
        {
            _shader = new Shader(Shaders2D.SpriteVert, Shaders2D.SpriteFrag);

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, Vert.Length * sizeof(float), Vert, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Ind.Length * sizeof(uint), Ind, BufferUsageHint.StaticDraw);

            int stride = 4 * sizeof(float);
            GL.EnableVertexAttribArray(0); // aPos
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(1); // aTex
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 2 * sizeof(float));

            GL.BindVertexArray(0);
        }

        public void Begin(Camera2D camera)
        {
            _vp = camera.GetViewProjection();

            _shader.Use();
            _shader.SetInt("uTexture", 0); // sampler = 0
        }

        public void Draw(Texture2D tex, Vector2 position, Vector2 size, float rotationRad = 0f, Vector4? color = null)
        {
            // model: kolejność -> translate * rotate * scale * anchor(-0.5)
            // obrót wokół środka sprite’a
            var translate = Matrix4.CreateTranslation(position.X + size.X * 0.5f, position.Y + size.Y * 0.5f, 0f);
            var rotate = rotationRad != 0f ? Matrix4.CreateRotationZ(rotationRad) : Matrix4.Identity;
            var scale = Matrix4.CreateScale(size.X, size.Y, 1f);
            var anchor = Matrix4.CreateTranslation(-0.5f, -0.5f, 0f); // quad 0..1 -> środek (0.5,0.5)

            var model = anchor * scale * rotate * translate;
            var mvp = model * _vp;

            _shader.SetMatrix4("uMVP", mvp);
            _shader.SetVector4("uColor", color ?? Vector4.One);

            tex.Bind(TextureUnit.Texture0);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void End()
        {
            // na razie nic (gdy dodasz batching, tu pójdzie flush)
        }

        public void Dispose()
        {
            _shader?.Dispose();
            if (_ebo != 0) GL.DeleteBuffer(_ebo);
            if (_vbo != 0) GL.DeleteBuffer(_vbo);
            if (_vao != 0) GL.DeleteVertexArray(_vao);
        }
    }
}
