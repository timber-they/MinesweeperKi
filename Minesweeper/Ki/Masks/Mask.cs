using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Documents;

using Minesweeper.Game;

using static Minesweeper.Game.Coordinate;


namespace Minesweeper.Ki.Masks
{
    public class Mask
    {
        private List <int?> Values { get; }
        private int         SizeX  { get; }
        private int         SizeY  { get; }
        private string      Name   { get; }

        /// <inheritdoc />
        public Mask (List <int?> values, int sizeX, string name = "")
        {
            if (values.Count % sizeX != 0)
                throw new Exception ($"Invalid SizeX ({sizeX})!");
            Values = values;
            SizeX  = sizeX;
            SizeY  = values.Count / SizeX;
            Name   = name;
        }

        public List <Coordinate> FindNullPointIntersections (KnownField field)
        {
            if (Values.Count (i => i == null) != 1)
                throw new Exception ("Multiple Nullpoints!");

            var fin = new List <Coordinate> ();
            for (var x = 0; x <= field.SizeX - SizeX; x++)
                for (var y = 0; y <= field.SizeY - SizeY; y++)
                {
                    var xF = -1;
                    var yF = -1;
                    for (var xI = 0; xI < SizeX; xI++)
                        for (var yI = 0; yI < SizeY; yI++)
                        {
                            var maskValue  = Values [yI * SizeX + xI];
                            var fieldValue = field.Get (x + xI, y + yI);
                            if (fieldValue == null)
                                throw new Exception ("Invalid fieldValue!");
                            if (!Fits (maskValue, fieldValue.Value, field.GetRemainingCount (x + xI, y + yI)))
                                goto cont;

                            if (maskValue != null)
                                continue;
                            xF = x + xI;
                            yF = y + yI;
                        }

                    Debug.WriteLine ($"{Name} matched at {xF} / {yF}");
                    fin.Add (C (xF, yF));
                    cont:;
                }

            return fin;
        }

        private static bool Fits (int? maskValue, KnownProperty fieldValue, int remainingCount) =>
            !maskValue.HasValue && fieldValue == KnownProperty.Unknown ||
            maskValue.HasValue &&
            (maskValue.Value == -2 ||
             maskValue.Value == -1 && fieldValue != KnownProperty.Unknown ||
             maskValue.Value == remainingCount);
    }
}