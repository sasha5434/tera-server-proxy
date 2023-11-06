using System;
using System.Text;
using System.Linq;
using System.IO;

namespace Tools
{
    internal static class Tools
    {
        private static void ReadBytes(Stream stream, byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                var bytesRead = stream.Read(buffer, offset, count);
                if (bytesRead == 0)
                    throw new IOException("Unexpected end of stream");
                count -= bytesRead;
                offset += bytesRead;
            }
        }

        public static byte[] ReadBytes(this Stream stream, int count)
        {
            var result = new byte[count];
            ReadBytes(stream, result, 0, result.Length);
            return result;
        }
        public static byte[] Append(this byte[] source, byte[] add)
        {
            byte[] temp = new byte[source.Length + add.Length];
            Buffer.BlockCopy(source, 0, temp, 0, source.Length);
            Buffer.BlockCopy(add, 0, temp, source.Length, add.Length);
            return temp;
        }
        public static byte[] Payload(this byte[] source, int offset)
        {
            byte[] temp = new byte[offset - 2];
            Buffer.BlockCopy(source, 2, temp, 0, offset - 2);
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
                sb.Append((temp[0] > 0x20 && temp[0] < 0x7F) ? System.Text.Encoding.ASCII.GetString(temp) : ".");
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

                sb.AppendLine($"{start:X8}: {string.Join(' ', buffer.Select(b => b.ToString("X2"))),-48} {buffer.ToASCII()}");

                start += position;
            }

            return sb.ToString();
        }
    }
}