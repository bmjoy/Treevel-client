using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Treevel.Common.Components.UIs;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Treevel.Editor
{
    public class TextChecker : EditorWindow
    {
        /// <summary>
        /// 全てのTextが参照すべきprefabのId
        /// </summary>
        private static readonly List<long> _FILE_ID = new List<long> {
            9178724915984365835, // BaseText
            8335351129932690981, // BaseMultiLanguage
        };

        /// <summary>
        /// 全てのシーンのTextを検証する
        /// </summary>
        [MenuItem("Tools/Text Checker")]
        private static void CheckTextInAllScene()
        {
            // 現在のシーン
            var currentScene = SceneManager.GetActiveScene().path;
            // プロジェクト内の全てのシーン名を取得
            var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Project" });
            for (var i = 0; i < sceneGuids.Length; i++) {
                var guid = sceneGuids[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                // プログレスバーを表示
                EditorUtility.DisplayProgressBar("", path, i / (float)sceneGuids.Length);
                // シーンを開く
                EditorSceneManager.OpenScene(path);
                Debug.Log(AssetDatabase.LoadMainAssetAtPath(path));
                // 開いているシーンのTextの検証
                CheckTextInScene();
            }

            // プログレスバーの終了
            EditorUtility.ClearProgressBar();
            if (!string.IsNullOrEmpty(currentScene))
                // はじめのシーンを再度開く
                EditorSceneManager.OpenScene(currentScene);
        }

        /// <summary>
        /// 現在開いているシーンの全てのTextを検証する
        /// </summary>
        /// <returns></returns>
        private static void CheckTextInScene()
        {
            // scene内の全てのTextオブジェクトを取得
            var textObjs = Resources.FindObjectsOfTypeAll(typeof(Text)).Select(t => t as Text)
                .Where(t => t != null && t.hideFlags != HideFlags.NotEditable &&
                            t.hideFlags != HideFlags.HideAndDontSave && t.hideFlags != HideFlags.HideInHierarchy);

            foreach (var textObj in textObjs) {
                if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(textObj) == null) {
                    // prefabではない
                    Debug.LogWarning("\"" + textObj.text + "\" is not a prefab.");
                    continue;
                }

                // 基になったprefabのfileIdを取得する
                var parentPrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(textObj);
                var inspectorModeInfo =
                    typeof(SerializedObject).GetProperty("inspectorMode",
                                                         BindingFlags.NonPublic | BindingFlags.Instance);

                var serializedObject = new SerializedObject(parentPrefab);
                if (inspectorModeInfo != null) inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);

                var localIdProp = serializedObject.FindProperty("m_LocalIdentfierInFile"); //note the misspelling!

                var localId = localIdProp.longValue;

                if (!_FILE_ID.Contains(localId)) {
                    // 指定したprefabではない
                    Debug.LogWarning("\"" + textObj.text + "\" is made from an uncertain prefab.");
                    continue;
                }

                // MultiLanguageTextかどうか
                if (textObj.GetType() == typeof(MultiLanguageText)) continue;

                var targetText = textObj.text;
                if (!targetText.All(char.IsDigit)) {
                    // 数字ではない
                    Debug.LogWarning("\"" + targetText + "\" should be fixed to MultiLanguageText.");
                }

                if (targetText.EndsWith(" ") || targetText.EndsWith("\n") || targetText.EndsWith("\r") ||
                    targetText.EndsWith("\r\n")) {
                    // 空白もしくは改行が末尾にある
                    Debug.Log("<color=gray>\"" + targetText + "\" ends with a space or \\n</color>");
                }
            }
        }
    }
}
