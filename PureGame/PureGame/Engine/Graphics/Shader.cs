using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PureGame.Engine.Graphics
{
    public sealed class Shader : IDisposable
    {
        public int Handle { get; private set; }
        private readonly Dictionary<string, int> _uniformLocations = new();

        public Shader(string vertexSrc, string fragmentSrc)
        {
            int vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, vertexSrc);
            GL.CompileShader(vs);
            GL.GetShader(vs, ShaderParameter.CompileStatus, out int compiledVS);
            if (compiledVS == 0) throw new Exception(GL.GetShaderInfoLog(vs));

            int fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, fragmentSrc);
            GL.CompileShader(fs);
            GL.GetShader(fs, ShaderParameter.CompileStatus, out int compiledFS);
            if (compiledFS == 0) throw new Exception(GL.GetShaderInfoLog(fs));

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vs);
            GL.AttachShader(Handle, fs);
            GL.LinkProgram(Handle);
            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int linked);
            if (linked == 0) throw new Exception(GL.GetProgramInfoLog(Handle));

            GL.DetachShader(Handle, vs);
            GL.DetachShader(Handle, fs);
            GL.DeleteShader(vs);
            GL.DeleteShader(fs);

            // cache uniform locations
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int count);
            for (int i = 0; i < count; i++)
            {
                var name = GL.GetActiveUniform(Handle, i, out _, out _);
                int loc = GL.GetUniformLocation(Handle, name);
                _uniformLocations[name] = loc;
            }
        }

        public void Use() => GL.UseProgram(Handle);
        public void Dispose() { if (Handle != 0) GL.DeleteProgram(Handle); Handle = 0; }

        private int U(string name) => _uniformLocations.TryGetValue(name, out var loc) ? loc : -1;

        public void SetMatrix4(string name, Matrix4 value) => GL.UniformMatrix4(U(name), false, ref value);
        public void SetVector4(string name, Vector4 value) => GL.Uniform4(U(name), value);
        public void SetInt(string name, int value) => GL.Uniform1(U(name), value);
        public void SetFloat(string name, float value) => GL.Uniform1(U(name), value);
    }
}
