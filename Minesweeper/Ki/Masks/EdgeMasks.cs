using System.Collections.Generic;
using System.Diagnostics;


// ReSharper disable BadListLineBreaks


namespace Minesweeper.Ki.Masks
{
    public static class EdgeMasks
    {
        public static readonly Mask LeftTop = new Mask (new List <int?>
        {
            null, -1, -1,
            -2, 2, -1,
            -2, 1, -1
        }, 3, nameof (LeftTop));

        public static readonly Mask LeftBottom = new Mask (new List <int?>
        {
            -2, 1, -1,
            -2, 2, -1,
            null, -1, -1
        }, 3, nameof (LeftBottom));

        public static readonly Mask RightTop = new Mask (new List <int?>
        {
            -1, -1, null,
            -1, 2, -2,
            -1, 1, -2
        }, 3, nameof (RightTop));

        public static readonly Mask RightBottom = new Mask (new List <int?>
        {
            -1, 1, -2,
            -1, 2, -2,
            -1, -1, null
        }, 3, nameof (RightBottom));

        public static readonly Mask TopLeft = new Mask (new List <int?>
        {
            null, -2, -2,
            -1, 2, 1,
            -1, -1, -1
        }, 3, nameof (TopLeft));

        public static readonly Mask TopRight = new Mask (new List <int?>
        {
            -2, -2, null,
            1, 2, -1,
            -1, -1, -1
        }, 3, nameof (TopRight));

        public static readonly Mask BottomLeft = new Mask (new List <int?>
        {
            -1, -1, -1,
            -1, 2, 1,
            null, -2, -2
        }, 3, nameof (BottomLeft));

        public static readonly Mask BottomRight = new Mask (new List <int?>
        {
            -1, -1, -1,
            1, 2, -1,
            -2, -2, null
        }, 3, nameof (BottomRight));

        public static List <Mask> AllMasks =
            new List <Mask> {LeftTop, LeftBottom, RightTop, RightBottom, TopLeft, TopRight, BottomLeft, BottomRight};
    }
}