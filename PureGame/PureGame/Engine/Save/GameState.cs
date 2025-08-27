using OpenTK.Mathematics;

namespace PureGame.Gameplay
{
    public class GameState
    {
        public string LevelId { get; set; } = "level_1";
        public float PlayerX { get; set; }
        public float PlayerY { get; set; }
        public int Health { get; set; } = 100;
        public string[] Inventory { get; set; } = Array.Empty<string>();

        // convenience
        public Vector2 PlayerPos
        {
            get => new(PlayerX, PlayerY);
            set { PlayerX = value.X; PlayerY = value.Y; }
        }
    }
}