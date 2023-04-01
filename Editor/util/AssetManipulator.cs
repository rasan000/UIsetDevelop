
using System;
using System.IO;
using UnityEngine;

namespace UIset.util
{

    /// <summary>
    /// アセットフォルダ操作用のクラスです
    /// </summary>
    class AssetManipulator
    {
        /// <summary>
        /// フォルダーを内容ごとコピーします
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        public static void CopyDirectoryRecursive(string sourcePath, string destinationPath)
        {
            // コピー先ディレクトリが存在しない場合は、作成します。
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            // ディレクトリ内のファイルをすべてコピーします。
            foreach (string filePath in Directory.GetFiles(sourcePath))
            {
                string destinationFilePath = Path.Combine(destinationPath, Path.GetFileName(filePath));
                File.Copy(filePath, destinationFilePath, true);
            }

            // サブディレクトリを再帰的にコピーします。
            foreach (string subDirectory in Directory.GetDirectories(sourcePath))
            {
                string destinationSubDirectory = Path.Combine(destinationPath, Path.GetFileName(subDirectory));
                CopyDirectoryRecursive(subDirectory, destinationSubDirectory);
            }
        }


    }
}