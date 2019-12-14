using Creation.Server.Extensions;
using Net.Communication.Incoming.Handlers;
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
    internal class MessageHandler : IIncomingObjectHandler<ReadOnlySequence<byte>>
    {
        private int Step;

        public void Handle(ref SocketPipelineContext context, ref ReadOnlySequence<byte> data)
        {
            Utf8JsonReader jsonReader = new Utf8JsonReader(data);

            Console.WriteLine($"-> {Encoding.UTF8.GetString(data.ToArray())}");

            //All packets are arrays! Yey!
            string packetName = jsonReader.ReadString();
            switch (packetName)
            {
                case "greetings":
                    {
                        context.Connection.Send(@"[""ver"", ""Creation Server"", ""0""]"); //Server type, Server id(?)
                        context.Connection.Send(@"[""pleaseLogin""]");
                    }
                    break;
                case "clientVersion":
                    {
                        //Please login here
                    }
                    break;
                case "login":
                    {
                        context.Connection.Send(@"[""selfIntroduction"", [""1"", ""j"", ""Joni"", ""2"", ""what"", ""{}""]]"); //Id, Site, Name, Power, Avatar, Server vars?
                        context.Connection.Send(@"[""connectionReady""]");
                    }
                    break;
                case "createGroup":
                    {
                        string groupType = jsonReader.ReadString();
                        if (groupType == "CreationGroup")
                        {
                            string groupName = jsonReader.ReadString();

                            context.Connection.Send(@$"[""createGroupResult"", ""{groupName}"", ""true""]"); //Success
                            context.Connection.Send(@$"[""{groupName}cp"", ""1""]"); //User id
                        }
                    }
                    break;
                case "update":
                    {
                        string groupName = jsonReader.ReadString();
                        List<int[][]> tiles = JsonSerializer.Deserialize<List<int[][]>>(jsonReader.ReadString());

                        tiles.RemoveAll((i) => i == null); //For whatever reason the client sends null's

                        if (tiles.Count > 0)
                        {
                            context.Connection.Send(@$"[""{groupName}step"", ""{this.Step++}"", {JsonSerializer.Serialize(tiles)}, """"]"); //Step num, tiles (x, y) coord pairs, user server vars
                        }
                    }
                    break;
            }
        }
    }
}
