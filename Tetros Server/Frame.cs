using System.Text;

namespace Tetros
{
    public class Frame
    {
        public bool FIN;
        public bool RSV1;
        public bool RSV2;
        public bool RSV3;
        public int opcode;
        public bool MASK;
        public int length;
        public byte[] maskKey;
        public byte[] maskedData;
        public byte[] payloadData;

        public Frame(bool FIN, bool RSV1, bool RSV2, bool RSV3, int opcode, bool MASK, int length, byte[] maskKey, byte[] maskedData, byte[] payloadData)
        {
            this.FIN = FIN;

            this.RSV1 = RSV1;
            this.RSV2 = RSV2;
            this.RSV3 = RSV3;

            this.MASK = MASK;

            this.opcode = opcode;
            
            this.length = length;

            this.maskKey = maskKey;
            this.maskedData = maskedData;
            this.payloadData = payloadData;
        }

        public Frame()
        {
        }

        public override string ToString()
        {
            if(payloadData.Length == 0)
            {
                return "FIN : " + FIN +
                   " | RSV1 : " + RSV1 +
                   " | RSV2 : " + RSV2 +
                   " | RSV3 : " + RSV3 +
                   " | MASK : " + MASK +
                   "\nOpcode : " + Utilities.opcodeText(opcode);
            }
            else
            {
                return "FIN : " + FIN +
                   " | RSV1 : " + RSV1 +
                   " | RSV2 : " + RSV2 +
                   " | RSV3 : " + RSV3 +
                   " | MASK : " + MASK +
                   "\nOpcode : " + Utilities.opcodeText(opcode) +
                   "\nData Length : " + length +
                   "\nData : " + Encoding.UTF8.GetString(payloadData);
            }
            
        }
    }
}
