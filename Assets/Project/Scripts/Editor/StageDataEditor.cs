using UnityEngine;
using UnityEditor;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;

[CustomEditor(typeof(StageData))]
[CanEditMultipleObjects]
public class StageDataEditor : Editor
{
    private SerializedProperty _panelDatasProp;
    private SerializedProperty _bulletGroupDatasProp;

    public void OnEnable()
    {
        _panelDatasProp = serializedObject.FindProperty("panels");
        _bulletGroupDatasProp = serializedObject.FindProperty("bulletGroups");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("id"));
        _panelDatasProp.isExpanded = EditorGUILayout.Foldout(_panelDatasProp.isExpanded, new GUIContent("Panels"));
        EditorGUILayout.PropertyField(_panelDatasProp.FindPropertyRelative("Array.size"));
        if (_panelDatasProp.isExpanded) {
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

        _bulletGroupDatasProp.isExpanded = EditorGUILayout.Foldout(_bulletGroupDatasProp.isExpanded, new GUIContent("Bullet Groups"));
        EditorGUILayout.PropertyField(_bulletGroupDatasProp.FindPropertyRelative("Array.size"));
        if (_bulletGroupDatasProp.isExpanded) {
            for (int i = 0 ; i < _bulletGroupDatasProp.arraySize ; i++) {
                SerializedProperty bulletGroupDataProp = _bulletGroupDatasProp.GetArrayElementAtIndex(i);

                bulletGroupDataProp.isExpanded = EditorGUILayout.Foldout(bulletGroupDataProp.isExpanded, $"Bullet Group {i + 1}");
                if (bulletGroupDataProp.isExpanded) {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(bulletGroupDataProp.FindPropertyRelative("appearTime"));
                    EditorGUILayout.PropertyField(bulletGroupDataProp.FindPropertyRelative("interval"));
                    EditorGUILayout.PropertyField(bulletGroupDataProp.FindPropertyRelative("loop"));

                    SerializedProperty bulletListProp = bulletGroupDataProp.FindPropertyRelative("bullets");

                    bulletListProp.isExpanded = EditorGUILayout.Foldout(bulletListProp.isExpanded, "Bullets");
                    EditorGUILayout.PropertyField(bulletListProp.FindPropertyRelative("Array.size"));
                    if (bulletListProp.isExpanded) {
                        for (int j = 0; j < bulletListProp.arraySize; j++) {
                            SerializedProperty bulletDataProp = bulletListProp.GetArrayElementAtIndex(i);

                            bulletDataProp.isExpanded = EditorGUILayout.Foldout(bulletDataProp.isExpanded, $"Bullet {j + 1}");
                            if (bulletDataProp.isExpanded) {
                                SerializedProperty bulletTypeProp = bulletDataProp.FindPropertyRelative("type");
                                SerializedProperty directionProp = bulletDataProp.FindPropertyRelative("direction");
                                SerializedProperty lineProp = bulletDataProp.FindPropertyRelative("line");
                                bulletTypeProp.enumValueIndex = (int)(EBulletType)EditorGUILayout.EnumPopup(new GUIContent("Type"), (EBulletType)bulletTypeProp.enumValueIndex);

                                EditorGUILayout.PropertyField(bulletDataProp.FindPropertyRelative("ratio"));
                                switch ((EBulletType)bulletTypeProp.enumValueIndex) {
                                    case EBulletType.NormalCartridge: {
                                            directionProp.intValue = (int)(ECartridgeDirection)EditorGUILayout.EnumPopup(
                                                label: new GUIContent("Direction"),
                                                selected: (ECartridgeDirection)directionProp.intValue,
                                                checkEnabled: (eType) => { // ランダムは選択不能にする
                                                    return (ECartridgeDirection)eType != ECartridgeDirection.Random;
                                                },
                                                includeObsolete: false
                                            );

                                            switch ((ECartridgeDirection)directionProp.intValue) {
                                                case ECartridgeDirection.ToLeft:
                                                case ECartridgeDirection.ToRight:
                                                    lineProp.intValue = (int)(ERow)EditorGUILayout.EnumPopup(new GUIContent("Row"), (ERow)lineProp.intValue);
                                                    break;
                                                case ECartridgeDirection.ToBottom:
                                                case ECartridgeDirection.ToUp:
                                                    lineProp.intValue = (int)(EColumn)EditorGUILayout.EnumPopup(new GUIContent("Column"), (EColumn)lineProp.intValue);
                                                    break;
                                            }
                                            break;
                                        }
                                    case EBulletType.RandomCartridge: {
                                        directionProp.intValue = (int)(ECartridgeDirection.Random);
                                        directionProp.intValue = (int)(ECartridgeDirection)EditorGUILayout.EnumPopup(
                                                label: new GUIContent("Direction"),
                                                selected: (ECartridgeDirection)directionProp.intValue,
                                                checkEnabled: (eType) => {
                                                    return (ECartridgeDirection)eType == ECartridgeDirection.Random;
                                                },
                                                includeObsolete: false
                                            );
                                        EditorGUILayout.PropertyField(bulletDataProp.FindPropertyRelative("randomCartridgeDirection"));
                                        EditorGUILayout.PropertyField(bulletDataProp.FindPropertyRelative("randomRow"));
                                        EditorGUILayout.PropertyField(bulletDataProp.FindPropertyRelative("randomColumn"));

                                        break;
                                    }
                                }
                            }
                        }
                    }

                    EditorGUI.indentLevel--;
                }
            }
        }

        // Set object dirty, this will make it be saved after saving the project.
        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(serializedObject.targetObject);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
