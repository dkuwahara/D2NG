using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D2NG
{
    public class BattleNetChatServer
    {

        private static readonly byte[] PROTOCOL_ID = BitConverter.GetBytes(0x00);

        private static readonly byte[] PLATFORM_CODE = Encoding.ASCII.GetBytes("IX86");

        private static readonly byte[] PRODUCT_CODE = Encoding.ASCII.GetBytes("D2XP");

        private static readonly byte[] PRODUCT_VERSION = BitConverter.GetBytes(0x0e);

        private static readonly byte[] LANGUAGE_CODE = Encoding.ASCII.GetBytes("enUS");

        private static readonly byte[] LOCAL_IP = BitConverter.GetBytes(0x00);

        private static readonly byte[] TIME_ZONE_BIAS = BitConverter.GetBytes((int)(DateTime.UtcNow.Subtract(DateTime.Now).TotalSeconds / 60));

        private static readonly byte[] MPQ_LOCALE_ID  = BitConverter.GetBytes(0x00);

        private static readonly byte[] USER_LANG_ID = BitConverter.GetBytes(0x00);

        private static readonly byte[] COUNTRY_ABBR = Encoding.ASCII.GetBytes("USA\0");

        private static readonly byte[] COUNTRY = Encoding.ASCII.GetBytes("United States\0");

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

            OnReceivedPacketEvent(0x25, obj => Connection.WritePacket(obj.Packet));
        }
 
        public void SendPacket(byte command, params IEnumerable<byte>[] args)
        {
            byte[] packet = BuildPacket(command, args);
            Connection.WritePacket(packet);
        }

        public byte[] BuildPacket(byte command, params IEnumerable<byte>[] args)
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
            Connection.WritePacket(BuildAuthInfoPacket());
        }

        public byte[] BuildAuthInfoPacket()
        {
            return BuildPacket(
                0x50,
                PROTOCOL_ID,
                PLATFORM_CODE.Reverse().ToArray(),
                PRODUCT_CODE.Reverse().ToArray(),
                PRODUCT_VERSION,
                LANGUAGE_CODE.Reverse().ToArray(),
                LOCAL_IP,
                TIME_ZONE_BIAS,
                MPQ_LOCALE_ID,
                USER_LANG_ID,
                COUNTRY_ABBR,
                COUNTRY);
        }
    }
}

