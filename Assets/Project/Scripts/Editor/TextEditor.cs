using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using Project.Scripts.UIComponents;
using System.Linq;

namespace Project.Scripts.Editor
{
    public class TextSetting : ScriptableObject
    {
        [SerializeField] private Font font;

        public Font Font => font;
    }

    public class TextEditor : UnityEditor.EditorWindow
    {
        /// <summary>
        /// フォント
        /// </summary>
        private static SerializedProperty _font;

        /// <summary>
        /// フォントサイズ
        /// </summary>
        private const int FONT_MIN = 40;

        /// <summary>
        /// 上部のToolタブに項目を増やす
        /// </summary>
        [MenuItem("Tools/Font Editor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(TextEditor), true, "Text Editor");
            var obj = ScriptableObject.CreateInstance<TextSetting>();
            var serializedObject = new UnityEditor.SerializedObject(obj);

            _font = serializedObject.FindProperty("font");
        }

        public void OnGUI()
        {
            EditorGUILayout.PropertyField(_font);

            // Fontが設定されていない時はボタンを押せない
            if(_font.objectReferenceValue as Font == null)
                EditorGUI.BeginDisabledGroup(true);

            if (GUILayout.Button("Replace All Text"))
            {
                ReplaceTextInAllScene();
                // シーンに変更があることをUnity側に通知
                EditorSceneManager.MarkAllScenesDirty();
            }

            EditorGUI.BeginDisabledGroup(false);
        }

        /// <summary>
        /// 全てのシーンのTextを変更する
        /// </summary>
        private static void ReplaceTextInAllScene()
        {
            // 現在のシーン
            string currentScene = EditorSceneManager.GetActiveScene().path;
            // プロジェクト内の全てのシーン名を取得
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new string[] { "Assets/Project" });
            for (int i = 0; i < sceneGuids.Length; i++)
            {
                string guid = sceneGuids[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                // プログレスバーを表示
                EditorUtility.DisplayProgressBar("", path, (float)i / (float)sceneGuids.Length);
                // シーンを開く
                EditorSceneManager.OpenScene(path);
                Debug.Log(AssetDatabase.LoadMainAssetAtPath(path));
                // 開いているシーンのTextの設定
                if (ReplaceTextInScene())
                    // 変更があれば保存
                    EditorSceneManager.SaveScene(EditorSceneManager.GetSceneByPath(path));
            }
            // プログレスバーの終了
            EditorUtility.ClearProgressBar();
            if (!string.IsNullOrEmpty(currentScene))
                // はじめのシーンを再度開く
                EditorSceneManager.OpenScene(currentScene);
        }

        /// <summary>
        /// 現在開いているシーンの全てのTextを変更する
        /// </summary>
        /// <returns></returns>
        private static bool ReplaceTextInScene()
        {
            // シーン内の全てのMultiLanguageTextオブジェクトの取得
            var texts = Resources.FindObjectsOfTypeAll(typeof(Text)) as Text[];
            if (texts.Count() == 0) return false;
            foreach (var multiLanguageText in texts)
            {
                if (multiLanguageText.GetType() != typeof(MultiLanguageText))
                {
                    // TextであるがMultiLanguageTextではない
                    Debug.Log("\"" + multiLanguageText.text + "\" is not MultiLanguageText.");
                }
                else
                {
                    // 設定の変更
                    multiLanguageText.font = _font.objectReferenceValue as Font;
                    // デフォルト設定
                    multiLanguageText.fontStyle = FontStyle.Normal;
                    multiLanguageText.fontSize = FONT_MIN;
                    multiLanguageText.supportRichText = true;
                    multiLanguageText.alignment = TextAnchor.MiddleCenter;
                    multiLanguageText.alignByGeometry = false;
                    multiLanguageText.horizontalOverflow = HorizontalWrapMode.Wrap;
                    multiLanguageText.verticalOverflow = VerticalWrapMode.Truncate;
                    multiLanguageText.resizeTextForBestFit = true;
                    multiLanguageText.raycastTarget = false;
                    multiLanguageText.resizeTextMinSize = Mathf.Max(multiLanguageText.resizeTextMinSize, FONT_MIN);
                    multiLanguageText.resizeTextMaxSize = Mathf.Max(multiLanguageText.resizeTextMaxSize, FONT_MIN);

                    EditorUtility.SetDirty(multiLanguageText);
                }
            }
            return true;
        }
    }
}
