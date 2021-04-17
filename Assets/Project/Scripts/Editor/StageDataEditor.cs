using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using Treevel.Modules.GamePlayScene.Gimmick;
using UnityEditor;
using UnityEngine;

namespace Treevel.Editor
{
    [CustomEditor(typeof(StageData)), CanEditMultipleObjects]
    public class StageDataEditor : UnityEditor.Editor
    {
        private SerializedProperty _tileDatasProp;
        private SerializedProperty _bottleDatasProp;
        private SerializedProperty _gimmickDatasProp;

        /// <summary>
        /// 攻撃可能なボトルの数
        /// </summary>
        private int _numOfAttackableBottles = 0;

        /// <summary>
        /// ギミックの数
        /// </summary>
        private int _numOfGimmicks = 0;

        private StageData _src;

        public void OnEnable()
        {
            _tileDatasProp = serializedObject.FindProperty("tiles");
            _bottleDatasProp = serializedObject.FindProperty("bottles");
            _gimmickDatasProp = serializedObject.FindProperty("gimmicks");

            _src = target as StageData;
            if (_src != null) {
                _numOfAttackableBottles = GetAttackableBottles()?.Count() ?? 0;
                _numOfGimmicks = _src.GimmickDatas?.Count ?? 0;
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("treeId"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("stageNumber"));

            DrawTutorialData();

            DrawConstraintStages();

            DrawOverviewGimmicks();

            DrawTileList();

            DrawBottleList();

            DrawGimmickList();

            CheckGoalColorConsistency();

            if (!EditorGUI.EndChangeCheck()) return;

            ClearConsole();

            // Set object dirty, this will make it be saved after saving the project.
            EditorUtility.SetDirty(serializedObject.targetObject);

            serializedObject.ApplyModifiedProperties();
            _numOfAttackableBottles = GetAttackableBottles()?.Count() ?? 0;
            // ギミックの個数が増えたとき、増えたギミックはデフォルト値(Tornado)に設定する
            if ((_src.GimmickDatas?.Count ?? 0) > _numOfGimmicks) {
                Enumerable.Range(_numOfGimmicks, _gimmickDatasProp.arraySize - _numOfGimmicks)
                    .ToList()
                    .ForEach(i => _gimmickDatasProp.GetArrayElementAtIndex(i).FindPropertyRelative("type")
                                 .enumValueIndex = (int)EGimmickType.Tornado);
            }

            _numOfGimmicks = _src.GimmickDatas?.Count ?? 0;
            ValidateTiles();
        }

        /// <summary>
        /// ステージの解放条件を描画する
        /// </summary>
        private void DrawConstraintStages()
        {
            // get serialized property
            var constraintsProp = serializedObject.FindProperty("constraintStages");

            this.DrawArrayProperty(constraintsProp, (property, idx) => {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel($"Stage{idx + 1}");

                var treeId = ETreeId.Spring_1;
                var stageNum = 1;
                if (property.stringValue != "") {
                    (treeId, stageNum) = StageData.DecodeStageIdKey(property.stringValue);
                }

                var rect = EditorGUILayout.GetControlRect();
                rect.width = 125.0f;
                var selectedTree = (ETreeId)EditorGUI.EnumPopup(position: rect, selected: treeId);
                rect.x += rect.width;
                var selectedStageNum = EditorGUI.IntField(rect, stageNum);

                property.stringValue = StageData.EncodeStageIdKey(selectedTree, selectedStageNum);
                GUILayout.EndHorizontal();
            });
        }

        /// <summary>
        /// タイル番号が被っているかを確認
        /// </summary>
        private void ValidateTiles()
        {
            var tiles = _src.TileDatas;

            // 検証済みのタイル番号
            var validatedTileNumbers = new HashSet<int>();

            tiles.ForEach(tile => {
                if (tile.number != 0 && validatedTileNumbers.Contains(tile.number)) {
                    Debug.LogWarning($"Tile {tile.number} has already been set");
                } else {
                    validatedTileNumbers.Add(tile.number);
                }

                if (tile.pairNumber != 0 && validatedTileNumbers.Contains(tile.pairNumber)) {
                    Debug.LogWarning($"Tile {tile.pairNumber} has already been set");
                } else {
                    validatedTileNumbers.Add(tile.pairNumber);
                }
            });
        }

        private void DrawTutorialData()
        {
            // get serialized property
            var tutorialProp = serializedObject.FindProperty("tutorial");

            // check is expanded
            tutorialProp.isExpanded = EditorGUILayout.Foldout(tutorialProp.isExpanded, new GUIContent("Tutorial Data"));
            if (!tutorialProp.isExpanded) return;

            var tutorialType = tutorialProp.FindPropertyRelative("type");
            tutorialType.enumValueIndex =
                (int)(ETutorialType)EditorGUILayout.EnumPopup(new GUIContent("Type"),
                                                              (ETutorialType)tutorialType.enumValueIndex);
            switch ((ETutorialType)tutorialType.enumValueIndex) {
                case ETutorialType.Image:
                    EditorGUILayout.PropertyField(tutorialProp.FindPropertyRelative("image"));
                    break;
                case ETutorialType.Video:
                    EditorGUILayout.PropertyField(tutorialProp.FindPropertyRelative("video"));
                    break;
                case ETutorialType.None:
                default:
                    break;
            }
        }

        private void DrawOverviewGimmicks()
        {
            this.DrawArrayProperty(serializedObject.FindProperty("overviewGimmicks"));
        }

        /// <summary>
        /// ギミック、ボトル、タイルのヘッダーを削除ボタン込みで描画する
        /// </summary>
        /// <param name="arrayProp">対象のリストのプロパティ</param>
        /// <param name="elementProp">描画対象のプロパティ</param>
        /// <param name="index">描画対象のインデックス</param>
        /// <returns>削除されたかどうかを返す</returns>
        private static bool DrawObjectHeader(SerializedProperty arrayProp, SerializedProperty elementProp, int index)
        {
            GUILayout.BeginHorizontal();
            elementProp.isExpanded = EditorGUILayout.Foldout(elementProp.isExpanded, $"{arrayProp.displayName} {index + 1}", toggleOnLabelClick: true);

            if (GUILayout.Button("Delete", GUILayout.Width(80)) &&
                !EditorUtility.DisplayDialog("削除確認", "まじで消すの？？復元できないぞ！？", "やめとこう", "まじで消す")) {
                arrayProp.DeleteArrayElementAtIndex(index);
                return true;
            }

            GUILayout.EndHorizontal();
            return false;
        }

        private void DrawTileList()
        {
            this.DrawArrayProperty(_tileDatasProp, (tileDataProp, index) => {
                if (DrawObjectHeader(_tileDatasProp, tileDataProp, index)) return;
                if (!tileDataProp.isExpanded) return;

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(tileDataProp.FindPropertyRelative("number"));

                var tileTypeProp = tileDataProp.FindPropertyRelative("type");

                var newEnumValueIndex =
                    (int)(ETileType)EditorGUILayout.EnumPopup(
                        new GUIContent("Type"),
                        (ETileType)tileTypeProp.enumValueIndex,
                        // Normalは選択不能にする
                        (eType) => (ETileType)eType != ETileType.Normal,
                        false
                    );

                // タイプが変わっていたらデータをリセット
                if (newEnumValueIndex != tileTypeProp.enumValueIndex) {
                    tileTypeProp.enumValueIndex = newEnumValueIndex;
                    ResetData(tileDataProp);
                }

                switch ((ETileType)tileTypeProp.enumValueIndex) {
                    case ETileType.Normal:
                        break;
                    case ETileType.Goal: {
                        var goalColorElem = tileDataProp.FindPropertyRelative("goalColor");
                        goalColorElem.intValue = (int)(EGoalColor)EditorGUILayout.EnumPopup(
                            label: new GUIContent("GoalColor"),
                            selected: (EGoalColor)goalColorElem.intValue,
                            // Noneは選択不能にする
                            checkEnabled: (eType) => (EGoalColor)eType != EGoalColor.None,
                            includeObsolete: false
                        );
                    }
                        break;
                    case ETileType.Warp: {
                        EditorGUILayout.PropertyField(tileDataProp.FindPropertyRelative("pairNumber"));
                    }
                        break;
                    case ETileType.Holy:
                        break;
                    case ETileType.Spiderweb:
                        break;
                    case ETileType.Ice:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                EditorGUI.indentLevel--;
            });
        }

        private void DrawBottleList()
        {
            this.DrawArrayProperty(_bottleDatasProp, (bottleDataProp, index) => {
                if (DrawObjectHeader(_bottleDatasProp, bottleDataProp, index)) return;
                if (!bottleDataProp.isExpanded) return;

                EditorGUI.indentLevel++;

                var bottleTypeProp = bottleDataProp.FindPropertyRelative("type");

                // ErasableBottle は ErasableGimmick のみから生成できるようにする
                var newEnumValueIndex = (int)(EBottleType)EditorGUILayout.EnumPopup(
                    label: new GUIContent("Type"),
                    selected: (EBottleType)bottleTypeProp.enumValueIndex,
                    checkEnabled: eType => (EBottleType)eType != EBottleType.Erasable,
                    includeObsolete: false);

                // タイプが変わっていたらデータをリセット
                if (newEnumValueIndex != bottleTypeProp.enumValueIndex) {
                    bottleTypeProp.enumValueIndex = newEnumValueIndex;
                    ResetData(bottleDataProp);
                }

                EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("initPos"));

                switch ((EBottleType)bottleTypeProp.enumValueIndex) {
                    case EBottleType.Normal: {
                        var lifeProp = bottleDataProp.FindPropertyRelative("life");
                        lifeProp.intValue = Mathf.Clamp(lifeProp.intValue, 1, LifeAttributeController.MAX_LIFE);
                        EditorGUILayout.PropertyField(lifeProp);
                        var goalColorElem = bottleDataProp.FindPropertyRelative("goalColor");
                        goalColorElem.intValue = (int)(EGoalColor)EditorGUILayout.EnumPopup(
                            label: new GUIContent("GoalColor"),
                            selected: (EGoalColor)goalColorElem.intValue,
                            // GoalTileが指定しているGoalColorのみ選択可能にする
                            checkEnabled: (eType) => GetTileGoalColors().Contains((EGoalColor)eType),
                            includeObsolete: false
                        );
                        EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("isSelfish"));
                        EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("isDark"));
                        EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("isReverse"));
                    }
                        break;
                    case EBottleType.Dynamic:
                        EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("isSelfish"));
                        EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("isReverse"));
                        break;
                    case EBottleType.AttackableDummy: {
                        EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("isSelfish"));
                        EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("isReverse"));
                    }
                        break;
                    case EBottleType.Static:
                        break;
                    case EBottleType.Erasable:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                EditorGUI.indentLevel--;
            });
        }

        private void DrawGimmickList()
        {
            var gimmickTypeData = Enumerable.Range(0, _gimmickDatasProp.arraySize)
                .Select(index => _gimmickDatasProp.GetArrayElementAtIndex(index).FindPropertyRelative("type")).ToList();

            // Powder ギミックが存在するかどうか
            var isExistPowder = gimmickTypeData
                .Any(data => (EGimmickType)data.enumValueIndex == EGimmickType.Powder);

            // Erasable ギミックが存在するか
            var isExistErasable = gimmickTypeData
                .Any(data => (EGimmickType)data.enumValueIndex == EGimmickType.Erasable);

            this.DrawArrayProperty(_gimmickDatasProp, (gimmickDataProp, index) => {
                if (DrawObjectHeader(_gimmickDatasProp, gimmickDataProp, index)) return;
                if (!gimmickDataProp.isExpanded) return;

                EditorGUI.indentLevel++;

                var gimmickTypeProp = gimmickDataProp.FindPropertyRelative("type");
                var newEnumValueIndex = (int)(EGimmickType)EditorGUILayout.EnumPopup(
                    label: new GUIContent("Type"),
                    selected: (EGimmickType)gimmickTypeProp.enumValueIndex,
                    // 1 ステージに 1 つまでのギミックは制約を設ける
                    checkEnabled: (eType) => !(isExistPowder && (EGimmickType)eType == EGimmickType.Powder ||
                                               isExistErasable && (EGimmickType)eType == EGimmickType.Erasable),
                    includeObsolete: false
                );
                // タイプが変わっていたらデータをリセット
                if (newEnumValueIndex != gimmickTypeProp.enumValueIndex) {
                    gimmickTypeProp.enumValueIndex = newEnumValueIndex;
                    ResetData(gimmickDataProp);
                }

                if ((EGimmickType)gimmickTypeProp.enumValueIndex == EGimmickType.Powder ||
                    (EGimmickType)gimmickTypeProp.enumValueIndex == EGimmickType.Erasable) {
                    EditorGUI.indentLevel--;
                    return;
                }

                EditorGUILayout.PropertyField(gimmickDataProp.FindPropertyRelative("appearTime"));
                var intervalProp = gimmickDataProp.FindPropertyRelative("interval");
                EditorGUILayout.PropertyField(intervalProp);
                var loopProp = gimmickDataProp.FindPropertyRelative("loop");
                EditorGUILayout.PropertyField(loopProp);

                switch ((EGimmickType)gimmickTypeProp.enumValueIndex) {
                    case EGimmickType.Tornado: {
                        var useRandomProp = gimmickDataProp.FindPropertyRelative("isRandom");
                        var directionsProp = gimmickDataProp.FindPropertyRelative("targetDirections");
                        var linesProp = gimmickDataProp.FindPropertyRelative("targetLines");

                        EditorGUILayout.PropertyField(useRandomProp);

                        // ターゲット数は少なくても1
                        directionsProp.arraySize = Math.Max(directionsProp.arraySize, 1);
                        EditorGUILayout.PropertyField(directionsProp.FindPropertyRelative("Array.size"),
                                                      new GUIContent("Number Of Target"));

                        var targetNum = linesProp.arraySize = directionsProp.arraySize;
                        var showRandomFieldsFlag = useRandomProp.boolValue;
                        for (var i = 0; i < targetNum; i++) {
                            var directionElem = directionsProp.GetArrayElementAtIndex(i);
                            var lineElem = linesProp.GetArrayElementAtIndex(i);

                            if (useRandomProp.boolValue) {
                                // 行列をランダムに設定
                                lineElem.intValue = -1;
                            } else {
                                EditorGUILayout.BeginVertical(GUI.skin.box);
                                EditorGUILayout.LabelField($"Target {i + 1}");
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(directionElem, new GUIContent("Direction"));

                                switch ((EDirection)directionElem.intValue) {
                                    case EDirection.ToDown:
                                    case EDirection.ToUp: {
                                        // デフォルト値設定
                                        lineElem.intValue =
                                            Mathf.Clamp(lineElem.intValue, 0, Constants.StageSize.COLUMN - 1);
                                        lineElem.intValue = (int)(EColumn)EditorGUILayout.EnumPopup(
                                            label: new GUIContent("Target Column"),
                                            selected: (EColumn)lineElem.intValue,
                                            //ランダムは選択不能にする
                                            checkEnabled: (eType) => (EColumn)eType != EColumn.Random,
                                            includeObsolete: false
                                        );
                                        break;
                                    }
                                    case EDirection.ToRight:
                                    case EDirection.ToLeft: {
                                        // デフォルト値設定
                                        lineElem.intValue =
                                            Mathf.Clamp(lineElem.intValue, 0, Constants.StageSize.ROW - 1);
                                        lineElem.intValue = (int)(ERow)EditorGUILayout.EnumPopup(
                                            label: new GUIContent("Target Row"),
                                            selected: (ERow)lineElem.intValue,
                                            //ランダムは選択不能にする
                                            checkEnabled: (eType) => (ERow)eType != ERow.Random,
                                            includeObsolete: false
                                        );
                                        break;
                                    }
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                                EditorGUI.indentLevel--;
                                EditorGUILayout.EndVertical();
                            }
                        }

                        if (showRandomFieldsFlag) {
                            {
                                var randomDirectionProp = gimmickDataProp.FindPropertyRelative("randomDirection");
                                randomDirectionProp.arraySize = 4;
                                var subLabels = new[] { "L", "R", "U", "D" }.Select(s => new GUIContent(s))
                                    .ToArray();
                                var rect = EditorGUILayout.GetControlRect();
                                EditorGUI.MultiPropertyField(rect, subLabels,
                                                             randomDirectionProp.GetArrayElementAtIndex(0),
                                                             new GUIContent("Random Direction"));
                            }
                            {
                                var randomRowProp = gimmickDataProp.FindPropertyRelative("randomRow");
                                randomRowProp.arraySize = Constants.StageSize.ROW;
                                var subLabels = Enumerable.Range(1, Constants.StageSize.ROW)
                                    .Select(n => new GUIContent(n.ToString())).ToArray();
                                var rect = EditorGUILayout.GetControlRect();
                                EditorGUI.MultiPropertyField(rect, subLabels, randomRowProp.GetArrayElementAtIndex(0),
                                                             new GUIContent("Random Row"));
                            }
                            {
                                var randomColumnProp = gimmickDataProp.FindPropertyRelative("randomColumn");
                                randomColumnProp.arraySize = Constants.StageSize.COLUMN;
                                var subLabels = Enumerable.Range(1, Constants.StageSize.COLUMN)
                                    .Select(n => new GUIContent(n.ToString())).ToArray();
                                var rect = EditorGUILayout.GetControlRect();
                                EditorGUI.MultiPropertyField(rect, subLabels,
                                                             randomColumnProp.GetArrayElementAtIndex(0),
                                                             new GUIContent("Random Column"));
                            }
                        }

                        break;
                    }
                    case EGimmickType.Meteorite: {
                        var useRandomProp = gimmickDataProp.FindPropertyRelative("isRandom");
                        var rowProp = gimmickDataProp.FindPropertyRelative("targetRow");
                        var colProp = gimmickDataProp.FindPropertyRelative("targetColumn");

                        EditorGUILayout.PropertyField(useRandomProp);

                        if (useRandomProp.boolValue) {
                            // 行、列をランダムに
                            rowProp.intValue = colProp.intValue = -1;

                            {
                                var randomRowProp = gimmickDataProp.FindPropertyRelative("randomRow");
                                randomRowProp.arraySize = Constants.StageSize.ROW;
                                var subLabels = Enumerable.Range(1, Constants.StageSize.ROW)
                                    .Select(n => new GUIContent(n.ToString())).ToArray();
                                var rect = EditorGUILayout.GetControlRect();
                                EditorGUI.MultiPropertyField(rect, subLabels, randomRowProp.GetArrayElementAtIndex(0),
                                                             new GUIContent("Random Row"));
                            }
                            {
                                var randomColumnProp = gimmickDataProp.FindPropertyRelative("randomColumn");
                                randomColumnProp.arraySize = Constants.StageSize.COLUMN;
                                var subLabels = Enumerable.Range(1, Constants.StageSize.COLUMN)
                                    .Select(n => new GUIContent(n.ToString())).ToArray();
                                var rect = EditorGUILayout.GetControlRect();
                                EditorGUI.MultiPropertyField(rect, subLabels,
                                                             randomColumnProp.GetArrayElementAtIndex(0),
                                                             new GUIContent("Random Column"));
                            }
                        } else {
                            rowProp.intValue = Mathf.Clamp(rowProp.intValue, 0, Constants.StageSize.ROW - 1);
                            rowProp.intValue = (int)(ERow)EditorGUILayout.EnumPopup(
                                label: new GUIContent("Row"),
                                selected: (ERow)rowProp.intValue,
                                //ランダムは選択不能にする
                                checkEnabled: (eType) => (ERow)eType != ERow.Random,
                                includeObsolete: false
                            );

                            colProp.intValue = Mathf.Clamp(colProp.intValue, 0, Constants.StageSize.COLUMN - 1);
                            colProp.intValue = (int)(EColumn)EditorGUILayout.EnumPopup(
                                label: new GUIContent("Column"),
                                selected: (EColumn)colProp.intValue,
                                //ランダムは選択不能にする
                                checkEnabled: (eType) => (EColumn)eType != EColumn.Random,
                                includeObsolete: false
                            );
                        }

                        break;
                    }
                    case EGimmickType.AimingMeteorite: {
                        var useRandomProp = gimmickDataProp.FindPropertyRelative("isRandom");
                        var targetBottleProp = gimmickDataProp.FindPropertyRelative("targetBottle");

                        EditorGUILayout.PropertyField(useRandomProp);

                        if (useRandomProp.boolValue) {
                            this.DrawFixedSizeArrayProperty(
                                gimmickDataProp.FindPropertyRelative("randomAttackableBottles"),
                                _numOfAttackableBottles, RenderRandomAttackableBottlesElement);
                        } else {
                            // 現在あるボトルのIDから選択するように
                            var bottleIds = GetAttackableBottles().Select(bottle => bottle.initPos).ToList();

                            var selectedIdx = bottleIds.Contains((short)targetBottleProp.intValue)
                                ? bottleIds.Select((id, idx) => new {
                                    id, idx,
                                }).First(t => t.id == targetBottleProp.intValue).idx
                                : 0;

                            selectedIdx = EditorGUILayout.Popup(
                                new GUIContent("Target Bottle"),
                                selectedIdx,
                                bottleIds.Select(id => id.ToString()).ToArray()
                            );

                            targetBottleProp.intValue = bottleIds[selectedIdx];
                        }

                        break;
                    }
                    case EGimmickType.Thunder: {
                        var targetsProp = gimmickDataProp.FindPropertyRelative("targets");
                        this.DrawArrayProperty(targetsProp, (vecProp, idx) => {
                            var xProp = vecProp.FindPropertyRelative("x");
                            var yProp = vecProp.FindPropertyRelative("y");

                            // Draw Prefix Label
                            var rect = EditorGUILayout.GetControlRect();
                            rect = EditorGUI.PrefixLabel(rect, new GUIContent($"Target {idx + 1}"));

                            // Render input fields for row and column
                            var subLabels = new GUIContent[] {
                                new GUIContent("Row"),
                                new GUIContent("Column"),
                            };
                            var buffer = new[] { xProp.intValue, yProp.intValue };
                            EditorGUI.MultiIntField(rect, subLabels, buffer);

                            xProp.intValue = Mathf.Clamp(buffer[0], 0, Constants.StageSize.ROW - 1);
                            yProp.intValue = Mathf.Clamp(buffer[1], 0, Constants.StageSize.COLUMN - 1);
                        });
                        break;
                    }
                    case EGimmickType.SolarBeam: {
                        // 攻撃回数
                        var attackTimesProp = gimmickDataProp.FindPropertyRelative("attackTimes");
                        // 1以上の値を入れる
                        attackTimesProp.intValue = Math.Max(1, attackTimesProp.intValue);
                        EditorGUILayout.PropertyField(attackTimesProp);

                        // 攻撃方向
                        var directionProp = gimmickDataProp.FindPropertyRelative("targetDirection");
                        EditorGUILayout.PropertyField(directionProp);
                        switch ((EDirection)directionProp.intValue) {
                            case EDirection.ToRight:
                            case EDirection.ToLeft:
                                EditorGUILayout.PropertyField(gimmickDataProp.FindPropertyRelative("targetRow"));
                                break;
                            case EDirection.ToUp:
                            case EDirection.ToDown:
                                EditorGUILayout.PropertyField(gimmickDataProp.FindPropertyRelative("targetColumn"));
                                break;
                            default:
                                // 描画を止めないように適当な値を設定する
                                Debug.LogWarning($"Invalid Enum Value: {directionProp.intValue}");
                                directionProp.intValue = (int)EDirection.ToLeft;
                                break;
                        }

                        break;
                    }
                    case EGimmickType.GustWind: {
                        // 攻撃方向
                        var directionProp = gimmickDataProp.FindPropertyRelative("targetDirection");
                        EditorGUILayout.PropertyField(directionProp);
                        switch ((EDirection)directionProp.intValue) {
                            case EDirection.ToRight:
                            case EDirection.ToLeft:
                                EditorGUILayout.PropertyField(gimmickDataProp.FindPropertyRelative("targetRow"));
                                break;
                            case EDirection.ToUp:
                            case EDirection.ToDown:
                                EditorGUILayout.PropertyField(gimmickDataProp.FindPropertyRelative("targetColumn"));
                                break;
                            default:
                                // 描画を止めないように適当な値を設定する
                                Debug.LogWarning($"Invalid Enum Value: {directionProp.intValue}");
                                directionProp.intValue = (int)EDirection.ToLeft;
                                break;
                        }

                        break;
                    }
                    case EGimmickType.Fog: {
                        var durationProp = gimmickDataProp.FindPropertyRelative("duration");
                        // 持続時間は登場間隔より短くする
                        if (loopProp.boolValue && durationProp.floatValue > intervalProp.floatValue) durationProp.floatValue = intervalProp.floatValue;
                        EditorGUILayout.PropertyField(durationProp);
                        var useRandomProp = gimmickDataProp.FindPropertyRelative("isRandom");
                        var rowProp = gimmickDataProp.FindPropertyRelative("targetRow");
                        var colProp = gimmickDataProp.FindPropertyRelative("targetColumn");

                        EditorGUILayout.PropertyField(useRandomProp);

                        if (useRandomProp.boolValue) {
                            // 行、列をランダムに
                            rowProp.intValue = colProp.intValue = -1;

                            var widthProp = gimmickDataProp.FindPropertyRelative("width");
                            widthProp.intValue = Mathf.Clamp(widthProp.intValue, 1, FogController.WIDTH_MAX);

                            EditorGUILayout.PropertyField(widthProp);
                            var heightProp = gimmickDataProp.FindPropertyRelative("height");
                            heightProp.intValue = Mathf.Clamp(heightProp.intValue, 1, FogController.HEIGHT_MAX);

                            EditorGUILayout.PropertyField(heightProp);
                            {
                                var randomRowProp = gimmickDataProp.FindPropertyRelative("randomRow");
                                randomRowProp.arraySize = Constants.StageSize.ROW + 1 - heightProp.intValue;
                                var subLabels = Enumerable.Range(1, randomRowProp.arraySize)
                                    .Select(n => new GUIContent(n.ToString())).ToArray();
                                var rect = EditorGUILayout.GetControlRect();
                                EditorGUI.MultiPropertyField(rect, subLabels, randomRowProp.GetArrayElementAtIndex(0),
                                                             new GUIContent("Random Row"));
                            }
                            {
                                var randomColumnProp = gimmickDataProp.FindPropertyRelative("randomColumn");
                                randomColumnProp.arraySize = Constants.StageSize.COLUMN + 1 - widthProp.intValue;
                                var subLabels = Enumerable.Range(1, randomColumnProp.arraySize)
                                    .Select(n => new GUIContent(n.ToString())).ToArray();
                                var rect = EditorGUILayout.GetControlRect();
                                EditorGUI.MultiPropertyField(rect, subLabels,
                                                             randomColumnProp.GetArrayElementAtIndex(0),
                                                             new GUIContent("Random Column"));
                            }
                        } else {
                            rowProp.intValue = Mathf.Clamp(rowProp.intValue, 0, Constants.StageSize.ROW - 1);
                            rowProp.intValue = (int)(ERow)EditorGUILayout.EnumPopup(
                                label: new GUIContent("Row"),
                                selected: (ERow)rowProp.intValue,
                                //ランダムは選択不能にする
                                checkEnabled: (eType) => (ERow)eType != ERow.Random,
                                includeObsolete: false
                            );

                            colProp.intValue = Mathf.Clamp(colProp.intValue, 0, Constants.StageSize.COLUMN - 1);
                            colProp.intValue = (int)(EColumn)EditorGUILayout.EnumPopup(
                                label: new GUIContent("Column"),
                                selected: (EColumn)colProp.intValue,
                                //ランダムは選択不能にする
                                checkEnabled: (eType) => (EColumn)eType != EColumn.Random,
                                includeObsolete: false
                            );

                            // 横幅、縦幅の設定
                            var widthProp = gimmickDataProp.FindPropertyRelative("width");
                            if (widthProp.intValue < 1 || widthProp.intValue >
                                Mathf.Min(FogController.WIDTH_MAX, Constants.StageSize.COLUMN - colProp.intValue))
                                widthProp.intValue = 1;
                            EditorGUILayout.PropertyField(widthProp);
                            var heightProp = gimmickDataProp.FindPropertyRelative("height");
                            if (heightProp.intValue < 1 || heightProp.intValue >
                                Mathf.Min(FogController.HEIGHT_MAX, Constants.StageSize.ROW - rowProp.intValue))
                                heightProp.intValue = 1;
                            EditorGUILayout.PropertyField(heightProp);
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                EditorGUI.indentLevel--;
            });
        }

        private void RenderRandomAttackableBottlesElement(SerializedProperty elementProperty, int index)
        {
            var bottles = GetAttackableBottles().OrderBy(x => x.initPos);
            var bottleId = bottles.ElementAt(index).initPos;
            EditorGUILayout.PropertyField(elementProperty, new GUIContent($"Bottle ID:{bottleId}"));
        }

        private IEnumerable<BottleData> GetAttackableBottles()
        {
            return _src.BottleDatas?.Where(x => x.type.IsAttackable());
        }

        private static void ClearConsole()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }

        /// <summary>
        /// GoalTileが指定しているGoalColorのみを取得する
        /// </summary>
        /// <returns></returns>
        private IEnumerable<EGoalColor> GetTileGoalColors()
        {
            return _src.TileDatas?.Where(tile => tile.type == ETileType.Goal).Select(tile => tile.goalColor);
        }

        /// <summary>
        /// TileとBottleが指定しているGoalColorの整合性を検証する
        /// </summary>
        private void CheckGoalColorConsistency()
        {
            var goalColorNum = Enum.GetValues(typeof(EGoalColor))
                .OfType<EGoalColor>()
                .ToDictionary(type => type, _ => 0);

            // タイルがしているGoalColorを数える
            _src.TileDatas?.Where(tile => tile.type == ETileType.Goal)
                .ToList().ForEach(tile => {
                    goalColorNum[tile.goalColor] += 1;
                });

            // ボトルが指定しているGoalColorを数える
            _src.BottleDatas?.Where(bottle => bottle.type == EBottleType.Normal)
                .ToList().ForEach(bottle => {
                    goalColorNum[bottle.goalColor] -= 1;
                });

            // それぞれのGoalColorの個数を検証する
            goalColorNum.ToList().ForEach(pair => {
                if (pair.Value > 0) {
                    Debug.LogWarning($"{pair.Value} <b>{pair.Key}</b> bottle are missing.");
                }

                if (pair.Value < 0) {
                    Debug.LogWarning($"There are {(-1) * pair.Value} extra <b>{pair.Key}</b> bottles.");
                }
            });
        }

        /// <summary>
        /// SerializedPropertyをデフォルト値に戻す。
        /// BottleData、TileData、GimmickDataにだけ動作を保証する
        /// </summary>
        /// <param name="prop"> 対象のSerializedProperty </param>
        private static void ResetData(SerializedProperty prop)
        {
            foreach (var child in prop.GetChildren()) {
                // タイプをリセットしたら意味ない
                if (child.name == "type") continue;

                if (child.isArray) {
                    child.ClearArray();
                    child.arraySize = 0;
                    continue;
                }

                // 現状，リセットする必要がある型だけをリセットしている
                switch (child.propertyType) {
                    case SerializedPropertyType.Boolean:
                        child.boolValue = default;
                        break;
                    case SerializedPropertyType.Integer:
                        child.intValue = default;
                        break;
                    case SerializedPropertyType.Float:
                        child.floatValue = default;
                        break;
                    case SerializedPropertyType.Enum:
                        child.enumValueIndex = child.intValue = default;
                        break;
                    case SerializedPropertyType.Generic:
                        var childCopy = child.Copy();
                        var end = child.GetEndProperty(true);
                        if (childCopy.Next(true)) {
                            while (!SerializedProperty.EqualContents(childCopy, end)) {
                                ResetData(childCopy);
                                if (!childCopy.Next(false)) break;
                            }
                        }

                        break;
                }
            }
        }
    }
}
