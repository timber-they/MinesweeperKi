using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using static Minesweeper.Game.Coordinate;


namespace Minesweeper.Game
{
    public class Field
    {
        private Random Random { get; }

        public  List <Coordinate> BombCoordinates   { get; }
        private List <Coordinate> FlagCoordinates   { get; }
        private List <Coordinate> OpenedCoordinates { get; }
        private List <Coordinate> CheckedFields     { get; }
        public  int               SizeX             { get; }
        public  int               SizeY             { get; }

        public Field (int x, int y, double threshold)
        {
            BombCoordinates   = new List <Coordinate> ();
            FlagCoordinates   = new List <Coordinate> ();
            OpenedCoordinates = new List <Coordinate> ();
            CheckedFields     = new List <Coordinate> ();

            Random = new Random ();
            for (var xI = 0; xI < x; xI++)
                for (var yI = 0; yI < y; yI++)
                    if (Random.NextDouble () > threshold)
                        BombCoordinates.Add (C (xI, yI));

            SizeX = x;
            SizeY = y;
        }

        public IEnumerable <(Coordinate, LeftResult)> OpenField (int x, int y, MainWindow mainWindow)
        {
            if (OpenedCoordinates.Any (coordinate => coordinate.X == x && coordinate.Y == y) ||
                FlagCoordinates.Any (coordinate => coordinate.X == x && coordinate.Y == y))
                return new [] {(C (x, y), LeftResult.AlreadyOpen)};

            var fin = new List <(Coordinate, LeftResult)> ();

            var count = GetSurroundingBombCount (x, y);
            if (count == 0)
            {
                CheckedFields.Add (C (x, y));
                fin.AddRange (RecursiveCheck (x + 1, y));
                fin.AddRange (RecursiveCheck (x + 1, y - 1));
                fin.AddRange (RecursiveCheck (x + 1, y + 1));
                fin.AddRange (RecursiveCheck (x, y - 1));
                fin.AddRange (RecursiveCheck (x, y + 1));
                fin.AddRange (RecursiveCheck (x - 1, y));
                fin.AddRange (RecursiveCheck (x - 1, y - 1));
                fin.AddRange (RecursiveCheck (x - 1, y + 1));

                IEnumerable <(Coordinate, LeftResult)> RecursiveCheck (int newX, int newY)
                {
                    if (!CheckedFields.All (coordinate => coordinate.X != newX || coordinate.Y != newY) ||
                        GetSurroundingBombCount (newX, newY) < 0)
                        return new List <(Coordinate, LeftResult)> ();

                    return OpenField (newX, newY, mainWindow);
                }
            }

            OpenedCoordinates.Add (C (x, y));
            switch (count)
            {
                case -1:
                    mainWindow.Time = new ReferenceInt (-1);
                    mainWindow.SaveBomb (x, y);
                    MessageBox.Show ("You lost.");
                    mainWindow.SetSize (SizeX, SizeY);
                    break;
                case 0:
                    mainWindow.SaveEmpty (x, y);
                    break;
                case var c:
                    mainWindow.SaveClosing (x, y, c);
                    break;
            }

            fin.Add ((C (x, y), (LeftResult) count));
            return fin;
        }

        private int GetSurroundingBombCount (int x, int y)
        {
            if (x < 0 || y < 0 || x >= SizeX || y >= SizeY)
                return -2;
            if (IsBomb (x, y))
                return -1;
            var count = 0;
            if (IsBomb (x - 1, y))
                count++;
            if (IsBomb (x - 1, y - 1))
                count++;
            if (IsBomb (x, y - 1))
                count++;
            if (IsBomb (x + 1, y - 1))
                count++;
            if (IsBomb (x + 1, y))
                count++;
            if (IsBomb (x + 1, y + 1))
                count++;
            if (IsBomb (x, y + 1))
                count++;
            if (IsBomb (x - 1, y + 1))
                count++;

            return count;
        }

        public int GetRemainingFlagCount () => BombCoordinates.Count - FlagCoordinates.Count;

        public RightResult SetFlag (int x, int y, MainWindow mainWindow)
        {
            if (OpenedCoordinates.Any ((coordinate) => coordinate.X == x && coordinate.Y == y))
                return RightResult.AlreadyOpened;
            for (var i = 0; i < FlagCoordinates.Count; i++)
            {
                var (existingX, existingY) = FlagCoordinates [i];
                if (existingX != x || existingY != y)
                    continue;
                FlagCoordinates.RemoveAt (i);
                mainWindow.RemoveFlag (x, y);
                return RightResult.AlreadyFlagged;
            }

            if (GetRemainingFlagCount () == 0)
                return RightResult.NoFlagsLeft;

            FlagCoordinates.Add (C (x, y));
            var won = BombCoordinates.All (bombCoordinate =>
                                               FlagCoordinates.Any (flagCoordinate =>
                                                                        flagCoordinate.X == bombCoordinate.X &&
                                                                        flagCoordinate.Y == bombCoordinate.Y));

            mainWindow.SaveFlag (x, y);
            mainWindow.SetFlagCount (GetRemainingFlagCount ());

            if (!won)
                return RightResult.Valid;

            mainWindow.Time = new ReferenceInt (-1);
            MessageBox.Show ("You won.");
            mainWindow.SetSize (SizeX, SizeY);

            return RightResult.Won;
        }

        private bool IsBomb (int x, int y) =>
            BombCoordinates.Any (coordinate => coordinate.X == x && coordinate.Y == y);
    }
}