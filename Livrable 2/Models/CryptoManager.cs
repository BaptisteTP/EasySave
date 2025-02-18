using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace EasySave2._0.Models
{
    public class CryptoManager
    {
        private string FilePath { get; }
        private string Key { get; }

        public CryptoManager(string filePath, string key)
        {
            FilePath = filePath;
            Key = key;
        }

        public int EncryptFile()
        {
            if (!CheckFile()) return -1;

            Stopwatch stopwatch = Stopwatch.StartNew();
            var fileBytes = File.ReadAllBytes(FilePath);
            var keyBytes = ConvertToByte(Key);
            fileBytes = XorMethod(fileBytes, keyBytes);
            File.WriteAllBytes(FilePath, fileBytes);
            stopwatch.Stop();
            return (int)stopwatch.ElapsedMilliseconds;
        }

        private bool CheckFile()
        {
            if (File.Exists(FilePath)) return true;
            Console.WriteLine("File not found.");
            Thread.Sleep(1000);
            return false;
        }

        private static byte[] ConvertToByte(string text)
        {
            // Convertit le texte en tableau de bytes et garantit une clé d'au moins 64 bits (8 octets)
            byte[] keyBytes = Encoding.UTF8.GetBytes(text);
            if (keyBytes.Length < 8)
            {
                Array.Resize(ref keyBytes, 8); // Si la clé est trop courte, on la remplit jusqu'à 8 octets
            }
            return keyBytes;
        }

        private static byte[] XorMethod(IReadOnlyList<byte> fileBytes, IReadOnlyList<byte> keyBytes)
        {
            var result = new byte[fileBytes.Count];
            for (var i = 0; i < fileBytes.Count; i++)
            {
                result[i] = (byte)(fileBytes[i] ^ keyBytes[i % keyBytes.Count]);
            }
            return result;
        }
    }
}
