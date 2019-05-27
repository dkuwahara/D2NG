using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using D2NG.MCP.Packet;

namespace D2NG.MCP
{
    public class RealmServer
    {
        private McpConnection Connection { get; } = new McpConnection();

        protected ConcurrentDictionary<Mcp, Action<McpPacket>> PacketReceivedEventHandlers { get; } = new ConcurrentDictionary<Mcp, Action<McpPacket>>();
        protected ConcurrentDictionary<Mcp, Action<McpPacket>> PacketSentEventHandlers { get; } = new ConcurrentDictionary<Mcp, Action<McpPacket>>();
        public ushort RequestId { get; private set; } = 0x02;

        private readonly McpEvent CharLogonEvent = new McpEvent();
        private readonly McpEvent CreateGameEvent = new McpEvent();
        private readonly McpEvent ListCharactersEvent = new McpEvent();
        private readonly McpEvent StartupEvent = new McpEvent();
        private readonly McpEvent JoinGameEvent = new McpEvent();

        internal RealmServer()
        {
            Connection.PacketReceived += (obj, eventArgs) => PacketReceivedEventHandlers.GetValueOrDefault((Mcp)eventArgs.Type, null)?.Invoke(eventArgs);
            Connection.PacketSent += (obj, eventArgs) => PacketSentEventHandlers.GetValueOrDefault((Mcp)eventArgs.Type, null)?.Invoke(eventArgs);

            OnReceivedPacketEvent(Mcp.STARTUP, StartupEvent.Set);
            OnReceivedPacketEvent(Mcp.CHARLIST2, ListCharactersEvent.Set);
            OnReceivedPacketEvent(Mcp.CHARLOGON, CharLogonEvent.Set);
            OnReceivedPacketEvent(Mcp.CREATEGAME, CreateGameEvent.Set);
            OnReceivedPacketEvent(Mcp.JOINGAME, JoinGameEvent.Set);
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

        internal void CharLogon(Character character)
        {
            CharLogonEvent.Reset();
            var packet = new CharLogonRequestPacket(character.Name);
            Connection.WritePacket(packet);
            var response = new CharLogonResponsePacket(CharLogonEvent.WaitForPacket());
            if (response.Result != 0x00)
            {
                throw new CharLogonException($"Failed to log on as {character.Name} - {response.Result}");
            }
        }

        internal void Logon(uint mcpCookie, uint mcpStatus, List<byte> mcpChunk, string mcpUniqueName)
        {
            StartupEvent.Reset();
            var packet = new McpStartupRequestPacket(mcpCookie, mcpStatus, mcpChunk, mcpUniqueName);
            Connection.WritePacket(packet);
            var response = StartupEvent.WaitForPacket();
            _ = new McpStartupResponsePacket(response.Raw);
        }

        internal List<Character> ListCharacters()
        {
            ListCharactersEvent.Reset();
            Connection.WritePacket(new ListCharactersClientPacket());
            var packet = ListCharactersEvent.WaitForPacket();
            var response = new ListCharactersServerPacket(packet.Raw);
            return response.Characters;
        }

        internal void CreateGame(Difficulty difficulty, string gameName, string password)
        {
            CreateGameEvent.Reset();
            Connection.WritePacket(new CreateGameRequestPacket(RequestId++, difficulty, gameName, password));
            _ = new CreateGameResponsePacket(CreateGameEvent.WaitForPacket().Raw);
        }

        internal JoinGameResponsePacket JoinGame(string name, string password)
        {
            JoinGameEvent.Reset();
            Connection.WritePacket(new JoinGameRequestPacket(RequestId++, name, password));
            return new JoinGameResponsePacket(JoinGameEvent.WaitForPacket().Raw);
        }

        internal void OnReceivedPacketEvent(Mcp type, Action<McpPacket> handler) 
            => PacketReceivedEventHandlers.AddOrUpdate(type, handler, (t, h) => h += handler);

        internal void OnSentPacketEvent(Mcp type, Action<McpPacket> handler) 
            => PacketSentEventHandlers.AddOrUpdate(type, handler, (t, h) => h += handler);
    }
}