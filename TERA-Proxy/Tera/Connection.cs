using Readers;
using System;
using System.Net;
using System.Net.Sockets;
using Tera.Connection.Crypt;
using Tera.Connection.Dispatcher;

namespace Tera.Connection
{
    public class TeraConnection
    {
        private Session serverSession;
        private Session clientSession;
        public UserData userData;
        private PacketBuffer serverBuffer;
        private PacketBuffer clientBuffer;
        private NetworkStream clientConnection;
        private NetworkStream serverConnection;
        private Dispatch dispatch;
        private byte state;
        private readonly object serverLocker = new(), clientLocker = new();

        public TeraConnection(NetworkStream clientConnection, NetworkStream serverConnection, IPEndPoint clientEndPoint)
        {
            this.userData = new UserData(clientEndPoint.ToString());
            this.state = 0;
            this.serverSession = new Session();
            this.dispatch = new Dispatch(this);
            this.serverBuffer = new PacketBuffer(dispatch);
            this.clientBuffer = new PacketBuffer(dispatch);
            this.clientConnection = clientConnection;
            this.serverConnection = serverConnection;
        }
        public void ServerWrite(byte[] data)
        {
            switch (this.state)
            {
                case 0:
                    {
                        if (data.ReadUInt32LE(0) == 1)
                        {
                            this.state = 1;
                            this.sendClient(data);
                        }
                        break;
                    }
                case 1:
                    {
                        if (data.Length == 128)
                        {
                            Array.Copy(data, this.serverSession.ServerKey1, 128);
                            this.state = 2;
                            this.sendClient(data);
                        }
                        break;
                    }
                case 2:
                    {
                        if (data.Length == 128)
                        {
                            Array.Copy(data, this.serverSession.ServerKey2, 128);
                            this.clientSession = this.serverSession.CloneKeys();
                            this.serverSession.init();
                            this.sendClient(data);
                            this.state = 3;
                        }
                        break;
                    }
                case 3:
                    {
                        this.serverSession.Encrypt(data);
                        this.serverBuffer.Write(data, true);
                        break;
                    }
            }
        }
        public void ClientWrite(byte[] data)
        {
            switch (this.state)
            {
                case 1:
                    {
                        if (data.Length == 128)
                        {
                            Array.Copy(data, this.serverSession.ClientKey1, 128);
                            sendServer(data);
                        }
                        break;
                    }
                case 2:
                    {
                        if (data.Length == 128)
                        {
                            Array.Copy(data, this.serverSession.ClientKey2, 128);
                            sendServer(data);
                        }
                        break;
                    }
                case 3:
                    {
                        this.clientSession.Decrypt(data);
                        this.clientBuffer.Write(data, false);
                        break;
                    }
            }
        }
        public void sendClient(byte[] data)
        {
            lock (clientLocker)
            {
                if (this.state == 3)
                {
                    this.clientSession.Encrypt(data);
                }
                this.clientConnection.Write(data, 0, data.Length);
            }
        }
        public void sendServer(byte[] data)
        {
            lock (serverLocker)
            {
                if (this.state == 3)
                {
                    this.serverSession.Decrypt(data);
                }
                this.serverConnection.Write(data, 0, data.Length);
            }
        }
        public void Close()
        {
            this.state = 4;
            this.clientSession = null;
            this.serverSession = null;
            this.clientBuffer = null;
            this.serverBuffer = null;
            this.clientConnection.Dispose();
            this.serverConnection.Dispose();
            this.dispatch = null;
            if (userData?.User?.Character?.Name != null)
                Globals.WebTeraData.Pools.Remove(userData.User.Character);
            this.userData = null;
        }

        public void logUser()
        {
            Console.WriteLine($"User Info : {{ {((IPEndPoint)clientConnection.Socket.RemoteEndPoint).Address} : {((IPEndPoint)clientConnection.Socket.RemoteEndPoint).Port} }} - {{ {((IPEndPoint)clientConnection.Socket.LocalEndPoint).Address} : {((IPEndPoint)clientConnection.Socket.LocalEndPoint).Port} }} ");
        }
    }
}