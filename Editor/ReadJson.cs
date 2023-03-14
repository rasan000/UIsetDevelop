
using System;
using System.IO;

namespace HakoTools
{
    class ReadJson
    {
        public static string Read(string filepath)
        {
            string dt = "";
            using (StreamReader sr = new StreamReader(filepath))
            {
                dt = sr.ReadToEnd();
            }
            return dt;
        }
    }
}