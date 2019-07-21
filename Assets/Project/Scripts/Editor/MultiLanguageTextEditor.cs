using UnityEditor;
using UnityEngine;
using Project.Scripts.UIComponents;
using Project.Scripts.Utils.Definitions;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        /// 基本的にUnityEngine.UI.Textの仕様を参考する
        /// </summary>
        /// <param name="menuCommand">コマンドの対象とするゲームオブジェクトを抽出するために使う</param>
        [MenuItem("GameObject/UI/Multi Language Text")]
        public static void CreateMultiLanguageText(MenuCommand menuCommand)
        {
            GameObject selection = menuCommand.context as GameObject;
            GameObject obj = new GameObject("Multi Language Text", typeof(MultiLanguageText));
            GameObject parent = null;

            // 選択なしの状態：Canvasを探してその下に置く
            if (selection == null) {
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas != null) {
                    GameObjectUtility.SetParentAndAlign(obj, canvas.gameObject);
                }
                else {　// シーンにCanvasがない場合
                    // キャンバス作成
                    parent = CreateAndInitializeCanvas();
                    // レイヤーをUIに設定する
                    parent.layer = LayerMask.NameToLayer("UI");
                    // 親子関係を設定する
                    GameObjectUtility.SetParentAndAlign(obj, parent);
                }
            }
            else {　// 何かを選択している場合
                if (selection.GetComponentInParent<Canvas>()) {
                    // 選択したオブジェクトはキャンバスの子オブジェクトだったらそのまま選択したオブジェクトの下に置く
                    GameObjectUtility.SetParentAndAlign(obj, selection);
                }
                else {
                    // 選択したオブジェクトがCanvasを持っていなかったらCanvasを作成
                    parent = CreateAndInitializeCanvas();
                    GameObjectUtility.SetParentAndAlign(parent, selection);

                    parent.layer = LayerMask.NameToLayer("UI");
                    GameObjectUtility.SetParentAndAlign(obj, parent);
                }
            }

            // EventSystemを一緒にUndoするためにUndo Group使う
            Undo.RecordObject(null, "Create Multi Language Text");
            int groupID = Undo.GetCurrentGroup();

            // EventSystem がなければ作成
            if (FindObjectOfType<EventSystem>() == null) {
                GameObject eventSystem = new GameObject("Event System",
                    typeof(EventSystem),
                    typeof(StandaloneInputModule)
                );
                Undo.RegisterCreatedObjectUndo(eventSystem, "Create Event System");
            }

            // Undoに登録
            Undo.RegisterCreatedObjectUndo(parent == null ? obj : parent, "Create Multi Language Text");
            Undo.CollapseUndoOperations(groupID);
            Selection.activeGameObject = obj;
        }

        /// <summary>
        /// キャンバスの作成とデフォルトの設定
        /// </summary>
        /// <returns>作成したキャンバスゲームオブジェクト</returns>
        private static GameObject CreateAndInitializeCanvas () {
            GameObject obj = new GameObject("Canvas");
            Canvas canvas = obj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = obj.AddComponent<CanvasScaler>();
            GraphicRaycaster raycaster = obj.AddComponent<GraphicRaycaster>();

            return obj;
        }
    }
}
