namespace Minesweeper
{
    public class ReferenceInt
    {
        /// <inheritdoc />
        public override bool Equals (object obj) => !ReferenceEquals (null, obj) &&
                                                    (ReferenceEquals (this, obj) ||
                                                     obj.GetType () == GetType () && Equals ((ReferenceInt) obj));

        /// <inheritdoc />
        public override int GetHashCode () => Value;

        public int Value { get; }

        public ReferenceInt (int value) => Value = value;

        public static ReferenceInt operator ++ (ReferenceInt value) => new ReferenceInt (value.Value + 1);

        public static bool operator == (ReferenceInt value, int comparison) => value?.Value == comparison;

        public static bool operator != (ReferenceInt value, int comparison) => value?.Value != comparison;

        public static int operator % (ReferenceInt value, int modulo) => value.Value % modulo;

        public static ReferenceInt operator * (ReferenceInt value, int multiply)
            => new ReferenceInt (value.Value * multiply);

        /// <inheritdoc />
        public override string ToString () => Value.ToString ();

        private bool Equals (ReferenceInt other) => Value == other.Value;
    }
}