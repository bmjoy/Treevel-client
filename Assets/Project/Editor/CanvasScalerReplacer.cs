using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Treevel.Editor
{
    public class CanvasScalerSetting : ScriptableObject
    {
        [SerializeField] private Vector2 referenceResolution;

        [SerializeField, Range(0, 1)] private float matchWidthOrHeight;

        public Vector2 ReferenceResolution => referenceResolution;

        public float MatchWidthOrHeight => matchWidthOrHeight;
    }

    public class CanvasScalerReplacer : EditorWindow
    {
        /// <summary>
        /// CanvasScalerの変数 : 理想的なデバイスの大きさ
        /// </summary>
        private static SerializedProperty _referenceResolutionProp;

        /// <summary>
        /// CanvasScalerの変数 : 横幅と縦幅の調節の重み
        /// </summary>
        private static SerializedProperty _matchWidthOrHeightProp;

        /// <summary>
        /// 上部のToolタブに項目を増やす
        /// </summary>
        [MenuItem("Tools/Replace All CanvasScalers")]
        public static void ShowWindow()
        {
            GetWindow(typeof(CanvasScalerReplacer), true, "CanvasScaler Replacer");
            var obj = ScriptableObject.CreateInstance<CanvasScalerSetting>();
            var serializedObject = new UnityEditor.SerializedObject(obj);

            _referenceResolutionProp = serializedObject.FindProperty("referenceResolution");
            _matchWidthOrHeightProp = serializedObject.FindProperty("matchWidthOrHeight");
        }

        public void OnGUI()
        {
            EditorGUILayout.PropertyField(_referenceResolutionProp);
            EditorGUILayout.PropertyField(_matchWidthOrHeightProp);
            if (GUILayout.Button("Replace All CanvasScalers")) {
                ReplaceCanvasScalerInAllScene();
                // シーンに変更があることをUnity側に通知
                EditorSceneManager.MarkAllScenesDirty();
            }
        }

        /// <summary>
        /// 全てのシーンのCanvasScalerを変更する
        /// </summary>
        private static void ReplaceCanvasScalerInAllScene()
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
                // 開いているシーンのCanvasScalerの設定
                if (ReplaceCanvasScalerInScene()) EditorSceneManager.SaveScene(SceneManager.GetSceneByPath(path)); // 変更があれば保存
            }

            // プログレスバーの終了
            EditorUtility.ClearProgressBar();
            if (!string.IsNullOrEmpty(currentScene)) EditorSceneManager.OpenScene(currentScene); // はじめのシーンを再度開く
        }

        /// <summary>
        /// 現在開いているシーンの全てのCanvasScalerを変更する
        /// </summary>
        /// <returns></returns>
        private static bool ReplaceCanvasScalerInScene()
        {
            // シーン内の全てのCanvasオブジェクトの取得
            var canvases = Resources.FindObjectsOfTypeAll(typeof(Canvas)) as Canvas[];
            if (canvases.Length == 0) return false;
            foreach (var canvas in canvases) {
                // CanvasScalerコンポーネントを必ず持たせる
                if (canvas.GetComponentInChildren<CanvasScaler>() == null) {
                    canvas.gameObject.AddComponent<CanvasScaler>();
                }

                var canvasScaler = canvas.GetComponent<CanvasScaler>();
                // 設定の変更
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = _referenceResolutionProp.vector2Value;
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = _matchWidthOrHeightProp.floatValue;
            }

            return true;
        }
    }
}
