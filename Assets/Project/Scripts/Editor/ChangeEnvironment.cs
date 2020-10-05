using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Project.Scripts.Editor
{
    public static class ChangeEnvironment
    {
        // 開発環境に切り替え
        [MenuItem("Environment/DEV")]
        public static void Dev(){
            var symbols = GetSymbols();

            symbols.Remove("ENV_PROD");
            symbols.Add("ENV_DEV");

            SetSymbols(symbols);
        }

        [MenuItem("Environment/DEV", validate = true)]
        public static bool DevValidation()
        {
            var symbols = GetSymbols();

            var isDev = symbols.Contains("ENV_DEV");
            Menu.SetChecked("Environment/DEV", isDev);

            return !isDev;
        }

        // 本番環境に切り替え
        [MenuItem("Environment/PROD")]
        public static void Prod()
        {
            var symbols = GetSymbols();

            symbols.Remove("ENV_DEV");
            symbols.Add("ENV_PROD");

            SetSymbols(symbols);
        }

        [MenuItem("Environment/PROD", validate = true)]
        public static bool ProdValidation()
        {
            var symbols = GetSymbols();

            var isProd = symbols.Contains("ENV_PROD");
            Menu.SetChecked("Environment/PROD", isProd);

            return !isProd;
        }

        // 設定されているシンボル定義を取得する
        private static List<string> GetSymbols(){
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';').ToList();
        }

        // シンボル定義をセットする
        private static void SetSymbols(List<string> symbols){
            var symbolStr = string.Empty;
            symbols.ForEach(s => symbolStr += s + ";");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbolStr);
        }
    }
}
