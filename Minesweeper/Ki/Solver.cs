﻿using System;


namespace Minesweeper.Ki
{
    public class Solver
    {
        private KnownField Field  { get; }
        private Random     Random { get; }

        public Solver (MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            Field      = new KnownField (MainWindow.SizeX, MainWindow.SizeY);

            Random = new Random ();
        }

        private MainWindow MainWindow { get; set; }

        public void TakeAction ()
        {
            var sureSpot = Field.FindSureSpot ();
            if (sureSpot != null)
            {
                var (x, y) = sureSpot.Value;
                SetFlag (x, y);
                return;
            }

            var saveSpot = Field.FindSaveSpot ();
            if (saveSpot != null)
            {
                var (x, y) = saveSpot.Value;
                SetField (x, y);
                return;
            }

            SetRandom ();
        }

        private void SetRandom ()
        {
            int r;
            do
                r = Random.Next (0, Field.SizeX * Field.SizeY);
            while (Field.Get (r) != KnownProperty.Unknown);

            SetField (r);
        }

        private void SetField (int initalX, int initalY)
        {
            var results = MainWindow.LeftClickOnField (initalX, initalY);

            foreach (var resultTuple in results)
            {
                var ((x, y), result) = resultTuple;
                switch (result)
                {
                    case LeftResult.Empty:
                        Field.Set (x, y, KnownProperty.Empty);
                        break;
                    case LeftResult.Bomb:
                        Field.Set (x, y, KnownProperty.Exploded);
                        break;
                    case LeftResult.AlreadyOpen:
                        break;
                    default:
                        var bombCount = (int) result;
                        Field.Set (x, y, (KnownProperty) bombCount);
                        break;
                }
            }
        }

        private void SetField (int i)
        {
            var (x, y) = Field.GetCoordinates (i);
            SetField (x, y);
        }

        private KnownProperty SetFlag (int i)
        {
            var (x, y) = Field.GetCoordinates (i);
            return SetFlag (x, y);
        }

        private KnownProperty SetFlag (int x, int y)
        {
            var result = MainWindow.RightClickOnField (x, y);

            switch (result)
            {
                case RightResult.Won:
                case RightResult.Valid:
                    Field.Set (x, y, KnownProperty.Flagged);
                    return KnownProperty.Flagged;
                case RightResult.AlreadyFlagged:
                    Field.Set (x, y, KnownProperty.Unknown);
                    return KnownProperty.Unknown;
                case RightResult.AlreadyOpened:
                    throw new Exception ("Invalid observation");
                case RightResult.NoFlagsLeft:
                    throw new Exception ("Invalid observation");
                default:
                    throw new ArgumentOutOfRangeException ();
            }
        }
    }
}