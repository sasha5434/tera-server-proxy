using System;
using System.Buffers.Binary;
using Tools;

namespace Tera.Connection
{
    public class PacketBuffer
    {
        private bool _incoming;
        private byte[] _buffer = new byte[0];
        private Dispatcher _dispatcher = new Dispatcher();
        private string _connectionSocket;

        public PacketBuffer (bool incoming, string socket)
        {
            _incoming = incoming;
            _connectionSocket = socket;
        }

        public void Write(byte[] data)
        {
            _buffer = _buffer.Append(data);
        }

        public void Read()
        {
            if (_buffer.Length < 2)
                return;
            int length = BinaryPrimitives.ReadUInt16LittleEndian(_buffer);
            while (_buffer.Length >= length && _buffer.Length > 2)
            {
                byte[] payload = _buffer.Payload(length);
                _dispatcher.Dispatch(payload, _connectionSocket, _incoming);
                _buffer = _buffer.Shift(length);
                if (_buffer.Length > 2)
                    length = BinaryPrimitives.ReadUInt16LittleEndian(_buffer);
            }
        }
    }
}