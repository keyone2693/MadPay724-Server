using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Common
{
    public static class LoadFile
    {
        public static string LoadTextFile(string file)
        {
            if(file.Length < 5)
            {
                throw new ArgumentException("error" , "file");
            }

            return "file was correctly loaded";
        }
    }
}
