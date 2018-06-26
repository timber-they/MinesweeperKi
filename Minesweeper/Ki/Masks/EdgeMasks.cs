using System.Collections.Generic;
using System.Diagnostics;


// ReSharper disable BadListLineBreaks


namespace Minesweeper.Ki.Masks
{
    public static class EdgeMasks
    {
        public static readonly Mask LeftTop = new Mask (new List <MaskValue>
        {
            MaskValue.Unknown, MaskValue.NotUnknown, MaskValue.NotUnknown,
            MaskValue.Anything, (MaskValue) 2, MaskValue.NotUnknown,
            MaskValue.Anything, (MaskValue) 1, MaskValue.NotUnknown
        }, 3, nameof (LeftTop));

        public static readonly Mask LeftBottom = new Mask (new List <MaskValue>
        {
            MaskValue.Anything, (MaskValue) 1, MaskValue.NotUnknown,
            MaskValue.Anything, (MaskValue) 2, MaskValue.NotUnknown,
            MaskValue.Unknown, MaskValue.NotUnknown, MaskValue.NotUnknown
        }, 3, nameof (LeftBottom));

        public static readonly Mask RightTop = new Mask (new List <MaskValue>
        {
            MaskValue.NotUnknown, MaskValue.NotUnknown, MaskValue.Unknown,
            MaskValue.NotUnknown, (MaskValue) 2, MaskValue.Anything,
            MaskValue.NotUnknown, (MaskValue) 1, MaskValue.Anything
        }, 3, nameof (RightTop));

        public static readonly Mask RightBottom = new Mask (new List <MaskValue>
        {
            MaskValue.NotUnknown, (MaskValue) 1, MaskValue.Anything,
            MaskValue.NotUnknown, (MaskValue) 2, MaskValue.Anything,
            MaskValue.NotUnknown, MaskValue.NotUnknown, MaskValue.Unknown
        }, 3, nameof (RightBottom));

        public static readonly Mask TopLeft = new Mask (new List <MaskValue>
        {
            MaskValue.Unknown, MaskValue.Anything, MaskValue.Anything,
            MaskValue.NotUnknown, (MaskValue) 2, (MaskValue) 1,
            MaskValue.NotUnknown, MaskValue.NotUnknown, MaskValue.NotUnknown
        }, 3, nameof (TopLeft));

        public static readonly Mask TopRight = new Mask (new List <MaskValue>
        {
            MaskValue.Anything, MaskValue.Anything, MaskValue.Unknown,
            (MaskValue) 1, (MaskValue) 2, MaskValue.NotUnknown,
            MaskValue.NotUnknown, MaskValue.NotUnknown, MaskValue.NotUnknown
        }, 3, nameof (TopRight));

        public static readonly Mask BottomLeft = new Mask (new List <MaskValue>
        {
            MaskValue.NotUnknown, MaskValue.NotUnknown, MaskValue.NotUnknown,
            MaskValue.NotUnknown, (MaskValue) 2, (MaskValue) 1,
            MaskValue.Unknown, MaskValue.Anything, MaskValue.Anything
        }, 3, nameof (BottomLeft));

        public static readonly Mask BottomRight = new Mask (new List <MaskValue>
        {
            MaskValue.NotUnknown, MaskValue.NotUnknown, MaskValue.NotUnknown,
            (MaskValue) 1, (MaskValue) 2, MaskValue.NotUnknown,
            MaskValue.Anything, MaskValue.Anything, MaskValue.Unknown
        }, 3, nameof (BottomRight));

        public static List <Mask> AllMasks =
            new List <Mask> {LeftTop, LeftBottom, RightTop, RightBottom, TopLeft, TopRight, BottomLeft, BottomRight};
    }
}