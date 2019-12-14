using Creation.Server.Net.Utils;
using Net.Communication.Incoming.Handlers;
using Net.Communication.Outgoing.Handlers;
using Net.Communication.Outgoing.Helpers;
using Net.Communication.Pipeline;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace Creation.Server.Net.Handlers
{
    internal class MessageChecksumHandle : IIncomingObjectHandler<ReadOnlySequence<byte>>, IOutgoingObjectHandler
    {
        private int IncomingCount;
        private int OutgoingCount;

        public void Handle(ref SocketPipelineContext context, ref ReadOnlySequence<byte> data)
        {
            Span<byte> checksum = stackalloc byte[3];

            CryptoUtils.CreateChecksumHash(++this.IncomingCount, ref checksum);

            Span<byte> checkAgainst = stackalloc byte[3];

            data.Slice(start: 0, length: 3).CopyTo(checkAgainst);

            if (checkAgainst.SequenceEqual(checksum))
            {
                data = data.Slice(start: 3);

                context.ProgressReadHandler(ref data);
            }
            else
            {
                context.Disconnect("Incoming checksum error");
            }
        }

        public void Handle<T>(ref SocketPipelineContext context, in T data, ref PacketWriter writer)
        {
            Span<byte> slice = writer.PrepareBytes(3).Span;

            CryptoUtils.CreateChecksumHash(++this.OutgoingCount, ref slice);

            context.ProgressWriteHandler(data, ref writer);
        }
    }
}
