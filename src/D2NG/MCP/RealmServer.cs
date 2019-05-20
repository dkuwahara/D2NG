using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using D2NG.MCP.Packet;
using Serilog;

namespace D2NG.MCP
{
    public class RealmServer
    {
        private McpConnection Connection { get; } = new McpConnection();

        protected ConcurrentDictionary<byte, Action<McpPacket>> PacketReceivedEventHandlers { get; } = new ConcurrentDictionary<byte, Action<McpPacket>>();
        protected ConcurrentDictionary<byte, Action<McpPacket>> PacketSentEventHandlers { get; } = new ConcurrentDictionary<byte, Action<McpPacket>>();

        private (AutoResetEvent Event, McpPacket Packet) ListCharactersEvent = (new AutoResetEvent(false), null);
        private (AutoResetEvent Event, McpPacket Packet) StartupEvent = (new AutoResetEvent(false), null);

        internal RealmServer()
        {
            Connection.PacketReceived += (obj, eventArgs) => {
                var sid = eventArgs.Type;
                PacketReceivedEventHandlers.GetValueOrDefault(sid, null)?.Invoke(eventArgs);
            };

            Connection.PacketSent += (obj, eventArgs) => {
                var sid = eventArgs.Type;
                PacketSentEventHandlers.GetValueOrDefault(sid, null)?.Invoke(eventArgs);
            };

            OnReceivedPacketEvent(0x01, obj => EventSet(ref StartupEvent, obj));
            OnReceivedPacketEvent(0x19, obj => EventSet(ref ListCharactersEvent, obj));
        }

        private void EventSet(ref (AutoResetEvent Event, McpPacket Packet) evt, McpPacket obj)
        {
            evt.Packet = obj;
            evt.Event.Set();
        }

        internal void Connect(IPAddress ip, short port)
        {
            Connection.Connect(ip, port);
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

        internal void Logon(uint mcpCookie, uint mcpStatus, List<byte> mcpChunk, string mcpUniqueName)
        {
            var packet = new McpStartupRequestPacket(mcpCookie, mcpStatus, mcpChunk, mcpUniqueName);
            Connection.WritePacket(packet);
            StartupEvent.Event.WaitOne();
            _ = new McpStartupResponsePacket(StartupEvent.Packet.Raw);
        }

        public List<McpCharacter> ListCharacters()
        {
            Connection.WritePacket(new ListCharactersClientPacket());
            ListCharactersEvent.Event.WaitOne();
            var response = new ListCharactersServerPacket(ListCharactersEvent.Packet.Raw);
            return response.Characters;
        }

        public void OnReceivedPacketEvent(byte type, Action<McpPacket> handler)
        {
            if (PacketReceivedEventHandlers.ContainsKey(type))
            {
                PacketReceivedEventHandlers[type] += handler;
            }
            else
            {
                PacketReceivedEventHandlers.GetOrAdd(type, handler);
            }
        }

        public void OnSentPacketEvent(byte type, Action<McpPacket> handler)
        {
            if (PacketSentEventHandlers.ContainsKey(type))
            {
                PacketSentEventHandlers[type] += handler;
            }
            else
            {
                PacketSentEventHandlers.GetOrAdd(type, handler);
            }
        }
    }
}