using System;
using System.Collections.Generic;
using System.Linq;

using Minesweeper.Game;
using Minesweeper.Ki.Masks;

using static Minesweeper.Game.Coordinate;


namespace Minesweeper.Ki
{
    public class KnownField
    {
        private List <KnownProperty> Field { get; }
        public  int                  SizeX { get; }
        public  int                  SizeY { get; }

        public KnownField (int sizeX, int sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            Field = Enumerable.Repeat (KnownProperty.Unknown, SizeX * SizeY).ToList ();
        }

        public void Set (int x, int y, KnownProperty property) => Set (x + y * SizeX, property);

        public void Set (int i, KnownProperty property) => Field [i] = property;

        public KnownProperty? Get (int x, int y)
            => x < 0 || y < 0 || x >= SizeX || y >= SizeY ? null : Get (x + y * SizeX);

        public KnownProperty? Get (int i)
            => i < 0 || i >= Field.Count ? (KnownProperty?) null : Field [i];

        /// <summary>
        /// Evaluates the coordinates for the specified Index in the field
        /// </summary>
        public Coordinate GetCoordinates (int i)
            => C (i % SizeX, i / SizeX);

        private IEnumerable <Coordinate> FindFullClose ()
        {
            for (var i = 0; i < Field.Count; i++)
            {
                if ((int) Field [i] < 1)
                    continue;

                var (x, y) = GetCoordinates (i);
                var close = GetCloseFields (x, y);

                var count = close.Count (property => property == KnownProperty.Flagged);
                if (count == (int) Field [i])
                    yield return C (x, y);
            }
        }

        private IEnumerable <Coordinate> FindBombsAtSurroundingCloses ()
        {
            for (var i = 0; i < Field.Count; i++)
            {
                if ((int) Field [i] < 1)
                    continue;

                var (x, y) = GetCoordinates (i);
                var close = GetCloseFields (x, y);

                var flaggedCount  = close.Count (property => property == KnownProperty.Flagged);
                var leftBombCount = (int) Field [i] - flaggedCount;

                var unknownCount = close.Count (property => property == KnownProperty.Unknown);
                if (unknownCount != leftBombCount)
                    continue;

                yield return C (x, y);
            }
        }

        public Coordinate FindSaveSpot () => GetFirstClosingUnknown (FindFullClose ());

        public Coordinate FindSureSpot () =>
            GetFirstClosingUnknown (FindBombsAtSurroundingCloses ()) ?? GetSureFieldAtTwosOnEdge ();

        private Coordinate GetFirstClosingUnknown (IEnumerable <Coordinate> centers)
        {
            foreach (var (x, y) in centers)
            {
                var close = GetCloseFields (x, y);
                for (var j = 0; j < close.Count; j++)
                    if (close [j] == KnownProperty.Unknown)
                        return GetCloseCoordinate (x, y, j);
            }

            return null;
        }

        public int GetRemainingCount (int x, int y)
        {
            var count = (int) (Get (x, y) ?? 0);
            if (count < 1)
                return 0;
            var close        = GetCloseFields (x, y);
            var flaggedCount = close.Count (property => property == KnownProperty.Flagged);
            return count - flaggedCount;
        }

        private Coordinate GetSureFieldAtTwosOnEdge ()
        {
            var masks              = EdgeMasks.AllMasks;
            var matchedCoordinates = masks.SelectMany (mask => mask.FindUnknownPointIntersections (this));

            return matchedCoordinates.FirstOrDefault ();
        }

        /// <summary>
        /// 0 1 2<br />
        /// 3 . 4<br />
        /// 5 6 7
        /// </summary>
        private List <KnownProperty?> GetCloseFields (int x, int y) =>
            new List <KnownProperty?> (8)
            {
                Get (x - 1, y - 1),
                Get (x, y - 1),
                Get (x + 1, y - 1),
                Get (x - 1, y),
                Get (x + 1, y),
                Get (x - 1, y + 1),
                Get (x, y + 1),
                Get (x + 1, y + 1),
            };

        private static Coordinate GetCloseCoordinate (int xBase, int yBase, int index)
        {
            switch (index)
            {
                case 0:
                    return C (xBase - 1, yBase - 1);
                case 1:
                    return C (xBase, yBase - 1);
                case 2:
                    return C (xBase + 1, yBase - 1);
                case 3:
                    return C (xBase - 1, yBase);
                case 4:
                    return C (xBase + 1, yBase);
                case 5:
                    return C (xBase - 1, yBase + 1);
                case 6:
                    return C (xBase, yBase + 1);
                case 7:
                    return C (xBase + 1, yBase + 1);
                default:
                    throw new IndexOutOfRangeException ();
            }
        }
    }
}