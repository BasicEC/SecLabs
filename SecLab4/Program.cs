using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace SecLab4
{
    public static class Program
    {
        private const string FilePath = "/home/basicec/text.txt";

        public static void Main(string[] args)
        {
            if (!File.Exists(FilePath))
            {
                Console.WriteLine($"Cannot find file {FilePath}");
                return;
            }

            using var sr = new StreamReader(FilePath);
            var message = sr.ReadToEnd();
            Console.Write($"Message: {message}");

            var bytes = Encoding.UTF8.GetBytes(message);
            Encrypt(bytes);
            Console.WriteLine($"Encrypted: {Encoding.UTF8.GetString(bytes)}");
            Decrypt(bytes);
            Console.WriteLine($"Decrypted: {Encoding.UTF8.GetString(bytes)}");
        }

        private static void Encrypt(byte[] message)
        {
            const string lsr1String = "111010010001011010010010010100101101001010001100100101011010010010011101001111";
            const string lsr2String = "100100100101000100100010101001001010100101001010100010101101010010100001100111";
            const string lsr3String = "10010010010010010010010100001001000101101010010001011010101001001001001011011101";
    
            var lsr1 = new BitArray(lsr1String.Select(_ => _ == '1').ToArray());
            var lsr2 = new BitArray(lsr2String.Select(_ => _ == '1').ToArray());
            var lsr3 = new BitArray(lsr3String.Select(_ => _ == '1').ToArray());

            for (var i = 0; i < message.Length; i++)
            {
                var resultLetter = 0;

                for (var mask = 0; mask < 8; mask++)
                {
                    var result = lsr2[0] && lsr3[0] ? 1 : 0;
                    resultLetter |= result << mask;

                    bool buff;
                    if (!lsr1[24])
                    {
                        buff = lsr2[0] ^ lsr2[2] ^ lsr2[5] ^ lsr2[6] ^ lsr2[77];
                        lsr2 = lsr2.RightShift(1);
                        lsr2[77] = buff;
                    }

                    if (!lsr1[77])
                    {
                        buff = lsr3[0] ^ lsr3[2] ^ lsr3[3] ^ lsr3[4] ^ lsr2[77];
                        lsr3 = lsr3.RightShift(1);
                        lsr3[79] = buff;
                    }

                    buff = lsr1[0] ^ lsr1[1] ^ lsr1[3] ^ lsr1[6] ^ lsr1[75];
                    lsr1 = lsr1.RightShift(1);
                    lsr1[77] = buff;
                }

                message[i] ^= (byte) resultLetter;
            }
        }

        private static void Decrypt(byte[] message)
        {
            Encrypt(message);
        }
    }
}