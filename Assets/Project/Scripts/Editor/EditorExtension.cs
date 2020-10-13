using System;
using UnityEditor;
using UnityEngine;

namespace Treevel.Editor
{
    public static class EditorExtension
    {
        /// <summary>
        /// List、Arrayなどの配列型のPropertyを描画するメソッド
        /// </summary>
        /// <param name="property">描画する対象、配列である必要がある</param>
        /// <param name="action">配列に格納する要素に対する描画処理、nullの場合はEditorGUI.PropertyFieldを使う</param>
        public static void DrawArrayProperty(this UnityEditor.Editor editor, SerializedProperty property, Action<SerializedProperty, int> action = null)
        {
            if (!property.isArray)
                return;

            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.displayName);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Array.size"));
            DrawArrayPropertyImpl(property, action);
        }

        /// <summary>
        /// 配列の大きさを制限した配列を描画する
        /// <see cref="DrawArrayProperty(Editor, SerializedProperty, Action{SerializedProperty, int})"/>
        /// </summary>
        /// <param name="property"></param>
        /// <param name="size"></param>
        /// <param name="action"></param>
        public static void DrawFixedSizeArrayProperty(this UnityEditor.Editor editor, SerializedProperty property, int size, Action<SerializedProperty, int> action = null)
        {
            if (!property.isArray)
                return;

            property.arraySize = size;
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.displayName);
            GUI.enabled = false;
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Array.size"));
            GUI.enabled = true;
            DrawArrayPropertyImpl(property, action);
        }

        private static void DrawArrayPropertyImpl(SerializedProperty property, Action<SerializedProperty, int> action = null)
        {
            if (!property.isExpanded) return;

            for (var i = 0 ; i < property.arraySize ; ++i) {
                var arrayElementProperty = property.GetArrayElementAtIndex(i);

                if (action != null) {
                    action.Invoke(arrayElementProperty, i);
                } else {
                    EditorGUILayout.PropertyField(arrayElementProperty, new GUIContent(arrayElementProperty.displayName));
                }
            }
        }
    }
}
