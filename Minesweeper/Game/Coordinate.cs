namespace Minesweeper.Game
{
    public class Coordinate
    {
        public int X { get; }
        public int Y { get; }

        private Coordinate (int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Coordinate C (int x, int y) => new Coordinate (x, y);

        public void Deconstruct (out int x, out int y)
        {
            x = X;
            y = Y;
        }

        /// <inheritdoc />
        public override string ToString () => $"({X} / {Y})";
    }
}