using System.Linq;

using Minesweeper.Ki;
using Minesweeper.Ki.Masks;

using NUnit.Framework;


namespace Tests
{
    [TestFixture]
    public class MaskTests
    {
        [Test]
        public void GeneralNullPointIntersectionTest ()
        {
            var field = new KnownField (3, 3);
            field.Set (0, 0, KnownProperty.Unknown);
            field.Set (0, 1, KnownProperty.Unknown);
            field.Set (0, 2, KnownProperty.Unknown);
            field.Set (1, 0, KnownProperty.Unknown);
            field.Set (1, 1, KnownProperty.Unknown);
            field.Set (1, 2, KnownProperty.Unknown);
            field.Set (2, 0, (KnownProperty) 2);
            field.Set (2, 1, (KnownProperty) 2);
            field.Set (2, 2, (KnownProperty) 1);

            var mask    = EdgeMasks.LeftTop;
            var matches = mask.FindUnknownPointIntersections (field);

            Assert.AreEqual (1, matches.Count ());
        }

        [Test]
        public void SpeicificNullPointIntersectionTest1 ()
        {
            var field = new KnownField (3, 3);
            field.Set (0, 0, KnownProperty.Unknown);
            field.Set (0, 1, KnownProperty.Unknown);
            field.Set (0, 2, KnownProperty.Unknown);
            field.Set (1, 0, (KnownProperty) 1);
            field.Set (1, 1, (KnownProperty) 3);
            field.Set (1, 2, KnownProperty.Flagged);
            field.Set (2, 0, KnownProperty.Empty);
            field.Set (2, 1, (KnownProperty) 2);
            field.Set (2, 2, KnownProperty.Flagged);

            var masks   = EdgeMasks.AllMasks;
            var matches = masks.SelectMany (mask => mask.FindUnknownPointIntersections (field));

            Assert.AreEqual (0, matches.Count ());
        }
    }
}