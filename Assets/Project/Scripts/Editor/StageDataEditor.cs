using UnityEngine;
using UnityEditor;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;

[CustomEditor(typeof(StageData))]
[CanEditMultipleObjects]
public class StageDataEditor : Editor
{
    private SerializedProperty _panelDatasProp;

    public void OnEnable()
    {
        _panelDatasProp = serializedObject.FindProperty("panels");
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("id"));
        _panelDatasProp.isExpanded = EditorGUILayout.Foldout(_panelDatasProp.isExpanded, new GUIContent("Panels"));
        if (_panelDatasProp.isExpanded) {
            if (_panelDatasProp.isArray) {
                EditorGUILayout.PropertyField(_panelDatasProp.FindPropertyRelative("Array.size"));
                for (int i = 0 ; i < _panelDatasProp.arraySize ; i++) {
                    SerializedProperty panelDataProp = _panelDatasProp.GetArrayElementAtIndex(i);
                    SerializedProperty panelPosProp = panelDataProp.FindPropertyRelative("position");
                    SerializedProperty panelTypeProp = panelDataProp.FindPropertyRelative("type");

                    panelDataProp.isExpanded = EditorGUILayout.Foldout(panelDataProp.isExpanded, $"Panel {i + 1}");
                    if (panelDataProp.isExpanded) {
                        using (var checkScope = new EditorGUI.ChangeCheckScope()) {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(panelPosProp);
                            panelTypeProp.enumValueIndex = (int)(EPanelType)EditorGUILayout.EnumPopup(new GUIContent("Type"), (EPanelType)panelTypeProp.enumValueIndex);

                            if (checkScope.changed) {
                                serializedObject.ApplyModifiedProperties();
                                serializedObject.Update();
                            }

                            switch ((EPanelType)panelTypeProp.enumValueIndex) {
                                case EPanelType.Number: {
                                        SerializedProperty numberProp = panelDataProp.FindPropertyRelative("number");
                                        SerializedProperty targetPosProp = panelDataProp.FindPropertyRelative("targetPos");
                                        EditorGUILayout.PropertyField(numberProp);
                                        EditorGUILayout.PropertyField(targetPosProp);
                                    }
                                    break;

                                case EPanelType.LifeNumber: {
                                        SerializedProperty numberProp = panelDataProp.FindPropertyRelative("number");
                                        SerializedProperty targetPosProp = panelDataProp.FindPropertyRelative("targetPos");
                                        SerializedProperty lifeProp = panelDataProp.FindPropertyRelative("life");
                                        EditorGUILayout.PropertyField(numberProp);
                                        EditorGUILayout.PropertyField(targetPosProp);
                                        EditorGUILayout.PropertyField(lifeProp);
                                    }
                                    break;
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
