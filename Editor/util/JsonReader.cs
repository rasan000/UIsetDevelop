
using System;
using System.IO;

namespace UIset.util
{
    /// <summary>
    /// Json読み込み用クラス
    /// ファイルパスを指定して、Jsonを読み込みます
    /// </summary>
    class JsonReader
    {
        public static string ReadJson(string filepath)
        {
            string jsonObject = "";
            StreamReader sr = new StreamReader(filepath);
            {
                jsonObject = sr.ReadToEnd();
            }
            return jsonObject;
        }
    }
}