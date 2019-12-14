using Creation.Server.Net.Utils;
using Net.Communication.Incoming.Handlers;
using Net.Communication.Outgoing.Handlers;
using Net.Communication.Outgoing.Helpers;
using Net.Communication.Pipeline;
using Nito.AsyncEx;
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Creation.Server.Net.Handlers
{
    internal class EncryptionHandler : IIncomingObjectHandler<ReadOnlySequence<byte>>, IOutgoingObjectHandler<byte[]>
    {
        private Rijndael Crypt;

        internal EncryptionHandler()
        {
            this.Crypt = CryptoUtils.CreateCrypt();
        }

        public void Handle(ref SocketPipelineContext context, ref ReadOnlySequence<byte> data)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ICryptoTransform decryptor = this.Crypt.CreateDecryptor())
                {
                    using CryptoStream stream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write, leaveOpen: true);

                    byte[] array = data.ToArray();

                    Base64.DecodeFromUtf8InPlace(array, out int writtenBytes);

                    stream.Write(array, 0, writtenBytes);
                }

                ReadOnlySequence<byte> plank = new ReadOnlySequence<byte>(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);

                //Encryption leaves some ugly null bytes
                SequencePosition? uglyNulls = plank.PositionOf((byte)'\0');
                if (uglyNulls != null)
                {
                    plank = plank.Slice(start: 0, end: uglyNulls.Value);
                }

                context.ProgressHandlerIn(ref plank);
            }
        }

        public void Handle(ref SocketPipelineContext context, in byte[] data, ref PacketWriter writer)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ICryptoTransform encrypter = this.Crypt.CreateEncryptor())
                {
                    using CryptoStream stream = new CryptoStream(memoryStream, encrypter, CryptoStreamMode.Write, leaveOpen: true);

                    stream.Write(data, 0, data.Length);
                }

                byte[] buffer = ArrayPool<byte>.Shared.Rent(Base64.GetMaxEncodedToUtf8Length((int)memoryStream.Length));

                try
                {
                    Base64.EncodeToUtf8(memoryStream.GetBuffer().AsSpan(start: 0, length: (int)memoryStream.Length), buffer, out _, out int writtenBytes);

                    writer.WriteBytes(buffer.AsSpan(start: 0, length: writtenBytes));
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }
    }
}
