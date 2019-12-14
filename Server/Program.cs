using Creation.Server.Core;
using Creation.Server.Net.Utils;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using log4net;
using log4net.Config;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Creation.Server
{
    internal class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static CreationGameServer Server { get; set; }

        private static async Task Main(string[] args)
        {
            try
            {
                XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), new FileInfo("log4net.config"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to setup logging! {ex.ToString()}");
            }

            Program.Logger.Info("Starting up server...");

            Program.Server = new CreationGameServer();
            Program.Server.Start();

            Program.Logger.Info("Server is ready!");

            await Task.Delay(-1);
        }
    }
}
