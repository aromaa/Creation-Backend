using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Creation.Server.Extensions
{
    internal static class Utf8JsonReaderExtension
    {
        private static void ReadUntil(this ref Utf8JsonReader reader)
        {
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartArray:
                    case JsonTokenType.StartObject:
                    case JsonTokenType.EndArray:
                    case JsonTokenType.EndObject:
                        continue;
                }

                break;
            }
        }

        public static string ReadString(this ref Utf8JsonReader reader)
        {
            reader.ReadUntil();

            return reader.GetString();
        }
    }
}
