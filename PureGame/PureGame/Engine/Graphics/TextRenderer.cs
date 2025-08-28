using OpenTK.Mathematics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace PureGame.Engine.Graphics
{
    public static class TextRenderer
    {
        public static Texture2D Render(string text, float fontSize, Vector4 color)
        {
            var font = SystemFonts.CreateFont("Arial", fontSize);
            var measureOpts = new TextOptions(font);
            var size = TextMeasurer.MeasureSize(text, measureOpts);
            using var img = new Image<Rgba32>((int)System.Math.Ceiling(size.Width), (int)System.Math.Ceiling(size.Height));
            var rgba = new Rgba32(
                (byte)(color.X * 255f),
                (byte)(color.Y * 255f),
                (byte)(color.Z * 255f),
                (byte)(color.W * 255f));
            var drawOpts = new RichTextOptions(font);
            img.Mutate(ctx => ctx.DrawText(drawOpts, text, rgba));
            return new Texture2D(img);
        }
    }
}