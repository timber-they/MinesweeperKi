using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


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

        public (int, int) GetCoordinates (int i)
            => (i % SizeX, i / SizeX);

        public List <(int, int)> FindFullClose ()
        {
            var fin = new List <(int, int)> ();
            for (var i = 0; i < Field.Count; i++)
            {
                if ((int) Field [i] < 1)
                    continue;
                var (x, y) = GetCoordinates (i);
                var close = GetCloseFields (x, y);
                var count = close.Count (property => property == KnownProperty.Flagged);
                if (count == (int) Field [i])
                    fin.Add ((x, y));
            }

            return fin;
        }

        public (int, int)? FindSaveSpot ()
        {
            var fullClose = FindFullClose ();
            foreach (var (x, y) in fullClose)
            {
                var closeFields = GetCloseFields (x, y);
                for (var i = 0; i < closeFields.Count; i++)
                    if (closeFields [i] == KnownProperty.Unknown)
                        return GetCloseCoordinate (x, y, closeFields, i);
            }

            return null;
        }

        public (int, int)? FindSureSpot ()
        {
            for (var i = 0; i < Field.Count; i++)
            {
                if ((int) Field [i] < 1)
                    continue;
                var (x, y) = GetCoordinates (i);
                var close         = GetCloseFields (x, y);
                var unknownCount  = close.Count (property => property == KnownProperty.Unknown);
                var flaggedCount  = close.Count (property => property == KnownProperty.Flagged);
                var leftBombCount = (int) Field [i] - flaggedCount;
                if (unknownCount != leftBombCount)
                    continue;
                for (var j = 0; j < close.Count; j++)
                    if (close [j] == KnownProperty.Unknown)
                        return GetCloseCoordinate (x, y, close, j);
            }

            return GetSureFieldAtTwosOnEdge ();
        }

        private List <(int, int)> GetTwos ()
        {
            var fin = new List <(int, int)> ();

            for (var i = 0; i < Field.Count; i++)
            {
                var (x, y) = GetCoordinates (i);
                if (GetRemainingCount (x, y) == 2)
                    fin.Add ((x, y));
            }

            return fin;
        }

        private int GetRemainingCount ((int x, int y) coordinates) => GetRemainingCount (coordinates.x, coordinates.y);

        private int GetRemainingCount (int x, int y)
        {
            var count = (int) (Get (x, y) ?? 0);
            if (count < 1)
                return 0;
            var close        = GetCloseFields (x, y);
            var flaggedCount = close.Count (property => property == KnownProperty.Flagged);
            return count - flaggedCount;
        }

        private (int, int)? GetSureFieldAtTwosOnEdge ()
        {
            var twos = GetTwos ();
            Debug.WriteLine ("Twos: " + string.Join (", ", twos));
            foreach (var two in twos)
            {
                var (x, y) = two;
                var close = GetCloseFields (x, y);
                if (
                    close [0] == KnownProperty.Unknown &&
                    close [1] == KnownProperty.Unknown &&
                    close [2] == KnownProperty.Unknown &&
                    close [3] != KnownProperty.Unknown &&
                    close [4] != KnownProperty.Unknown &&
                    close [5] != KnownProperty.Unknown &&
                    close [6] != KnownProperty.Unknown &&
                    close [7] != KnownProperty.Unknown)
                {
                    //Top edge
                    Debug.WriteLine ($"Found top edge {two}");
                    if (GetRemainingCount (GetCloseCoordinate (x, y, close, 3)) == 1)
                        return GetCloseCoordinate (x, y, close, 2);
                    if (GetRemainingCount (GetCloseCoordinate (x, y, close, 4)) == 1)
                        return GetCloseCoordinate (x, y, close, 0);
                }
                else if (
                    close [0] == KnownProperty.Unknown &&
                    close [1] != KnownProperty.Unknown &&
                    close [2] != KnownProperty.Unknown &&
                    close [3] == KnownProperty.Unknown &&
                    close [4] != KnownProperty.Unknown &&
                    close [5] == KnownProperty.Unknown &&
                    close [6] != KnownProperty.Unknown &&
                    close [7] != KnownProperty.Unknown)
                {
                    //Left edge
                    Debug.WriteLine ($"Found left edge {two}");
                    if (GetRemainingCount (GetCloseCoordinate (x, y, close, 1)) == 1)
                        return GetCloseCoordinate (x, y, close, 5);
                    if (GetRemainingCount (GetCloseCoordinate (x, y, close, 6)) == 1)
                        return GetCloseCoordinate (x, y, close, 0);
                }
                else if (
                    close [0] != KnownProperty.Unknown &&
                    close [1] != KnownProperty.Unknown &&
                    close [2] == KnownProperty.Unknown &&
                    close [3] != KnownProperty.Unknown &&
                    close [4] == KnownProperty.Unknown &&
                    close [5] != KnownProperty.Unknown &&
                    close [6] != KnownProperty.Unknown &&
                    close [7] == KnownProperty.Unknown)
                {
                    //Right edge
                    Debug.WriteLine ($"Found right edge {two}");
                    if (GetRemainingCount (GetCloseCoordinate (x, y, close, 6)) == 1)
                        return GetCloseCoordinate (x, y, close, 2);
                    if (GetRemainingCount (GetCloseCoordinate (x, y, close, 1)) == 1)
                        return GetCloseCoordinate (x, y, close, 7);
                }
                else if (
                    close [0] != KnownProperty.Unknown &&
                    close [1] != KnownProperty.Unknown &&
                    close [2] != KnownProperty.Unknown &&
                    close [3] != KnownProperty.Unknown &&
                    close [4] != KnownProperty.Unknown &&
                    close [5] == KnownProperty.Unknown &&
                    close [6] == KnownProperty.Unknown &&
                    close [7] == KnownProperty.Unknown)
                {
                    //Bottom edge
                    Debug.WriteLine ($"Found bottom edge {two}");
                    if (GetRemainingCount (GetCloseCoordinate (x, y, close, 3)) == 1)
                        return GetCloseCoordinate (x, y, close, 7);
                    if (GetRemainingCount (GetCloseCoordinate (x, y, close, 4)) == 1)
                        return GetCloseCoordinate (x, y, close, 5);
                }
            }

            return null;
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

        private (int, int) GetCloseCoordinate <T> (int xBase, int yBase, List <T> close, int index)
        {
            switch (index)
            {
                case 0:
                    return (xBase - 1, yBase - 1);
                case 1:
                    return (xBase, yBase - 1);
                case 2:
                    return (xBase + 1, yBase - 1);
                case 3:
                    return (xBase - 1, yBase);
                case 4:
                    return (xBase + 1, yBase);
                case 5:
                    return (xBase - 1, yBase + 1);
                case 6:
                    return (xBase, yBase + 1);
                case 7:
                    return (xBase + 1, yBase + 1);
                default:
                    throw new IndexOutOfRangeException ();
            }
        }
    }
}