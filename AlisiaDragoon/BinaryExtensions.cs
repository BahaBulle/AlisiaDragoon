namespace AlisiaDragoon
{
    using System;
    using System.IO;

    public static class BinaryExtensions
    {
        public static ushort ReadUInt16BE(this BinaryReader reader)
        {
            byte[] data = reader.ReadBytes(2);
            Array.Reverse(data);

            return BitConverter.ToUInt16(data, 0);
        }

        public static uint ReadUInt32BE(this BinaryReader reader)
        {
            byte[] data = reader.ReadBytes(4);
            Array.Reverse(data);

            return BitConverter.ToUInt32(data, 0);
        }

        public static void WriteUIntBE(this BinaryWriter writer, uint value)
        {
            writer.Write((byte)(value >> 24));
            writer.Write((byte)(value >> 16));
            writer.Write((byte)(value >> 8));
            writer.Write((byte)value);
        }

        public static void WriteBytes(this MemoryStream stream, ushort value)
        {
            stream.WriteByte((byte)((value >> 8) & 0xFF));
            stream.WriteByte((byte)(value & 0xFF));
        }

        public static void WriteUShortBE(this BinaryWriter writer, ushort value)
        {
            writer.Write((byte)(value >> 8));
            writer.Write((byte)value);
        }
    }
}