using System;
using Treevel.Common.Components.UIs;
using Treevel.Common.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TextEditor = UnityEditor.UI.TextEditor;

namespace Treevel.Editor
{
    [CustomEditor(typeof(MultiLanguageText))]
    public class MultiLanguageTextEditor : TextEditor
    {
        #region GUI_STRINGS

        private static readonly GUIContent _TEXT_INDEX = new GUIContent("Text Index", "Text Index");

        #endregion

        public override void OnInspectorGUI()
        {
            var uiText = (MultiLanguageText)target;

            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            ETextIndex currentTextIndex;
            if (!Enum.TryParse(uiText.IndexStr, out currentTextIndex)) {
                currentTextIndex = ETextIndex.Error;
            }

            uiText.TextIndex = (ETextIndex)EditorGUILayout.EnumPopup(_TEXT_INDEX, currentTextIndex);


            if (!EditorGUI.EndChangeCheck()) return;

            EditorUtility.SetDirty(serializedObject.targetObject);
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 新しく<code>MultiLanguageText</code>をシーンに追加する
        /// 基本的にUnityEngine.UI.Textの仕様を参考する
        /// </summary>
        /// <param name="menuCommand">コマンドの対象とするゲームオブジェクトを抽出するために使う</param>
        [MenuItem("GameObject/UI/Multi Language Text")]
        public static void CreateMultiLanguageText(MenuCommand menuCommand)
        {
            var selection = menuCommand.context as GameObject;
            var obj = new GameObject("Multi Language Text", typeof(MultiLanguageText));
            GameObject parent = null;

            // 選択なしの状態：Canvasを探してその下に置く
            if (selection == null) {
                var canvas = FindObjectOfType<Canvas>();
                if (canvas != null) {
                    GameObjectUtility.SetParentAndAlign(obj, canvas.gameObject);
                } else {
                    // シーンにCanvasがない場合
                    // キャンバス作成
                    parent = CreateAndInitializeCanvas();
                    // レイヤーをUIに設定する
                    parent.layer = LayerMask.NameToLayer("UI");
                    // 親子関係を設定する
                    GameObjectUtility.SetParentAndAlign(obj, parent);
                }
            } else {
                // 何かを選択している場合
                if (selection.GetComponentInParent<Canvas>()) {
                    // 選択したオブジェクトはキャンバスの子オブジェクトだったらそのまま選択したオブジェクトの下に置く
                    GameObjectUtility.SetParentAndAlign(obj, selection);
                } else {
                    // 選択したオブジェクトがCanvasを持っていなかったらCanvasを作成
                    parent = CreateAndInitializeCanvas();
                    GameObjectUtility.SetParentAndAlign(parent, selection);

                    parent.layer = LayerMask.NameToLayer("UI");
                    GameObjectUtility.SetParentAndAlign(obj, parent);
                }
            }

            // EventSystemを一緒にUndoするためにUndo Group使う
            Undo.RecordObject(obj, "Create Multi Language Text");
            var groupId = Undo.GetCurrentGroup();

            // EventSystem がなければ作成
            if (FindObjectOfType<EventSystem>() == null) {
                var eventSystem = new GameObject("Event System",
                                                 typeof(EventSystem),
                                                 typeof(StandaloneInputModule)
                );
                Undo.RegisterCreatedObjectUndo(eventSystem, "Create Event System");
            }

            // Undoに登録
            Undo.RegisterCreatedObjectUndo(parent == null ? obj : parent, "Create Multi Language Text");
            Undo.CollapseUndoOperations(groupId);
            Selection.activeGameObject = obj;
        }

        /// <summary>
        /// キャンバスの作成とデフォルトの設定
        /// </summary>
        /// <returns>作成したキャンバスゲームオブジェクト</returns>
        private static GameObject CreateAndInitializeCanvas()
        {
            var obj = new GameObject("Canvas");
            var canvas = obj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            obj.AddComponent<CanvasScaler>();
            obj.AddComponent<GraphicRaycaster>();

            return obj;
        }
    }
}
