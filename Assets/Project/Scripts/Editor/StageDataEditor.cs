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
        private SerializedProperty _bulletGroupDatasProp;
        private SerializedProperty _gimmickDatasProp;

        private int _numOfAttackableBottles = 0;

        private StageData _src;
        public void OnEnable()
        {
            _tileDatasProp = serializedObject.FindProperty("tiles");
            _bottleDatasProp = serializedObject.FindProperty("bottles");
            _bulletGroupDatasProp = serializedObject.FindProperty("bulletGroups");
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

            DrawBulletGroupList();

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

                var bulletTypeProp = gimmickDataProp.FindPropertyRelative("type");

                bulletTypeProp.enumValueIndex = (int)(EGimmickType)EditorGUILayout.EnumPopup(
                        label: new GUIContent("Type"),
                        selected: (EGimmickType)bulletTypeProp.enumValueIndex
                    );

                switch ((EGimmickType)bulletTypeProp.enumValueIndex) {
                    case EGimmickType.Tornado: {
                            var directionsProp = gimmickDataProp.FindPropertyRelative("targetDirections");
                            var linesProp = gimmickDataProp.FindPropertyRelative("targetLines");

                            // ターゲット数は少なくても1
                            directionsProp.arraySize = Math.Max(directionsProp.arraySize, 1);
                            EditorGUILayout.PropertyField(directionsProp.FindPropertyRelative("Array.size"), new GUIContent("Number Of Target"));

                            var targetNum = linesProp.arraySize = directionsProp.arraySize;
                            var showRandomFiledsFlag = false;
                            for (var i = 0 ; i < targetNum ; i++) {
                                var directionElem = directionsProp.GetArrayElementAtIndex(i);
                                var lineElem = linesProp.GetArrayElementAtIndex(i);

                                EditorGUILayout.BeginVertical(GUI.skin.box);
                                EditorGUILayout.LabelField($"Target {i + 1}");
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(directionElem, new GUIContent("Direction"));

                                if ((directionElem.intValue != -1 && directionElem.intValue < 1) || directionElem.intValue > 4)
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
                    case EGimmickType.RandomTornado : {
                            var directionsProp = gimmickDataProp.FindPropertyRelative("targetDirections");
                            var linesProp = gimmickDataProp.FindPropertyRelative("targetLines");

                            // ターゲット数は少なくても1
                            directionsProp.arraySize = Math.Max(directionsProp.arraySize, 1);
                            EditorGUILayout.PropertyField(directionsProp.FindPropertyRelative("Array.size"), new GUIContent("Number Of Target"));

                            // 方向とlineをRandomに設定しておく
                            var targetNum = linesProp.arraySize = directionsProp.arraySize;
                            for (var i = 0 ; i < targetNum ; ++i) {
                                var directionElem = directionsProp.GetArrayElementAtIndex(i);
                                var lineElem = linesProp.GetArrayElementAtIndex(i);
                                directionElem.intValue = -1;
                                lineElem.intValue = -1;
                            }

                            // 重みリストの設定
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
                            break;
                        }
                    case EGimmickType.Meteorite: {
                            var rowProp = gimmickDataProp.FindPropertyRelative("targetRow");
                            var colProp = gimmickDataProp.FindPropertyRelative("targetColumn");

                            if (rowProp.intValue < 1 || rowProp.intValue > 5)
                                rowProp.intValue = 1;
                            rowProp.intValue = (int)(ERow)EditorGUILayout.EnumPopup(
                                    label: new GUIContent("Row"),
                                    selected: (ERow)rowProp.intValue,
                                    //ランダムは選択不能にする
                                    checkEnabled: (eType) => (ERow)eType != ERow.Random,
                                    includeObsolete: false
                                );

                            if (colProp.intValue < 1 || colProp.intValue > 5)
                                colProp.intValue = 1;
                            colProp.intValue = (int)(EColumn)EditorGUILayout.EnumPopup(
                                    label: new GUIContent("ColuEColumn"),
                                    selected: (EColumn)colProp.intValue,
                                    //ランダムは選択不能にする
                                    checkEnabled: (eType) => (EColumn)eType != EColumn.Random,
                                    includeObsolete: false
                                );
                            break;
                        }
                    case EGimmickType.RandomMeteorite: {
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
                            break;
                        }
                    case EGimmickType.AimingMeteorite: {
                            var targetBottleProp = gimmickDataProp.FindPropertyRelative("targetBottle");

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
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        private void DrawBulletGroupList()
        {
            this.DrawArrayProperty(_bulletGroupDatasProp, (bulletGroupDataProp, index) => {
                bulletGroupDataProp.isExpanded = EditorGUILayout.Foldout(bulletGroupDataProp.isExpanded, $"Bullet Group {index + 1}");

                if (!bulletGroupDataProp.isExpanded) return;

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(bulletGroupDataProp.FindPropertyRelative("appearTime"));
                EditorGUILayout.PropertyField(bulletGroupDataProp.FindPropertyRelative("interval"));
                EditorGUILayout.PropertyField(bulletGroupDataProp.FindPropertyRelative("loop"));

                var bulletListProp = bulletGroupDataProp.FindPropertyRelative("bullets");

                this.DrawArrayProperty(bulletListProp, (bulletDataProp, index2) => {
                    bulletDataProp.isExpanded = EditorGUILayout.Foldout(bulletDataProp.isExpanded, $"Bullet {index2 + 1}");

                    if (!bulletDataProp.isExpanded) return;

                    var bulletTypeProp = bulletDataProp.FindPropertyRelative("type");

                    bulletTypeProp.enumValueIndex = (int)(EBulletType)EditorGUILayout.EnumPopup(
                            label: new GUIContent("Type"),
                            selected: (EBulletType)bulletTypeProp.enumValueIndex
                        );

                    EditorGUILayout.PropertyField(bulletDataProp.FindPropertyRelative("ratio"));

                    switch ((EBulletType)bulletTypeProp.enumValueIndex) {
                        case EBulletType.NormalHole: {
                                var rowProp  =  bulletDataProp.FindPropertyRelative("row");
                                var columnProp  =  bulletDataProp.FindPropertyRelative("column");

                                if (rowProp.intValue == (int)ERow.Random)
                                    // 行がランダムの場合強制に変える
                                    rowProp.intValue = (int)ERow.First;

                                if (columnProp.intValue == (int)EColumn.Random)
                                    // 列がランダムの場合強制に変える
                                    columnProp.intValue = (int)EColumn.Left;

                                rowProp.intValue  =  (int)(ERow)EditorGUILayout.EnumPopup(
                                        label:  new GUIContent("Row"),
                                        selected:  (ERow)rowProp.intValue,
                                        // ランダムは選択不能にする
                                        checkEnabled:  (eType)  =>  (ERow)eType  !=  ERow.Random,
                                        includeObsolete:  false
                                    );

                                columnProp.intValue  =  (int)(ERow)EditorGUILayout.EnumPopup(
                                        label:  new GUIContent("Column"),
                                        selected:  (EColumn)columnProp.intValue,
                                        // ランダムは選択不能にする
                                        checkEnabled:  (eType)  =>  (EColumn)eType  !=  EColumn.Random,
                                        includeObsolete:  false
                                    );

                                break;
                            }
                        case EBulletType.AimingHole: {
                                var aimingBottlesProp = bulletDataProp.FindPropertyRelative("aimingBottles");
                                for (var i = 0 ; i < aimingBottlesProp.arraySize ; i++) {
                                    var aimingBottleProp = aimingBottlesProp.GetArrayElementAtIndex(i);
                                    aimingBottleProp.intValue = Math.Min(aimingBottleProp.intValue, _numOfAttackableBottles);
                                }

                                this.DrawArrayProperty(aimingBottlesProp);

                                break;
                            }
                        case EBulletType.RandomNormalHole: {
                                var rowProp  =  bulletDataProp.FindPropertyRelative("row");
                                var columnProp  =  bulletDataProp.FindPropertyRelative("column");

                                rowProp.intValue = (int)(ERow.Random);
                                rowProp.intValue = (int)(ERow)EditorGUILayout.EnumPopup(
                                        label: new GUIContent("Row"),
                                        selected: (ERow)rowProp.intValue,
                                        checkEnabled: (eType) => (ERow)eType == ERow.Random,
                                        includeObsolete: false
                                    );

                                columnProp.intValue = (int)(EColumn.Random);
                                columnProp.intValue = (int)(EColumn)EditorGUILayout.EnumPopup(
                                        label: new GUIContent("Column"),
                                        selected: (EColumn)columnProp.intValue,
                                        checkEnabled: (eType) => (EColumn)eType == EColumn.Random,
                                        includeObsolete: false
                                    );

                                this.DrawFixedSizeArrayProperty(bulletDataProp.FindPropertyRelative("randomRow"), Enum.GetValues(typeof(ERow)).Length - 1);
                                this.DrawFixedSizeArrayProperty(bulletDataProp.FindPropertyRelative("randomColumn"), Enum.GetValues(typeof(EColumn)).Length - 1);

                                break;
                            }
                        case EBulletType.RandomAimingHole: {
                                this.DrawFixedSizeArrayProperty(bulletDataProp.FindPropertyRelative("randomAttackableBottles"), _numOfAttackableBottles, RenderRandomAttackableBottlesElement);
                                break;
                            }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
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
