using System.Buffers.Binary;
using System.IO;
using System.Text;
using Tera.Game;

namespace Readers
{
    internal static class StreamReader
    {
        public static MemoryStream GetStream(this byte[] source)
        {
            MemoryStream stream = new(source);
            stream.Position = 0;
            return stream;
        }
        public static BinaryReader GetReader(this MemoryStream source)
        {
            BinaryReader reader = new BinaryReader(source, Encoding.Unicode);
            return reader;
        }
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
        public static void Skip(this BinaryReader reader, int count)
        {
            reader.BaseStream.Position += count;
        }
        public static void SkipHeader(this BinaryReader reader)
        {
            Skip(reader, 4);
        }
        public static string ReadTeraString(this BinaryReader reader)
        {
            var builder = new StringBuilder();
            try
            {
                while (true)
                {
                    var c = reader.ReadChar();
                    if (c == 0)
                    {
                        return builder.ToString();
                    }
                    builder.Append(c);
                }
            }
            catch { return builder.ToString(); } // don't crash on parsing strings from corrupted packets
        }
        public static EntityId ReadEntityId(this BinaryReader reader)
        {
            return new EntityId(reader.ReadUInt64());
        }
        public static Vector3f ReadVector3f(this BinaryReader reader)
        {
            Vector3f result;
            result.X = reader.ReadSingle();
            result.Y = reader.ReadSingle();
            result.Z = reader.ReadSingle();
            return result;
        }
        public static Angle ReadAngle(this BinaryReader reader)
        {
            return new Angle(reader.ReadInt16());
        }
        public static ulong ReadUInt64LE(this BinaryReader reader)
        {
            byte[] buffer = ReadBytes(reader.BaseStream, 8);
            return BinaryPrimitives.ReadUInt64LittleEndian(buffer);
        }
    }
}