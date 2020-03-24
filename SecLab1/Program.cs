using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SecLab1
{
    public class Program
    {
        private const string Abc = "abcdefghiklmnopqrstuvwxyz"; // no j.
        private static readonly char[,] Matrix = new char[5, 5];
        private const string FilePath = "/home/basicec/text.txt";

        
        public static void Main(string[] args)
        {
            if (!File.Exists(FilePath))
            {
                Console.WriteLine($"Cannot find file {Path.GetFullPath(FilePath)}");
                return;
            }

            using var sr = new StreamReader(FilePath);
            var message = sr.ReadToEnd();

            var used = new HashSet<char>();
            Console.WriteLine("Write key:");
            var key = Console.ReadLine();
            if (string.IsNullOrEmpty(key)) Console.WriteLine("Key cannot be null or empty.");
            key = Normalize(key);

            var i = 0;
            var j = 0;
            while (!string.IsNullOrEmpty(key))
            {
                used.Add(key[0]);
                Matrix[i, j] = key[0];
                if (j == 4) i++;
                j = (j + 1) % 5;
                key = key.Replace($"{key[0]}", "");
            }

            foreach (var letter in Abc.Where(letter => !used.Contains(letter)))
            {
                Matrix[i, j] = letter;
                if (j == 4) i++;
                j = (j + 1) % 5;
            }

            Console.WriteLine($"Message: {message}");
            message = Normalize(message);

            if (message.Length % 2 == 1) message = $"{message}x";

            Encrypt(ref message);
            Console.WriteLine($"Encrypted: {message}");

            Decrypt(ref message);
            Console.WriteLine($"Decrypted: {message}");
        }

        private static string Normalize(string input) => input?
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", "")
            .Replace("\t", "")
            .Replace("j", "i");

        private static void Encrypt(ref string message)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < message.Length; i += 2)
            {
                var first = message[i];
                var second = message[i + 1];
                if (first == second) second = 'x';
                IndexOf(first, out var firstI, out var firstJ);
                IndexOf(second, out var secondI, out var secondJ);
                if (firstI == secondI) //same line
                {
                    first = Matrix[firstI, (firstJ + 1) % 5];
                    second = Matrix[secondI, (secondJ + 1) % 5];
                }
                else if (firstJ == secondJ)
                {
                    first = Matrix[(firstI + 1) % 5, firstJ];
                    second = Matrix[(secondI + 1) % 5, secondJ];
                }
                else
                {
                    first = Matrix[firstI, secondJ];
                    second = Matrix[secondI, firstJ];
                }

                builder.AppendFormat("{0}{1}", first, second);
            }

            message = builder.ToString();
        }

        private static void Decrypt(ref string message)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < message.Length; i += 2)
            {
                var first = message[i];
                var second = message[i + 1];
                IndexOf(first, out var firstI, out var firstJ);
                IndexOf(second, out var secondI, out var secondJ);
                if (firstI == secondI) //same line
                {
                    first = Matrix[firstI, (firstJ + 4) % 5];
                    second = Matrix[secondI, (secondJ + 4) % 5];
                }
                else if (firstJ == secondJ)
                {
                    first = Matrix[(firstI + 4) % 5, firstJ];
                    second = Matrix[(secondI + 4) % 5, secondJ];
                }
                else
                {
                    first = Matrix[firstI, secondJ];
                    second = Matrix[secondI, firstJ];
                }

                builder.AppendFormat("{0}{1}", first, second);
            }

            if (builder.Length % 2 == 0 && builder[^1] == 'x') builder.Remove(message.Length - 1, 1);
            message = builder.ToString();
        }
        
        private static void IndexOf(char symbol, out int i, out int j)
        {
            j = 0;
            for (i = 0; i < 5; i++)
            {
                for (j = 0; j < 5; j++)
                {
                    if (Matrix[i,j] == symbol) return;
                }
            }
        }
    }
}