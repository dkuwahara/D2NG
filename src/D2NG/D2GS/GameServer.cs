using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using D2NG.D2GS.Packet;
using D2NG.MCP;
using Serilog;

namespace D2NG.D2GS
{
    internal class GameServer
    {
        private const ushort Port = 4000;

        private GameServerConnection Connection { get; } = new GameServerConnection();

        protected ConcurrentDictionary<byte, Action<D2gsPacket>> PacketReceivedEventHandlers { get; } = new ConcurrentDictionary<byte, Action<D2gsPacket>>();
        protected ConcurrentDictionary<byte, Action<D2gsPacket>> PacketSentEventHandlers { get; } = new ConcurrentDictionary<byte, Action<D2gsPacket>>();

        private ManualResetEvent LoadSuccessEvent = new ManualResetEvent(false);

        public GameServer()
        {
            Connection.PacketReceived += (obj, eventArgs) => PacketReceivedEventHandlers.GetValueOrDefault(eventArgs.Type, null)?.Invoke(eventArgs);
            Connection.PacketSent += (obj, eventArgs) => PacketSentEventHandlers.GetValueOrDefault(eventArgs.Type, null)?.Invoke(eventArgs);

            Connection.PacketReceived += (obj, packet) => Log.Verbose($"Received packet of type: 0x{(byte)packet.Type, 2:X2}");
            Connection.PacketSent += (obj, packet) => Log.Verbose($"Sent packet of type: 0x{packet.Type,2:X2} {(D2gs)packet.Type}");

            OnReceivedPacketEvent(0x02, p => LoadSuccessEvent.Set());
        }

        internal void OnReceivedPacketEvent(byte type, Action<D2gsPacket> handler)
            => PacketReceivedEventHandlers.AddOrUpdate(type, handler, (t, h) => h += handler);

        internal void OnSentPacketEvent(byte type, Action<D2gsPacket> handler)
            => PacketSentEventHandlers.AddOrUpdate(type, handler, (t, h) => h += handler);

        public void Connect(IPAddress ip)
        {
            Connection.Connect(ip, Port);
            var listener = new Thread(Listen);
            listener.Start();
        }

        private void Listen()
        {
            while (Connection.Connected)
            {
                _ = Connection.ReadPacket();
            }
        }

        internal void GameLogon(uint gameHash, ushort gameToken, Character character)
        {
            LoadSuccessEvent.Reset();
            Connection.WritePacket(new GameLogonPacket(gameHash, gameToken, character));
            LoadSuccessEvent.WaitOne();
            Connection.WritePacket(new byte[] { 0x6B });
        }
    }
}
