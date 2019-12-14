using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Creation.Server.Net.Utils
{
    internal static class CryptoUtils
    {
        private const string KEY = "NmkjVTk3NFVWNStRRDRrNA==";
        private const string IV = "6kT+lc7WPCXBjMxLHURgbw==";

        private static readonly byte[] IV_ASCII = Encoding.ASCII.GetBytes(CryptoUtils.IV);

        private static readonly byte[] KEY_BYTES = Convert.FromBase64String(CryptoUtils.KEY);
        private static readonly byte[] IV_BYTES = Convert.FromBase64String(CryptoUtils.IV);

        private static readonly char[] HEX = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        internal static void CreateChecksumHash(int counter, ref Span<byte> hash)
        {
            using MD5 md5 = MD5.Create();

            int digits = Math.Max(1, (int)MathF.Log10(counter) + 1);

            Span<byte> hashBytes = stackalloc byte[CryptoUtils.IV_ASCII.Length + digits];

            CryptoUtils.IV_ASCII.CopyTo(hashBytes);

            //Over complex stuff to avoid string allocations, what a mess!
            CryptoUtils.IntToAsciBytes(hashBytes.Slice(start: CryptoUtils.IV_ASCII.Length), digits, counter);

            //TryComputeHash requires 16 bytes or it won't even try
            Span<byte> returnBytes = stackalloc byte[16];

            md5.TryComputeHash(hashBytes, returnBytes, out _);

            //Over complex stuff to avoid string allocations, what a mess!
            hash[0] = (byte)CryptoUtils.HEX[returnBytes[0] >> 4 & 0xF];
            hash[1] = (byte)CryptoUtils.HEX[returnBytes[0] & 0xF];
            hash[2] = (byte)CryptoUtils.HEX[returnBytes[1] >> 4 & 0xF];
        }

        private static void IntToAsciBytes(Span<byte> buffer, int digits, int value)
        {
            for(int i = --digits; i >= 0; i--)
            {
                value = Math.DivRem(value, 10, out int remainder);

                buffer[i] = (byte)CryptoUtils.HEX[remainder];
            }
        }

        internal static Rijndael CreateCrypt()
        {
            Rijndael crypt = Rijndael.Create();
            crypt.Mode = CipherMode.CBC;
            crypt.KeySize = 128;
            crypt.Padding = PaddingMode.Zeros;

            crypt.IV = Convert.FromBase64String(CryptoUtils.IV);
            crypt.Key = Convert.FromBase64String(CryptoUtils.KEY);

            return crypt;
        }
    }
}
