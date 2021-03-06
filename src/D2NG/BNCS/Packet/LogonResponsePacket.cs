﻿using Serilog;
using System.IO;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class LogonResponsePacket : BncsPacket
    {
        public uint Status { get; }

        public LogonResponsePacket(BncsPacket packet) : this(packet.Raw)
        {
        }

        public LogonResponsePacket(byte[] packet) : base(packet)
        {
            var reader = new BinaryReader(new MemoryStream(packet), Encoding.ASCII);
            if (PrefixByte != reader.ReadByte())
            {
                throw new BncsPacketException("Not a valid BNCS Packet");
            }
            if ((byte)Sid.LOGONRESPONSE2 != reader.ReadByte())
            {
                throw new BncsPacketException("Expected type was not found");
            }
            if (packet.Length != reader.ReadUInt16())
            {
                throw new BncsPacketException("Packet length does not match");
            }

            Status = reader.ReadUInt32();
            
            switch (Status)
            {
                case 0x00:
                    Log.Verbose("Logon success");
                    break;
                case 0x01:
                    throw new LogonFailedException("Account does not exist");
                case 0x02:
                    throw new LogonFailedException("Invalid Password");
                case 0x06:
                    string message = reader.ReadString();
                    throw new LogonFailedException($"Account closed {message}");
                default:
                    throw new LogonFailedException($"Unknown login error {Status:X}");
            }

            reader.Close();
        }
    }
}