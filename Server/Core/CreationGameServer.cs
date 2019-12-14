using Creation.Server.Net.Handlers;
using Net.Listeners;
using Net.Managers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

using TcpListener = Net.Listeners.Tcp.TcpListener;

namespace Creation.Server.Core
{
    internal class CreationGameServer
    {
        internal SocketListenerManager SocketListenerManager { get; private set; }

        internal void Start()
        {
            this.SocketListenerManager = new SocketListenerManager((connection) =>
            {
                connection.Send(@"[""ver"", ""Creation Server"", ""0""]"); //Send this random ass packet as ping, no idea how to do this properly
            });

            this.SocketListenerManager.ConnectionManager.PreAccept += (connection) =>
            {
                connection.Pipeline.AddHandlerLast(SplitMessageHandler.INSTANCE);
                connection.Pipeline.AddHandlerLast(new MessageChecksumHandle());
                connection.Pipeline.AddHandlerLast(new EncryptionHandler());
                connection.Pipeline.AddHandlerLast(new MessageHandler());
            };

            this.SocketListenerManager.AddListener<TcpListener>(new ListenerConfig()
            {
                Address = IPAddress.Any,
                Port = 1501,

                Backlog = 10,
            });
        }
    }
}
