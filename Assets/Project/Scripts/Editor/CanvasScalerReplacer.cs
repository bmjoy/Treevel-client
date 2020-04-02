using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using Project.Scripts.Settings;

namespace Project.Scripts.Editor
{public class CanvasScalerReplacer : UnityEditor.EditorWindow
    {
        private static SerializedProperty _referenceResolutionProp;
        private static SerializedProperty _matchWidthOrHeightProp;

        [MenuItem("Tools/Replace All CanvaseScalers")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CanvasScalerReplacer), true, "CanvasScaler Replacer");
            var obj = ScriptableObject.CreateInstance<CanvasScalerSetting>();
            var serializedObject = new UnityEditor.SerializedObject(obj);

            _referenceResolutionProp = serializedObject.FindProperty("referenceResolution");
            _matchWidthOrHeightProp = serializedObject.FindProperty("matchWidthOrHeight");
        }

        public void OnGUI()
        {
            EditorGUILayout.PropertyField(_referenceResolutionProp);
            EditorGUILayout.PropertyField(_matchWidthOrHeightProp);
            if (GUILayout.Button("Replace All Canvases"))
            {
                ReplaceCanvasScalerInAllScene((string path) => {return ReplaceCanvasScalerInScene();});
                // シーンに変更があることをUnity側に通知
                EditorSceneManager.MarkAllScenesDirty();
            }
        }

        private static bool ReplaceCanvasScalerInScene() {
            var canvases = Object.FindObjectsOfType(typeof(Canvas)) as Canvas[];
            if(canvases.Length == 0) return false;
            foreach (var canvas in canvases)
            {
                if(canvas.GetComponentInChildren<CanvasScaler>() == null)
                {
                    canvas.gameObject.AddComponent<CanvasScaler>();
                }
                var canvasScaler = canvas.GetComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = _referenceResolutionProp.vector2Value;
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = _matchWidthOrHeightProp.floatValue;
            }
            return true;
        }

        delegate bool OnScene(string path);
        private static void ReplaceCanvasScalerInAllScene(OnScene onScene) {
            string currentScene = EditorApplication.currentScene;
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new string[] {"Assets/Project"});
            for (int i = 0; i < sceneGuids.Length; i++) {
                string guid = sceneGuids[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                EditorUtility.DisplayProgressBar("", path, (float)i / (float)sceneGuids.Length);
                EditorApplication.OpenScene(path);
                Debug.Log(AssetDatabase.LoadMainAssetAtPath (path));
                if (onScene(path))
                    EditorApplication.SaveScene();
            }
            EditorUtility.ClearProgressBar();
            if (!string.IsNullOrEmpty(currentScene))
                EditorApplication.OpenScene(currentScene);
        }
    }
}
