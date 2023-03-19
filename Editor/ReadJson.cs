
using System;
using System.IO;

namespace UIset.util
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