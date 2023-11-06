using System;
using System.IO;
using System.Linq;
using Tera.Connection.Crypt;
using Tools;

namespace Tera.Connection
{
    public class ConnectionDecrypter
    {
        private MemoryStream _client = new MemoryStream();
        private MemoryStream _server = new MemoryStream();
        private Session _session;
        private PacketBuffer _clientBuffer;
        private PacketBuffer _serverBuffer;
        private string _clientSocket;

        public bool Initialized => _session != null;

        private void TryInitialize()
        {
            if (Initialized)
                throw new InvalidOperationException("Already initalized");
            if (!(_client.Length == 256 && _server.Length == 260))
                return;

            _server.Position = 0;
            _client.Position = 0;

            var magicBytes = _server.ReadBytes(4);
            if (!magicBytes.SequenceEqual(new byte[] {1, 0, 0, 0}))
                throw new FormatException("Not a Tera connection");

            var clientKey1 = _client.ReadBytes(128);
            var clientKey2 = _client.ReadBytes(128);
            var serverKey1 = _server.ReadBytes(128);
            var serverKey2 = _server.ReadBytes(128);
            _session = new Session(clientKey1, clientKey2, serverKey1, serverKey2);
            _clientBuffer = new PacketBuffer(true, _clientSocket);
            _serverBuffer = new PacketBuffer(false, _clientSocket);
            _client.Close();
            _server.Close();
        }

        public void ClientToServer(byte[] data)
        {
            if (Initialized)
            {
                var result = data.ToArray();
                _session.Decrypt(result);
                _serverBuffer.Write(result);
                _serverBuffer.Read();
            }
            else
            {
                _client.Write(data, 0, data.Length);
                TryInitialize();
            }
        }

        public void ServerToClient(byte[] data)
        {
            if (Initialized)
            {
                var result = data.ToArray();
                _session.Encrypt(result);
                _clientBuffer.Write(result);
                _clientBuffer.Read();
            }
            else
            {
                _server.Write(data, 0, data.Length);
                TryInitialize();
            }
        }
        public void setSocket(string socket)
        {
            _clientSocket = socket;
        }
    }
}