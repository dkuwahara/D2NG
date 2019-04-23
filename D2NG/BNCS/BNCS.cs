using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using D2NG.BNCS;

namespace D2NG
{
    public class BattleNetChatServer
    {
        /**
         * Current version byte, update this on new patches
         */
        public static readonly byte VERSION = 0x0e;

        /**
         * Packet sent to authenticate version.
         */
        private static readonly byte[] AUTH_INFO_PACKET =
        {
            0xff, 0x50, 0x3a, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x36, 0x38, 0x58, 0x49, 0x50, 0x58, 0x32, 0x44,
            VERSION, 0x00, 0x00, 0x00, 0x53, 0x55, 0x6e, 0x65,
            0x55, 0xb4, 0x47, 0x40, 0x88, 0xff, 0xff, 0xff,
            0x09, 0x04, 0x00, 0x00, 0x09, 0x04, 0x00, 0x00,
            0x55, 0x53, 0x41, 0x00, 0x55, 0x6e, 0x69, 0x74,
            0x65, 0x64, 0x20, 0x53, 0x74, 0x61, 0x74, 0x65,
            0x73, 0x00
        };

        private BNCSConnection Connection { get; } = new BNCSConnection();

        protected ConcurrentDictionary<byte, Action<BNCSPacketReceivedEvent>> PacketReceivedEventHandlers { get; } = new ConcurrentDictionary<byte, Action<BNCSPacketReceivedEvent>>();

        protected ConcurrentDictionary<byte, Action<BNCSPacketSentEvent>> PacketSentEventHandlers { get; } = new ConcurrentDictionary<byte, Action<BNCSPacketSentEvent>>();

        public BattleNetChatServer()
        {
            Connection.PacketReceived += (obj, eventArgs) => {
                Log.Debug("[{0}] Received Packet 0x{1:X}", GetType(), eventArgs.Type);
                PacketReceivedEventHandlers.GetValueOrDefault(eventArgs.Type, null)?.Invoke(eventArgs);
            };

            Connection.PacketSent += (obj, eventArgs) => {
                Log.Debug("[{0}] Sent Packet 0x{1:X}", GetType(), eventArgs.Type);
                PacketSentEventHandlers.GetValueOrDefault(eventArgs.Type, null)?.Invoke(eventArgs);
            };

            OnReceivedPacketEvent(0x25, obj => Connection.Send(obj.Packet));
        }
 
        public void SendPacket(byte command, params IEnumerable<byte>[] args)
        {
            byte[] packet = BuildPacket(command, args);
            Connection.Send(packet);
        }

        private byte[] BuildPacket(byte command, IEnumerable<byte>[] args)
        {
            var packet = new List<byte> { 0xFF, command };
            var packetArray = new List<byte>();

            foreach (var a in args)
            {
                packetArray.AddRange(a);
            }

            UInt16 packetSize = (UInt16)(packetArray.Count + 4);
            packet.AddRange(BitConverter.GetBytes(packetSize));
            packet.AddRange(packetArray);

            return packet.ToArray();
        }

        public void OnReceivedPacketEvent(byte type, Action<BNCSPacketReceivedEvent> handler)
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

        public void OnSentPacketEvent(byte type, Action<BNCSPacketSentEvent> handler)
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

        public void ConnectTo(String realm)
        {
            Connection.Connect(realm);
            Connection.Send(0x01);
            Connection.Send(AUTH_INFO_PACKET);
            Connection.Listen();
        }

    }
}

