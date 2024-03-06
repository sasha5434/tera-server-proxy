using Readers;
using System;

namespace Tera.Connection
{
    public class PacketBuffer
    {
        private Dispatcher.Dispatch dispatch;
        private int position = 0;
        private byte[] buffer = null;

        public PacketBuffer(Dispatcher.Dispatch dispatch)
        {
            this.dispatch = dispatch;
        }

        public bool Write(byte[] data, bool fromServer = false)
        {
            while (data.Length > 0)
            {
                int size;
                // if we have a buffer prepared, we should append to it first
                if (this.buffer != null)
                {
                    // if our buffer size is less than 2, we'll need to compute the full size
                    if (this.buffer.Length < 2)
                    {
                        byte old = this.buffer[0];         // save old byte
                        size = data.Length + 1;            // calc new size
                        this.buffer = new byte[size];      // make new buffer
                        this.buffer[0] = old;              // write old value
                        this.position = 1;                 // update position
                    }

                    // write as many bytes as we can
                    var remaining = Math.Min(data.Length, this.buffer.Length - this.position);
                    Buffer.BlockCopy(data, 0, this.buffer, this.position, remaining);
                    this.position += remaining;

                    // if we filled the buffer, push it
                    if (this.position == this.buffer.Length)
                    {
                        dispatch.Handle(this.buffer, fromServer);
                        this.buffer = null;
                        this.position = 0;
                    }

                    // chop off the front and keep going
                    data = new ArraySegment<byte>(data, remaining, data.Length - remaining).ToArray();
                    continue;
                }

                // if it's too small to read the size value, just save it in the buffer and
                // we'll hopefully get to it the next time around
                if (data.Length < 2)
                {
                    this.buffer = data;
                    this.position = data.Length;
                    break;
                }

                // otherwise, read the size value, and if it's bigger than the size of the
                // data we have, we should save it in the buffer
                size = data.ReadUInt16LE(offset: 0);
                if (size > data.Length)
                {
                    this.buffer = new byte[size];
                    Buffer.BlockCopy(data, 0, this.buffer, 0, data.Length);
                    this.position = data.Length;
                    break;
                }

                // otherwise, just push it and chop off the front, then keep going
                dispatch.Handle(new ArraySegment<byte>(data, 0, size).ToArray(), fromServer);
                data = new ArraySegment<byte>(data, size, data.Length - size).ToArray();
            }
            return true;
        }
    }
}