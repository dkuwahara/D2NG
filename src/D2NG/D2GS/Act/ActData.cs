using System;
using System.Collections.Generic;
using System.Text;
using D2NG.D2GS.Act.Packet;

namespace D2NG.D2GS.Act
{
    class ActData
    {
        byte Act { get; set; }
        uint MapId { get; set; }
        Area Area { get; set; }

        internal void LoadActData(ActDataPacket packet)
        {
            Act = packet.Act;
            Area = packet.Area;
            MapId = packet.MapId;
        }
    }
}
