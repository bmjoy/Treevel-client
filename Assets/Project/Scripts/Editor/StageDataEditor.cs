using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Project.Scripts.Editor
{
    [CustomEditor(typeof(StageData))]
    [CanEditMultipleObjects]
    public class StageDataEditor : UnityEditor.Editor
    {
        private SerializedProperty _tileDatasProp;
        private SerializedProperty _bottleDatasProp;
        private SerializedProperty _gimmickDatasProp;

        private int _numOfAttackableBottles = 0;

        private StageData _src;
        public void OnEnable()
        {
            _tileDatasProp = serializedObject.FindProperty("tiles");
            _bottleDatasProp = serializedObject.FindProperty("bottles");
            _gimmickDatasProp = serializedObject.FindProperty("gimmicks");

            _src = target as StageData;
            if (_src != null)
                _numOfAttackableBottles = GetAttackableBottles()?.Count() ?? 0;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("treeId"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("stageNumber"));

            DrawTutorialData();

            DrawOverviewGimmicks();

            DrawTileList();

            DrawBottleList();

            DrawGimmickList();

            if (!EditorGUI.EndChangeCheck()) return;

            ClearConsole();

            // Set object dirty, this will make it be saved after saving the project.
            EditorUtility.SetDirty(serializedObject.targetObject);

            serializedObject.ApplyModifiedProperties();
            _numOfAttackableBottles = GetAttackableBottles()?.Count() ?? 0;
            ValidateTiles();
        }

        /// <summary>
        /// タイル番号が被っているかを確認
        /// </summary>
        private void ValidateTiles()
        {
            var tiles = _src.TileDatas;

            // 検証済みのタイル番号
            HashSet<int> validatedTileNumbers = new HashSet<int>();

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
            if (!tutorialProp.isExpanded)
                return;

            var tutorialType = tutorialProp.FindPropertyRelative("type");
            tutorialType.enumValueIndex = (int)(ETutorialType)EditorGUILayout.EnumPopup(new GUIContent("Type"), (ETutorialType)tutorialType.enumValueIndex);
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

        private void DrawTileList()
        {
            this.DrawArrayProperty(_tileDatasProp, (tileDataProp, index) => {
                tileDataProp.isExpanded = EditorGUILayout.Foldout(tileDataProp.isExpanded, $"Tile {index + 1}");

                if (!tileDataProp.isExpanded) return;

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(tileDataProp.FindPropertyRelative("number"));

                var tileTypeProp = tileDataProp.FindPropertyRelative("type");
                tileTypeProp.enumValueIndex = (int)(ETileType)EditorGUILayout.EnumPopup(new GUIContent("Type"), (ETileType)tileTypeProp.enumValueIndex);

                switch ((ETileType)tileTypeProp.enumValueIndex) {
                    case ETileType.Normal:
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
                bottleDataProp.isExpanded = EditorGUILayout.Foldout(bottleDataProp.isExpanded, $"Bottle {index + 1}");

                if (!bottleDataProp.isExpanded) return;

                EditorGUI.indentLevel++;

                var bottleTypeProp = bottleDataProp.FindPropertyRelative("type");
                bottleTypeProp.enumValueIndex = (int)(EBottleType)EditorGUILayout.EnumPopup(new GUIContent("Type"), (EBottleType)bottleTypeProp.enumValueIndex);
                EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("initPos"));

                switch ((EBottleType)bottleTypeProp.enumValueIndex) {
                    case EBottleType.Normal: {
                            EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("targetPos"));
                            EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("bottleSprite"));
                            EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("targetTileSprite"));
                        }
                        break;

                    case EBottleType.Life: {
                            EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("targetPos"));
                            EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("life"));
                            EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("bottleSprite"));
                            EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("targetTileSprite"));
                        }
                        break;
                    case EBottleType.AttackableDummy:
                        EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("life"));
                        EditorGUILayout.PropertyField(bottleDataProp.FindPropertyRelative("bottleSprite"));
                        break;
                    case EBottleType.Dynamic:
                    case EBottleType.Static:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                EditorGUI.indentLevel--;
            });
        }

        private void DrawGimmickList()
        {
            this.DrawArrayProperty(_gimmickDatasProp, (gimmickDataProp, index) => {
                gimmickDataProp.isExpanded = EditorGUILayout.Foldout(gimmickDataProp.isExpanded, $"Gimmick {index + 1}");

                if (!gimmickDataProp.isExpanded) return;

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(gimmickDataProp.FindPropertyRelative("appearTime"));
                EditorGUILayout.PropertyField(gimmickDataProp.FindPropertyRelative("interval"));
                EditorGUILayout.PropertyField(gimmickDataProp.FindPropertyRelative("loop"));

                var gimmickTypeProp = gimmickDataProp.FindPropertyRelative("type");

                gimmickTypeProp.enumValueIndex = (int)(EGimmickType)EditorGUILayout.EnumPopup(
                        label: new GUIContent("Type"),
                        selected: (EGimmickType)gimmickTypeProp.enumValueIndex
                    );

                switch ((EGimmickType)gimmickTypeProp.enumValueIndex) {
                    case EGimmickType.Tornado: {
                            var useRandomProp = gimmickDataProp.FindPropertyRelative("useRandomParameter");
                            var directionsProp = gimmickDataProp.FindPropertyRelative("targetDirections");
                            var linesProp = gimmickDataProp.FindPropertyRelative("targetLines");

                            EditorGUILayout.PropertyField(useRandomProp);

                            // ターゲット数は少なくても1
                            directionsProp.arraySize = Math.Max(directionsProp.arraySize, 1);
                            EditorGUILayout.PropertyField(directionsProp.FindPropertyRelative("Array.size"), new GUIContent("Number Of Target"));

                            var targetNum = linesProp.arraySize = directionsProp.arraySize;
                            var showRandomFiledsFlag = useRandomProp.boolValue;
                            for (var i = 0 ; i < targetNum ; i++) {
                                var directionElem = directionsProp.GetArrayElementAtIndex(i);
                                var lineElem = linesProp.GetArrayElementAtIndex(i);
                                
                                if (useRandomProp.boolValue) {
                                    // 方向と行列をランダムに設定
                                    directionElem.intValue = -1;
                                    lineElem.intValue = -1;
                                } else {
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    EditorGUILayout.LabelField($"Target {i + 1}");
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(directionElem, new GUIContent("Direction"));

                                    if ((directionElem.intValue != (int)ETornadoDirection.Random) && (directionElem.intValue < 1 || 4 < directionElem.intValue))
                                        directionElem.intValue = 1;

                                    switch ((ETornadoDirection)directionElem.intValue) {
                                        case ETornadoDirection.ToBottom:
                                        case ETornadoDirection.ToUp: {
                                                // デフォルト値設定
                                                if (lineElem.intValue < 1 || lineElem.intValue > StageSize.COLUMN)
                                                    lineElem.intValue = 1;

                                                var options = Enum.GetNames(typeof(EColumn)).Where(str => str != "Random").ToArray();
                                                var selectedIdx = EditorGUILayout.Popup(new GUIContent($"Target Column"), lineElem.intValue - 1, options);
                                                lineElem.intValue = (int)Enum.Parse(typeof(EColumn), options[selectedIdx]);
                                                break;
                                            }
                                        case ETornadoDirection.ToRight:
                                        case ETornadoDirection.ToLeft: {
                                                // デフォルト値設定
                                                if (lineElem.intValue < 1 || lineElem.intValue > StageSize.ROW)
                                                    lineElem.intValue = 1;

                                                var options = Enum.GetNames(typeof(ERow)).Where(str => str != "Random").ToArray();
                                                var selectedIdx = EditorGUILayout.Popup(new GUIContent($"Target Row"), lineElem.intValue - 1, options);
                                                lineElem.intValue = (int)Enum.Parse(typeof(ERow), options[selectedIdx]);
                                                break;
                                            }
                                        case ETornadoDirection.Random: {
                                                showRandomFiledsFlag = true;
                                                break;
                                            }
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                    EditorGUI.indentLevel--;
                                    EditorGUILayout.EndVertical();
                                }
                            }
                            if (showRandomFiledsFlag) {
                                {
                                    var randomDirectionProp = gimmickDataProp.FindPropertyRelative("randomDirection");
                                    randomDirectionProp.arraySize = 4;
                                    var subLabels = (new string[] {"L", "R", "U", "D"}).Select(s => new GUIContent(s)).ToArray();
                                    var rect = EditorGUILayout.GetControlRect();
                                    EditorGUI.MultiPropertyField(rect, subLabels, randomDirectionProp.GetArrayElementAtIndex(0), new GUIContent("Random Direction"));
                                }
                                {
                                    var randomRowProp = gimmickDataProp.FindPropertyRelative("randomRow");
                                    randomRowProp.arraySize = StageSize.ROW;
                                    var subLabels = Enumerable.Range(1, StageSize.ROW).Select(n => new GUIContent(n.ToString())).ToArray();
                                    var rect = EditorGUILayout.GetControlRect();
                                    EditorGUI.MultiPropertyField(rect, subLabels, randomRowProp.GetArrayElementAtIndex(0), new GUIContent("Random Row"));
                                }
                                {
                                    var randomColumnProp = gimmickDataProp.FindPropertyRelative("randomColumn");
                                    randomColumnProp.arraySize = StageSize.COLUMN;
                                    var subLabels = Enumerable.Range(1, StageSize.COLUMN).Select(n => new GUIContent(n.ToString())).ToArray();
                                    var rect = EditorGUILayout.GetControlRect();
                                    EditorGUI.MultiPropertyField(rect, subLabels, randomColumnProp.GetArrayElementAtIndex(0), new GUIContent("Random Column"));
                                }
                            }
                            break;
                        }
                    case EGimmickType.Meteorite: {
                            var useRandomProp = gimmickDataProp.FindPropertyRelative("useRandomParameter");
                            var rowProp = gimmickDataProp.FindPropertyRelative("targetRow");
                            var colProp = gimmickDataProp.FindPropertyRelative("targetColumn");

                            EditorGUILayout.PropertyField(useRandomProp);

                            if (useRandomProp.boolValue) {
                                // 行、列をランダムに
                                rowProp.intValue = colProp.intValue = -1;

                                {
                                    var randomRowProp = gimmickDataProp.FindPropertyRelative("randomRow");
                                    randomRowProp.arraySize = StageSize.ROW;
                                    var subLabels = Enumerable.Range(1, StageSize.ROW).Select(n => new GUIContent(n.ToString())).ToArray();
                                    var rect = EditorGUILayout.GetControlRect();
                                    EditorGUI.MultiPropertyField(rect, subLabels, randomRowProp.GetArrayElementAtIndex(0), new GUIContent("Random Row"));
                                }
                                {
                                    var randomColumnProp = gimmickDataProp.FindPropertyRelative("randomColumn");
                                    randomColumnProp.arraySize = StageSize.COLUMN;
                                    var subLabels = Enumerable.Range(1, StageSize.COLUMN).Select(n => new GUIContent(n.ToString())).ToArray();
                                    var rect = EditorGUILayout.GetControlRect();
                                    EditorGUI.MultiPropertyField(rect, subLabels, randomColumnProp.GetArrayElementAtIndex(0), new GUIContent("Random Column"));
                                }
                            } else {
                                if (rowProp.intValue < 1 || rowProp.intValue > StageSize.ROW)
                                    rowProp.intValue = 1;

                                rowProp.intValue = (int)(ERow)EditorGUILayout.EnumPopup(
                                        label: new GUIContent("Row"),
                                        selected: (ERow)rowProp.intValue,
                                        //ランダムは選択不能にする
                                        checkEnabled: (eType) => (ERow)eType != ERow.Random,
                                        includeObsolete: false
                                    );

                                if (colProp.intValue < 1 || colProp.intValue > StageSize.COLUMN)
                                    colProp.intValue = 1;
                                colProp.intValue = (int)(EColumn)EditorGUILayout.EnumPopup(
                                        label: new GUIContent("ColuEColumn"),
                                        selected: (EColumn)colProp.intValue,
                                        //ランダムは選択不能にする
                                        checkEnabled: (eType) => (EColumn)eType != EColumn.Random,
                                        includeObsolete: false
                                    );
                            }
                            break;
                        }
                    case EGimmickType.AimingMeteorite: {
                            var useRandomProp = gimmickDataProp.FindPropertyRelative("useRandomParameter");
                            var targetBottleProp = gimmickDataProp.FindPropertyRelative("targetBottle");

                            EditorGUILayout.PropertyField(useRandomProp);

                            if (useRandomProp.boolValue) {
                                this.DrawFixedSizeArrayProperty(gimmickDataProp.FindPropertyRelative("randomAttackableBottles"), _numOfAttackableBottles, RenderRandomAttackableBottlesElement);
                            } else {
                                // 現在あるボトルのIDから選択するように
                                var bottleIds = GetAttackableBottles().Select(bottle => bottle.initPos).ToList();

                                var selectedIdx = bottleIds.Contains(targetBottleProp.intValue) ?
                                bottleIds.Select((id, idx) => new {
                                    id, idx
                                }).First(t => t.id == targetBottleProp.intValue).idx :
                                0;

                                selectedIdx = EditorGUILayout.Popup(
                                        new GUIContent("Target Bottle"),
                                        selectedIdx,
                                        bottleIds.Select(id => id.ToString()).ToArray()
                                    );

                                targetBottleProp.intValue = bottleIds[selectedIdx];
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
            return _src.BottleDatas?.Where(x => x.type == EBottleType.Normal || x.type == EBottleType.Life);
        }

        private static void ClearConsole()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
    }
}
