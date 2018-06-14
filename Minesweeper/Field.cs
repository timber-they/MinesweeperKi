using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;


namespace Minesweeper
{
    public class Field
    {
        private Random _random;

        public  List <(int, int)> BombCoordinates   { get; }
        private List <(int, int)> FlagCoordinates   { get; }
        private List <(int, int)> OpenedCoordinates { get; }
        private List <(int, int)> CheckedFields     { get; }
        public int               SizeX             { get; }
        public int               SizeY             { get; }

        public Field (int x, int y, double threshold)
        {
            BombCoordinates   = new List <(int, int)> ();
            FlagCoordinates   = new List <(int, int)> ();
            OpenedCoordinates = new List <(int, int)> ();
            CheckedFields     = new List <(int, int)> ();

            _random = new Random ();
            for (var xI = 0; xI < x; xI++)
                for (var yI = 0; yI < y; yI++)
                    if (_random.NextDouble () > threshold)
                        BombCoordinates.Add ((xI, yI));

            SizeX = x;
            SizeY = y;
        }

        /// <summary>
        /// Returns 0 for empty field, -1 for bomb, -2 if the field was already open and anything else as the close bomb count
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mainWindow"></param>
        /// <param name="sender"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int OpenField (int x, int y, MainWindow mainWindow)
        {
            if (OpenedCoordinates.Any (tuple => tuple.Item1 == x && tuple.Item2 == y) ||
                FlagCoordinates.Any (tuple => tuple.Item1 == x && tuple.Item2 == y))
                return -2;

            var count = GetSurroundingBombCount (x, y);
            if (count == 0)
            {
                CheckedFields.Add ((x, y));
                RecursiveCheck (x + 1, y);
                RecursiveCheck (x + 1, y - 1);
                RecursiveCheck (x + 1, y + 1);
                RecursiveCheck (x, y - 1);
                RecursiveCheck (x, y + 1);
                RecursiveCheck (x - 1, y);
                RecursiveCheck (x - 1, y - 1);
                RecursiveCheck (x - 1, y + 1);

                void RecursiveCheck (int newX, int newY)
                {
                    if (!CheckedFields.All (tuple => tuple.Item1 != newX || tuple.Item2 != newY) ||
                        GetSurroundingBombCount (newX, newY) < 0)
                        return;
                    Debug.IndentLevel++;

                    OpenField (newX, newY, mainWindow);

                    Debug.IndentLevel--;
                }
            }

            OpenedCoordinates.Add ((x, y));
            Debug.WriteLine ($"Opened field ({x}, {y})");
            switch (count)
            {
                case -1:
                    mainWindow.Time = -1;
                    Debug.WriteLine ("    Lost");
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

            return count;
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

        /// <summary>
        /// Returns 1 if the player has won, -1 if there was a flag already, -2 if the field was already opened and 0 elsewise
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int SetFlag (int x, int y)
        {
            if (OpenedCoordinates.Any (tuple => tuple.Item1 == x && tuple.Item2 == y))
                return -2;
            for (var i = 0; i < FlagCoordinates.Count; i++)
            {
                var tuple = FlagCoordinates [i];
                if (tuple.Item1 != x || tuple.Item2 != y)
                    continue;
                FlagCoordinates.RemoveAt (i);
                return -1;
            }

            if (GetRemainingFlagCount () == 0)
                return -3;

            FlagCoordinates.Add ((x, y));
            return BombCoordinates.All (bombCoordinate =>
                                            FlagCoordinates.Any (flagCoordinate =>
                                                                     flagCoordinate.Item1 == bombCoordinate.Item1 &&
                                                                     flagCoordinate.Item2 == bombCoordinate.Item2))
                       ? 1
                       : 0;
        }

        private bool IsBomb (int x, int y) => BombCoordinates.Any (tuple => tuple.Item1 == x && tuple.Item2 == y);
    }
}