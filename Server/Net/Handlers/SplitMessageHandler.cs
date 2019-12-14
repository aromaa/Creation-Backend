using Net.Communication.Incoming.Handlers;
using Net.Communication.Incoming.Helpers;
using Net.Communication.Outgoing.Handlers;
using Net.Communication.Outgoing.Helpers;
using Net.Communication.Pipeline;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Creation.Server.Net.Handlers
{
    internal class SplitMessageHandler : IIncomingObjectHandler<ReadOnlySequence<byte>>, IOutgoingObjectHandler
    {
        internal static readonly SplitMessageHandler INSTANCE = new SplitMessageHandler();

        private const byte BREAK_CHAR = 4;

        public void Handle(ref SocketPipelineContext context, ref ReadOnlySequence<byte> data)
        {
            SequencePosition? position = data.PositionOf(SplitMessageHandler.BREAK_CHAR);
            if (position != null)
            {
                ReadOnlySequence<byte> withoutTerminator = data.Slice(start: 0, end: position.Value);

                context.ProgressReadHandler(ref withoutTerminator);

                data = data.Slice(start: data.GetPosition(1, position.Value));
            }
        }

        public void Handle<T>(ref SocketPipelineContext context, in T data, ref PacketWriter writer)
        {
            string send;
            if (data is string asString)
            {
                send = asString;
            }
            else
            {
                send = JsonSerializer.Serialize(data);
            }

            Console.WriteLine($"<- {send}");

            byte[] buffer = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(send.Length));

            try
            {
                Encoding.UTF8.GetBytes(send, buffer);

                context.ProgressWriteHandler(buffer, ref writer);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            writer.WriteByte(SplitMessageHandler.BREAK_CHAR);
        }
    }
}
