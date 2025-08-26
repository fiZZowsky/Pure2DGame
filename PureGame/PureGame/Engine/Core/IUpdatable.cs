namespace PureGame.Engine.Core
{
    public interface IUpdatable
    {
        bool Enabled { get; }
        int UpdateOrder { get; }
        void Update(double dt);
    }
}