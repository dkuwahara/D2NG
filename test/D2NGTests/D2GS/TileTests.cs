using D2NG.D2GS;
using D2NG.D2GS.Act;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace D2NGTests.D2GS
{
    public class TileTests
    {
        [Fact]
        public void testTileContainsPoint()
        {
            var tile = new Tile(1128,1104, Area.ROGUE_ENCAMPMENT);
            var point = new Point(5653, 5523);
            Assert.True(tile.Contains(point));
        }
    }
}
