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

            if (symbols.Contains("ENV_DEV")) return;

            symbols.Remove("ENV_PROD");
            Menu.SetChecked("Environment/PROD", false);

            symbols.Add("ENV_DEV");
            Menu.SetChecked("Environment/DEV", true);

            SetSymbols(symbols);
        }

        // 本番環境に切り替え
        [MenuItem("Environment/PROD")]
        public static void Prod(){
            var symbols = GetSymbols();

            if (symbols.Contains("ENV_PROD")) return;

            symbols.Remove("ENV_DEV");
            Menu.SetChecked("Environment/DEV", false);

            symbols.Add("ENV_PROD");
            Menu.SetChecked("Environment/PROD", true);

            SetSymbols(symbols);
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
