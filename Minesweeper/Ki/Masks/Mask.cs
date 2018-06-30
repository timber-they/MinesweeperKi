using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Minesweeper.Game;

using static Minesweeper.Game.Coordinate;
// ReSharper disable BadListLineBreaks
// ReSharper disable BadListLineBreaks
// ReSharper disable UnusedMember.Global


namespace Minesweeper.Ki.Masks
{
    public class Mask
    {
        private List <MaskValue> Values { get; }
        private int              SizeX  { get; }
        private int              SizeY  { get; }
        private string           Name   { get; }

        /// <inheritdoc />
        public Mask (List <MaskValue> values, int sizeX, string name = "")
        {
            if (values.Count % sizeX != 0)
                throw new Exception ($"Invalid SizeX ({sizeX})!");
            Values = values;
            SizeX  = sizeX;
            SizeY  = values.Count / SizeX;
            Name   = name;
        }

        public IEnumerable <Coordinate> FindUnknownPointIntersections (KnownField field)
        {
            if (Values.Count (i => i == MaskValue.Unknown) != 1)
                throw new Exception ("Invalid UnknownPoints count!");

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

                            if (maskValue != MaskValue.Unknown)
                                continue;
                            xF = x + xI;
                            yF = y + yI;
                        }

                    Debug.WriteLine ($"{Name} matched at {xF} / {yF}");
                    yield return C (xF, yF);
                    cont:;
                }
        }

        public static readonly Mask LeftTop = new Mask (new List <MaskValue>
        {
            MaskValue.Unknown, MaskValue.NotUnknown, MaskValue.NotUnknown,
            MaskValue.Anything, (MaskValue) 2, MaskValue.NotUnknown,
            MaskValue.Anything, (MaskValue) 1, MaskValue.NotUnknown
        }, 3, nameof (LeftTop));

        private static bool Fits (
            MaskValue     maskValue,
            KnownProperty fieldValue,
            int           remainingCount)
            => maskValue == MaskValue.Unknown && fieldValue == KnownProperty.Unknown ||
               maskValue == MaskValue.Anything ||
               maskValue == MaskValue.NotUnknown && fieldValue != KnownProperty.Unknown ||
               (int) maskValue == remainingCount;
    }
}