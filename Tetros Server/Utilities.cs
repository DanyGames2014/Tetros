using System;
using System.Security.Cryptography;
using System.Text;

namespace Tetros
{
    class Utilities
    {
        public static string ServerWebsocketKey(string clientKey)
        {
            string clientKeyMagic = string.Concat(clientKey, "258EAFA5-E914-47DA-95CA-C5AB0DC85B11").Trim();
            byte[] data = Encoding.UTF8.GetBytes(clientKeyMagic.Trim());
            return Convert.ToBase64String(SHA1.HashData(data));
        }

        public static bool[] ConvertByteToBoolArray(byte b)
        {
            bool[] result = new bool[8];

            for (int i = 0; i < 8; i++)
                result[i] = (b & (1 << i)) != 0;

            Array.Reverse(result);

            return result;
        }

        public static byte[] demask(byte[] toDemask, byte[] mask)
        {
            byte[] decoded = new byte[toDemask.Length];

            for (int i = 0; i < toDemask.Length; i++)
            {
                decoded[i] = (byte)(toDemask[i] ^ mask[i % 4]);
            }

            return decoded;
        }

        public static string opcodeText(int opcode)
        {
            switch (opcode)
            {
                case 0:
                    return "Continuation Frame";
                case 1:
                    return "Text Frame";
                case 2:
                    return "Binary Frame";
                case 8:
                    return "Close Frame";
                case 9:
                    return "Ping";
                case 10:
                    return "Pong";
                default:
                    return "Unknown";
            }
        }

        public static Frame decodeFrame(byte[] frame)
        {
            Frame result = new();

            /// First Byte - 0:FIN 1:RSV1 2:RSV2 3:RSV3 4-7:Opcode
            bool[] firstByte = ConvertByteToBoolArray(frame[0]);
            
            // FIN
            if(firstByte[0] == true)
            {
                result.FIN = true;
            }
            else
            {
                result.FIN = false;
                throw new UnhandledFrameFormatException();
            }

            // RSV
            if(firstByte[1] == false && firstByte[2] == false && firstByte[3] == false)
            {
                result.RSV1 = false;
                result.RSV2 = false;
                result.RSV3 = false;
            }
            else
            {
                result.RSV1 = firstByte[1];
                result.RSV2 = firstByte[2];
                result.RSV3 = firstByte[3];
                throw new UnhandledFrameFormatException();
            }

            // Opcode
            int opcode = 0;
            opcode += Convert.ToInt32(firstByte[7]) * 1;
            opcode += Convert.ToInt32(firstByte[6]) * 2;
            opcode += Convert.ToInt32(firstByte[5]) * 4;
            opcode += Convert.ToInt32(firstByte[4]) * 8;

            result.opcode = opcode;

            switch (opcode)
            {
                case 1: // Text Frame
                    break;

                case 8: // Close
                    //Console.WriteLine("Client closed the connection");
                    return result;

                default:
                    throw new UnknownOpcodeException();
            }

            /// Second Byte - 0:Mask 1-7:Length
            bool[] secondByte = ConvertByteToBoolArray(frame[1]);

            // Mask Bit
            result.MASK = secondByte[0];
            /*
            if(secondByte[0] == true)
            {
                result.MASK = true;
            }
            else
            {
                result.MASK = false;
            }
            */

            // Length
            int payloadLength;
            int maskKeyByte;
            switch(frame[1] - 128)
            {
                case 127:
                    maskKeyByte = 10;
                    // frame 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9
                    break;

                case 126:
                    // frame 2 + 3
                    maskKeyByte = 4;
                    payloadLength = 0;
                    int[] reverse7 = new int[8] { 7, 6, 5, 4, 3, 2, 1, 0 };
                    for (int i = 2; i < 4; i++)
                    {
                        bool[] len = ConvertByteToBoolArray(frame[i]);
                        for (int j = 7; j > -1; j--)
                        {
                            int newi = i - (i - 2) + (1 * (3 - i));
                            int value = Convert.ToInt32(Convert.ToInt32(Math.Pow(2, j)) * Convert.ToInt32(Math.Pow(256, newi - 2)));
                            payloadLength += Convert.ToInt32(len[reverse7[j]]) * value;
                        }
                    }
                    result.length = payloadLength;
                    break;

                default:
                    maskKeyByte = 2;
                    payloadLength = Convert.ToInt32(frame[1]-128);
                    result.length = payloadLength;
                    break;
            }

            // Masking Key
            int payloadDataByte = maskKeyByte;
            byte[] mask = new byte[4];
            if (result.MASK)
            {
                mask[0] = frame[maskKeyByte];
                mask[1] = frame[maskKeyByte + 1];
                mask[2] = frame[maskKeyByte + 2];
                mask[3] = frame[maskKeyByte + 3];
                payloadDataByte = maskKeyByte + 4;
            }

            // Payload Data
            byte[] payloadData = new byte[frame.Length - payloadDataByte];
            if (result.MASK)
            {
                byte[] maskedData = new byte[frame.Length - payloadDataByte];


                for (int i = 0; i < maskedData.Length ; i++)
                {
                    maskedData[i] = frame[payloadDataByte + i];
                }

                payloadData = demask(maskedData, mask);
            }
            else
            {
                for (int i = 0; i < payloadData.Length; i++)
                {
                    payloadData[i] = frame[payloadDataByte + i];
                }
            }
            result.payloadData = payloadData;

            //Console.WriteLine(result.ToString());

            return result;
        }

        public static byte[] encodeTextFrame(string text)
        {
            byte[] result = null;
            byte[] payloadData = Encoding.UTF8.GetBytes(text);

            byte firstByte = 0b10000001; // FIN 1 | RSV* 0 | Opcode 0001
            byte secondByte = 0b00000000; // MASK 0 | Length

            if(text.Length <= 125) // 0 - 125
            {
                secondByte += Convert.ToByte(text.Length);
                result = new byte[text.Length + 2];

                result[0] = firstByte;
                result[1] = secondByte;

                for (int i = 0; i < payloadData.Length; i++)
                {
                    result[i + 2] = payloadData[i];
                }

                return result;
            }
            else
            {
                if (text.Length > 125 && text.Length <= 65536) // Length byte = 126
                {
                    byte[] lengthBytes = lengthTo2Byte(text.Length);
                    result = new byte[text.Length + 4];
                    result[0] = firstByte;
                    secondByte += 126;
                    result[1] = secondByte;
                    result[2] = lengthBytes[0];
                    result[3] = lengthBytes[1];

                    for (int i = 0; i < payloadData.Length; i++)
                    {
                        result[i + 4] = payloadData[i];
                    }
                    return result;
                }
                else
                {
                    throw new UnhandledFrameFormatException();
                }
            }
        }

        public static byte[] encodePongFrame()
        {
            byte[] result = new byte[2];

            byte firstByte = 0b10000001; // FIN 1 | RSV* 0 | Opcode 0001
            byte secondByte = 0b00001010; // MASK 0 | Length 0

            result[0] = firstByte;
            result[1] = secondByte;

            return result;

            
        }

        public static byte[] lengthTo2Byte(int length)
        {
            byte[] result = new byte[2];
            byte firstByte = 0b00000000;
            byte secondByte = 0b00000000;

            if (length >= 32768) { firstByte  += 0b10000000; length -= 32768; }
            if (length >= 16384) { firstByte  += 0b01000000; length -= 16384; }
            if (length >= 8192)  { firstByte  += 0b00100000; length -= 8192; }
            if (length >= 4096)  { firstByte  += 0b00010000; length -= 4096; }
            if (length >= 2048)  { firstByte  += 0b00001000; length -= 2048; }
            if (length >= 1024)  { firstByte  += 0b00000100; length -= 1024; }
            if (length >= 512)   { firstByte  += 0b00000010; length -= 512; }
            if (length >= 256)   { firstByte  += 0b00000001; length -= 256; }
            if (length >= 128)   { secondByte += 0b10000000; length -= 128; }
            if (length >= 64)    { secondByte += 0b01000000; length -= 64; }
            if (length >= 32)    { secondByte += 0b00100000; length -= 32; }
            if (length >= 16)    { secondByte += 0b00010000; length -= 16; }
            if (length >= 8)     { secondByte += 0b00001000; length -= 8; }
            if (length >= 4)     { secondByte += 0b00000100; length -= 4; }
            if (length >= 2)     { secondByte += 0b00000010; length -= 2; }
            if (length >= 1)     { secondByte += 0b00000001; length -= 1; }

            result[0] = firstByte;
            result[1] = secondByte;
            return result;
        }

        public static string randomString(int length)
        {
            string validChars = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
            Random rnd = new();
            string result = string.Empty;

            for (int i = 0; i < length; i++)
            {
                int randomNum = rnd.Next(0, validChars.Length - 1);
                result += validChars[randomNum];
            }
            return result;
        }

        public static string stringToHash(string toHash)
        {
            byte[] data = Encoding.UTF8.GetBytes(toHash);
            byte[] result;

            SHA512 shaM = new SHA512Managed();

            result = shaM.ComputeHash(data);

            string hash = Encoding.UTF8.GetString(result);

            return hash;
        }
    }
}