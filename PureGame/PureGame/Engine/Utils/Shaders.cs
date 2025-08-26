namespace PureGame.Engine.Graphics
{
    public static class Shaders2D
    {
        public const string SpriteVert = @"
#version 330 core
layout(location = 0) in vec2 aPos;      // bazowy quad [0..1]
layout(location = 1) in vec2 aTex;
uniform mat4 uMVP;
out vec2 vTex;
void main()
{
    vTex = aTex;
    gl_Position = uMVP * vec4(aPos, 0.0, 1.0);
}";

        public const string SpriteFrag = @"
#version 330 core
in vec2 vTex;
out vec4 FragColor;
uniform sampler2D uTexture;
uniform vec4 uColor; // tint (rgba)
void main()
{
    vec4 tex = texture(uTexture, vTex);
    FragColor = tex * uColor;
}";
    }
}