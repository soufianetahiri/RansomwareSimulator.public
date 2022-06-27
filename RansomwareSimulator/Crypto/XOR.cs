using RansomwareSimulator.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RansomwareSimulator.Crypto
{
  public static class XOR
    {
      public static void XORtFile(string inputFile, string outputFile)
        {
            using var fin = new FileStream(inputFile, FileMode.Open);
            using var fout = new FileStream(outputFile, FileMode.Create);
            byte[] buffer = new byte[4096];
            while (true)
            {
                int bytesRead = fin.Read(buffer);
                if (bytesRead == 0)
                    break;
                EncryptBytes(buffer, bytesRead);
                fout.Write(buffer, 0, bytesRead);
            }
        }
        private static  void EncryptBytes(byte[] buffer, int count)
        {
            for (int i = 0; i < count; i++)
                buffer[i] = (byte)(buffer[i] ^ Consts.Secret);
        }
    }
}
