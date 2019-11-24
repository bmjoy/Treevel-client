﻿using UnityEngine;
using UnityEditor;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;
using System;

[CustomEditor(typeof(StageData))]
[CanEditMultipleObjects]
public class StageDataEditor : Editor
{
    private SerializedProperty _panelDatasProp;
    private SerializedProperty _bulletGroupDatasProp;

    private StageData _src;
    public void OnEnable()
    {
        _src = target as StageData;
        _panelDatasProp = serializedObject.FindProperty("panels");
        _bulletGroupDatasProp = serializedObject.FindProperty("bulletGroups");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("id"));

        DrawPanelList();

        DrawBulletGroupList();

        // Set object dirty, this will make it be saved after saving the project.
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(_src, _src.name);
            EditorUtility.SetDirty(serializedObject.targetObject);
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawPanelList()
    {
        _panelDatasProp.isExpanded = EditorGUILayout.Foldout(_panelDatasProp.isExpanded, new GUIContent("Panels"));
        EditorGUILayout.PropertyField(_panelDatasProp.FindPropertyRelative("Array.size"));
        if (_panelDatasProp.isExpanded) {
            for (int i = 0 ; i < _panelDatasProp.arraySize ; i++) {
                SerializedProperty panelDataProp = _panelDatasProp.GetArrayElementAtIndex(i);
                SerializedProperty panelPosProp = panelDataProp.FindPropertyRelative("initPos");
                SerializedProperty panelTypeProp = panelDataProp.FindPropertyRelative("type");

                panelDataProp.isExpanded = EditorGUILayout.Foldout(panelDataProp.isExpanded, $"Panel {i + 1}");
                if (panelDataProp.isExpanded) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(panelPosProp);
                    panelTypeProp.enumValueIndex = (int)(EPanelType)EditorGUILayout.EnumPopup(new GUIContent("Type"), (EPanelType)panelTypeProp.enumValueIndex);

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
    private void DrawBulletGroupList()
    {
        DrawArrayProperty(_bulletGroupDatasProp, (bulletGroupDataProp, index) => {
            bulletGroupDataProp.isExpanded = EditorGUILayout.Foldout(bulletGroupDataProp.isExpanded, $"Bullet Group {index + 1}");
            if (bulletGroupDataProp.isExpanded) {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(bulletGroupDataProp.FindPropertyRelative("appearTime"));
                EditorGUILayout.PropertyField(bulletGroupDataProp.FindPropertyRelative("interval"));
                EditorGUILayout.PropertyField(bulletGroupDataProp.FindPropertyRelative("loop"));

                SerializedProperty bulletListProp = bulletGroupDataProp.FindPropertyRelative("bullets");
                DrawArrayProperty(bulletListProp, (bulletDataProp, index2) => {
                    bulletDataProp.isExpanded = EditorGUILayout.Foldout(bulletDataProp.isExpanded, $"Bullet {index2 + 1}");
                    if (bulletDataProp.isExpanded) {
                        SerializedProperty bulletTypeProp = bulletDataProp.FindPropertyRelative("type");
                        SerializedProperty directionProp = bulletDataProp.FindPropertyRelative("direction");
                        SerializedProperty lineProp = bulletDataProp.FindPropertyRelative("line");
                        bulletTypeProp.enumValueIndex = (int)(EBulletType)EditorGUILayout.EnumPopup(
                                label: new GUIContent("Type"),
                                selected: (EBulletType)bulletTypeProp.enumValueIndex,
                                checkEnabled: (eType) => (int)(EBulletType)eType < (int)EBulletType.NormalHole, // TODO: 実装したら外す
                                includeObsolete: false
                            );

                        EditorGUILayout.PropertyField(bulletDataProp.FindPropertyRelative("ratio"));
                        switch ((EBulletType)bulletTypeProp.enumValueIndex) {
                            case EBulletType.NormalCartridge: {
                                    directionProp.intValue = (int)(ECartridgeDirection)EditorGUILayout.EnumPopup(
                                            label: new GUIContent("Direction"),
                                            selected: (ECartridgeDirection)directionProp.intValue,
                                            checkEnabled: (eType) => (ECartridgeDirection)eType != ECartridgeDirection.Random, // ランダムは選択不能にする
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
                            case EBulletType.RandomNormalCartridge: {
                                    directionProp.intValue = (int)(ECartridgeDirection.Random);
                                    directionProp.intValue = (int)(ECartridgeDirection)EditorGUILayout.EnumPopup(
                                            label: new GUIContent("Direction"),
                                            selected: (ECartridgeDirection)directionProp.intValue,
                                            checkEnabled: (eType) => (ECartridgeDirection)eType == ECartridgeDirection.Random,
                                            includeObsolete: false
                                        );
                                    DrawFixedSizeArrayProperty(bulletDataProp.FindPropertyRelative("randomCartridgeDirection"), Enum.GetValues(typeof(ECartridgeDirection)).Length - 1);
                                    DrawFixedSizeArrayProperty(bulletDataProp.FindPropertyRelative("randomRow"), Enum.GetValues(typeof(ERow)).Length - 1);
                                    DrawFixedSizeArrayProperty(bulletDataProp.FindPropertyRelative("randomColumn"), Enum.GetValues(typeof(EColumn)).Length - 1);
                                    break;
                                }
                            case EBulletType.TurnCartridge: {
                                    if (directionProp.intValue == (int)ECartridgeDirection.Random) // 方向がランダムの場合強制に変える
                                        directionProp.intValue = (int)ECartridgeDirection.ToLeft;

                                    directionProp.intValue = (int)(ECartridgeDirection)EditorGUILayout.EnumPopup(
                                            label: new GUIContent("Direction"),
                                            selected: (ECartridgeDirection)directionProp.intValue,
                                            checkEnabled: (eType) => (ECartridgeDirection)eType != ECartridgeDirection.Random,
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

                                    // TODO pair constraint of turnDirections/tunrLines
                                    DrawArrayProperty(bulletDataProp.FindPropertyRelative("turnDirections"));
                                    DrawArrayProperty(bulletDataProp.FindPropertyRelative("turnLines"));
                                    break;
                                }
                            case EBulletType.RandomTurnCartridge: {
                                    directionProp.intValue = (int)(ECartridgeDirection.Random);
                                    directionProp.intValue = (int)(ECartridgeDirection)EditorGUILayout.EnumPopup(
                                            label: new GUIContent("Direction"),
                                            selected: (ECartridgeDirection)directionProp.intValue,
                                            checkEnabled: (eType) => (ECartridgeDirection)eType == ECartridgeDirection.Random,
                                            includeObsolete: false
                                        );
                                    DrawFixedSizeArrayProperty(bulletDataProp.FindPropertyRelative("randomCartridgeDirection"), Enum.GetValues(typeof(ECartridgeDirection)).Length - 1);
                                    DrawFixedSizeArrayProperty(bulletDataProp.FindPropertyRelative("randomRow"), Enum.GetValues(typeof(ERow)).Length - 1);
                                    DrawFixedSizeArrayProperty(bulletDataProp.FindPropertyRelative("randomColumn"), Enum.GetValues(typeof(EColumn)).Length - 1);
                                    DrawFixedSizeArrayProperty(bulletDataProp.FindPropertyRelative("randomTurnDirection"), Enum.GetValues(typeof(ECartridgeDirection)).Length - 1);
                                    DrawFixedSizeArrayProperty(bulletDataProp.FindPropertyRelative("randomTurnRow"), Enum.GetValues(typeof(ERow)).Length - 1);
                                    DrawFixedSizeArrayProperty(bulletDataProp.FindPropertyRelative("randomTurnColumn"), Enum.GetValues(typeof(EColumn)).Length - 1);
                                    break;
                                }
                        }
                    }
                });
                EditorGUI.indentLevel--;
            }
        });
    }

    /// <summary>
    /// List、Arrayなどの配列型のPropertyを描画するメソッド
    /// </summary>
    /// <param name="property">描画する対象、配列である必要がある</param>
    /// <param name="action">配列に格納する要素に対する描画処理、nullの場合はEditorGUI.PropertyFieldを使う</param>
    private static void DrawArrayProperty(SerializedProperty property, Action<SerializedProperty, int> action = null)
    {
        if (!property.isArray)
            return;

        property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.displayName);
        EditorGUILayout.PropertyField(property.FindPropertyRelative("Array.size"));
        if (property.isExpanded) {
            for (int i = 0 ; i < property.arraySize ; ++i) {
                var arrayElementProperty = property.GetArrayElementAtIndex(i);
                if (action != null) {
                    action.Invoke(arrayElementProperty, i);
                } else {
                    EditorGUILayout.PropertyField(arrayElementProperty, new GUIContent(arrayElementProperty.displayName));
                }
            }
        }
    }

    /// <summary>
    /// 配列の大きさを制限した配列を描画する
    /// <see cref="StageDataEditor.DrawArrayProperty(SerializedProperty, Action{SerializedProperty, int})"/>
    /// </summary>
    /// <param name="property"></param>
    /// <param name="size"></param>
    /// <param name="action"></param>
    private static void DrawFixedSizeArrayProperty(SerializedProperty property, int size, Action<SerializedProperty, int> action = null)
    {
        if (!property.isArray)
            return;

        property.arraySize = size;
        property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.displayName);
        GUI.enabled = false;
        EditorGUILayout.PropertyField(property.FindPropertyRelative("Array.size"));
        GUI.enabled = true;

        if (property.isExpanded) {
            for (int i = 0 ; i < property.arraySize ; ++i) {
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
