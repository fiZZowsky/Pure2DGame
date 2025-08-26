namespace PureGame.Engine.Core
{
    public interface IDrawable
    {
        bool Visible { get; }
        int DrawOrder { get; }
        void Draw();
    }
}
