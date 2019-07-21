using UnityEditor;
using UnityEngine;
using Project.Scripts.UIComponents;
using Project.Scripts.Utils.Definitions;
using UnityEngine.UI;

namespace Project.Scripts.Editor
{
    [CustomEditor(typeof(MultiLanguageText))]
    public class MultiLanguageTextEditor : UnityEditor.UI.TextEditor
    {
        #region GUI_STRINGS
        private static GUIContent TEXT_INDEX = new GUIContent("Text Index", "Text Index");
        #endregion

        MultiLanguageText uiText;

        public override void OnInspectorGUI()
        {
            uiText = (MultiLanguageText)target;

            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            uiText.TextIndex = (TextIndex)EditorGUILayout.EnumPopup(TEXT_INDEX, uiText.TextIndex);
            if (EditorGUI.EndChangeCheck())
            {
                uiText.text = "test text";
                // uiText.text = TextUtility.GetText(uiText.TextIndex);
            }
        }

        /// <summary>
        /// 新しく<code>MultiLanguageText</code>をシーンに追加する
        /// </summary>
        [MenuItem("GameObject/UI/Multi Language Text")]
        public static void CreateMultiLanguageText(MenuCommand menuCommand)
        {
            GameObject selection = menuCommand.context as GameObject;
            GameObject obj = new GameObject("Multi Language Text");

            // 選択なしの状態
            if (selection == null) {
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas != null) {
                    GameObjectUtility.SetParentAndAlign(obj, canvas.gameObject);
                }
                else {
                    GameObject parent = new GameObject("Canvas");
                    CreateAndInitializeCanvas(parent);
                    parent.layer = LayerMask.NameToLayer("UI");
                    GameObjectUtility.SetParentAndAlign(obj, parent);
                }
            }
            else {
                if (selection.GetComponentInParent<Canvas>()) {
                    GameObjectUtility.SetParentAndAlign(obj, selection);
                }
                else {
                    GameObject canvas = new GameObject("Canvas");
                    CreateAndInitializeCanvas(canvas);
                    GameObjectUtility.SetParentAndAlign(canvas, selection);

                    canvas.layer = LayerMask.NameToLayer("UI");
                    GameObjectUtility.SetParentAndAlign(obj, canvas);
                }
            }

            obj.AddComponent<MultiLanguageText>();

            // Undoに登録
            Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);
            // focus
            Selection.activeObject = obj;
        }

        private static GameObject CreateAndInitializeCanvas (GameObject obj) {
            Canvas canvas = obj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = obj.AddComponent<CanvasScaler>();
            GraphicRaycaster raycaster = obj.AddComponent<GraphicRaycaster>();
            
            return obj;
        }
    }
}
