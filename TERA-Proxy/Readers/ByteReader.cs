using System;
using System.Buffers.Binary;
using System.Linq;
using System.Text;

namespace Readers
{
    internal static class ByteReader
    {
        public static ushort ReadUInt16LE(this byte[] source, int offset = 0)
        {
            var buffer = new ArraySegment<byte>(source, offset, 2);
            return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
        }
        public static void WriteUInt16LE(this byte[] source, ushort value, int offset = 0)
        {
            var buffer = new ArraySegment<byte>(source, offset, 2);
            BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        }
        public static uint ReadUInt32LE(this byte[] source, int offset = 0)
        {
            var buffer = new ArraySegment<byte>(source, offset, 4);
            return BinaryPrimitives.ReadUInt32LittleEndian(buffer);
        }
        public static ulong ReadUInt64LE(this byte[] source, int offset = 0)
        {
            var buffer = new ArraySegment<byte>(source, offset, 8);
            return BinaryPrimitives.ReadUInt64LittleEndian(buffer);
        }
        public static byte[] Append(this byte[] source, byte[] data)
        {
            byte[] temp = new byte[source.Length + data.Length];
            Buffer.BlockCopy(source, 0, temp, 0, source.Length);
            Buffer.BlockCopy(data, 0, temp, source.Length, data.Length);
            return temp;
        }
        public static byte[] Shift(this byte[] source, int offset)
        {
            byte[] temp = new byte[source.Length - offset];
            Buffer.BlockCopy(source, offset, temp, 0, source.Length - offset);
            return temp;
        }
        private static string ToASCII(this byte[] source)
        {
            var sb = new StringBuilder();
            byte[] temp = new byte[1];
            for (int i = 0; i < source.Length; i++)
            {
                Buffer.BlockCopy(source, i, temp, 0, 1);
                sb.Append((temp[0] > 0x20 && temp[0] < 0x7F) ? Encoding.ASCII.GetString(temp) : ".");
            }
            return sb.ToString();
        }
        public static string ToHex(this byte[] source)
        {
            int start = 0;
            var sb = new StringBuilder();

            while (start < source.Length)
            {
                int position = (source.Length - start >= 16) ? 16 : source.Length - start;
                byte[] buffer = new byte[position];
                Buffer.BlockCopy(source, start, buffer, 0, position);

                sb.AppendLine($"{start:X4}: {string.Join(' ', buffer.Select(b => b.ToString("X2"))),-48} {buffer.ToASCII()}");

                start += position;
            }

            return sb.ToString();
        }
    }
}