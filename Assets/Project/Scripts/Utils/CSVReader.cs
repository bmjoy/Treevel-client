using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Project.Scripts.Utils
{
    public static class CSVReader
    {

        /// <summary>
        /// CSVファイルを読み込むメソッド
        /// 使い方:
        /// <code>
        /// CSVReader.LoadCSV(path_to_read);
        /// </code>
        /// </summary>
        /// <param name="path">読み込むcsvファイルのパス</param>
        /// <returns>文字列の二次元配列</returns>
        public static List<string[]> LoadCSV(string path)
        {
            List<string[]> result = new List<string[]>();
            TextAsset csvFile = Resources.Load<TextAsset>(path);

            StringReader stringReader = new StringReader(csvFile.text);

            while (stringReader.Peek() != -1) {
                string line = stringReader.ReadLine();
                result.Add(line.Split(','));
            }

            return result;
        }
    }

}
