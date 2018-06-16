namespace Minesweeper
{
    public class ReferenceInt
    {
        public int Value { get; set; }

        public ReferenceInt (int value) => Value = value;

        public static ReferenceInt operator ++ (ReferenceInt value) => new ReferenceInt (value.Value + 1);

        public static bool operator == (ReferenceInt value, int comparison) => value?.Value == comparison;

        public static bool operator != (ReferenceInt value, int comparison) => value?.Value != comparison;

        public static int operator % (ReferenceInt value, int modulo) => value.Value % modulo;

        public static ReferenceInt operator * (ReferenceInt value, int multiply)
            => new ReferenceInt (value.Value * multiply);

        /// <inheritdoc />
        public override string ToString () => Value.ToString ();
    }
}