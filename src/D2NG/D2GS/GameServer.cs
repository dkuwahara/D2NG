using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using D2NG.D2GS.Packet;
using D2NG.MCP;
using Serilog;

namespace D2NG.D2GS
{
    internal class GameServer : IDisposable
    {
        private const ushort Port = 4000;

        private GameServerConnection Connection { get; } = new GameServerConnection();

        protected ConcurrentDictionary<byte, Action<D2gsPacket>> PacketReceivedEventHandlers { get; } = new ConcurrentDictionary<byte, Action<D2gsPacket>>();

        protected ConcurrentDictionary<byte, Action<D2gsPacket>> PacketSentEventHandlers { get; } = new ConcurrentDictionary<byte, Action<D2gsPacket>>();

        private readonly ManualResetEvent LoadCompleteEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent LoadSuccessEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent GameExitEvent = new ManualResetEvent(false);

        private Thread _listener;

        public GameServer()
        {
            Connection.PacketReceived += (obj, eventArgs) 
                => PacketReceivedEventHandlers.GetValueOrDefault(eventArgs.Type, p => Log.Debug($"Received unhandled D2GS packet of type: 0x{(byte)p.Type,2:X2}"))?.Invoke(eventArgs);
            Connection.PacketSent += (obj, eventArgs) => PacketSentEventHandlers.GetValueOrDefault(eventArgs.Type, null)?.Invoke(eventArgs);

            Connection.PacketSent += (obj, packet) => Log.Verbose($"Sent D2GS packet of type: 0x{packet.Type,2:X2} {BitConverter.ToString(packet.Raw)}");

            OnReceivedPacketEvent(0x02, _ => LoadSuccessEvent.Set());
            OnReceivedPacketEvent(0x04, _ => LoadCompleteEvent.Set());
            OnReceivedPacketEvent(0xB0, _ => GameExitEvent.Set());
        }

        internal void OnReceivedPacketEvent(byte type, Action<D2gsPacket> handler)
            => PacketReceivedEventHandlers.AddOrUpdate(type, handler, (t, h) => h += handler);

        internal void OnSentPacketEvent(byte type, Action<D2gsPacket> handler)
            => PacketSentEventHandlers.AddOrUpdate(type, handler, (t, h) => h += handler);

        public void Connect(IPAddress ip)
        {
            Connection.Connect(ip, Port);
            _listener = new Thread(Listen);
            _listener.Start();
        }

        private void Listen()
        {
            try
            {
                while (Connection.Connected)
                {
                    _ = Connection.ReadPacket();
                }
            }
            catch(IOException)
            {
                Log.Debug("Connection was terminated");
                Thread.Sleep(300);
            }
        }
        public void LeaveGame()
        {
            GameExitEvent.Reset();
            Connection.WritePacket(new byte[] { 0x69 });
            GameExitEvent.WaitOne();
            Connection.Terminate();
            _listener.Join();
        }

        internal void GameLogon(uint gameHash, ushort gameToken, Character character)
        {
            LoadSuccessEvent.Reset();
            Connection.WritePacket(new GameLogonPacket(gameHash, gameToken, character));
            LoadSuccessEvent.WaitOne();
            LoadCompleteEvent.Reset();
            Connection.WritePacket(new byte[] { 0x6B });
            LoadCompleteEvent.WaitOne();
            Log.Verbose("Game load complete");
        }

        internal void Ping() => Connection.WritePacket(new PingPacket());

        public void Dispose() 
        {
            LoadCompleteEvent.Dispose();
            LoadSuccessEvent.Dispose();
            GameExitEvent.Dispose();
        }

        internal void SendPacket(byte[] packet) => Connection.WritePacket(packet);
    }
}
