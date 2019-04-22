using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace D2NG
{
    public class BNCS
    {
        private readonly BNCSConnection _connection = new BNCSConnection();

        protected readonly ConcurrentDictionary<byte, Action<BNCSPacketReceivedEvent>> _packetReceivedEventHandlers = new ConcurrentDictionary<byte, Action<BNCSPacketReceivedEvent>>();

        protected readonly ConcurrentDictionary<byte, Action<BNCSPacketSentEvent>> _packetSentEventHandlers = new ConcurrentDictionary<byte, Action<BNCSPacketSentEvent>>();

        public BNCS()
        {
            //this.SubscribeToReceivedPacketEvent(0x25, (evt) => _connection.Send(evt.Packet));

            EventHandler<BNCSPacketReceivedEvent> onReceived = (sender, eventArgs) =>
            {
                Console.WriteLine("[{0}] Received Packet 0x{1:X}", GetType(), eventArgs.Type);
                this._packetReceivedEventHandlers.GetValueOrDefault(eventArgs.Type, null)?.Invoke(eventArgs);
            };
            _connection.PacketReceived += onReceived;

            EventHandler<BNCSPacketSentEvent> onSent = (sender, eventArgs) =>
            {
                Console.WriteLine("[{0}] Sent Packet 0x{1:X}", GetType(), eventArgs.Type);
                _packetSentEventHandlers.GetValueOrDefault(eventArgs.Type, null)?.Invoke(eventArgs);
            };
            _connection.PacketSent += onSent;
        }

        public void SubscribeToReceivedPacketEvent(byte type, Action<BNCSPacketReceivedEvent> handler)
        {
            if (_packetReceivedEventHandlers.ContainsKey(type))
            {
                _packetReceivedEventHandlers[type] += handler;
            }
            else
            {
                _packetReceivedEventHandlers.GetOrAdd(type, handler);
            }
        }

        public void SubscribeToSentPacketEvent(byte type, Action<BNCSPacketSentEvent> handler)
        {
            if (_packetSentEventHandlers.ContainsKey(type))
            {
                _packetSentEventHandlers[type] += handler;
            }
            else
            {
                _packetSentEventHandlers.GetOrAdd(type, handler);
            }
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

