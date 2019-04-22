using System;

namespace D2NG
{
    public class BNCS
    {
        private readonly BNCSConnection _connection = new BNCSConnection();

        public BNCS()
        {
            _connection.SubscribeToReceivedPacketEvent(0x25, (evt) => _connection.Send(evt.Packet));
        }

        public void ConnectToBattleNet(String realm)
        {
            _connection.Connect(realm);
            _connection.Send(0x01);
            _connection.Send(BNCSConnection.AuthInfoPacket);
            _connection.Listen();
        }
    }
}

