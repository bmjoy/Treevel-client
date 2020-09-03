using System.Collections;
using System;
using System.Linq;
using Project.Scripts.GameDatas;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Gimmick
{
    public class GustWindController : AbstractGimmickController
    {
        [SerializeField] private float MoveBottleSpeed = 6.0f;

        private const float TIME_BEFORE_WARNING = 2.0f;

        private EGimmickDirection _targetDirection;

        private int _targetLine;

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);
            _targetDirection = gimmickData.targetDirection;
            switch (_targetDirection) {
                case EGimmickDirection.ToLeft:
                case EGimmickDirection.ToRight:
                    _targetLine = (int)gimmickData.targetRow;
                    break;
                case EGimmickDirection.ToBottom:
                case EGimmickDirection.ToUp:
                    _targetLine = (int)gimmickData.targetColumn;
                    break;
                case EGimmickDirection.Random:
                default:
                    throw new NotImplementedException();
            }
        }

        public override IEnumerator Trigger()
        {
            yield return new WaitForSeconds(TIME_BEFORE_WARNING);

            // 警告出す

            // 警告終わり

            // 攻撃演出開始

            // 目標ボトルを移動させる
            var targetBottles = GetTargetBottles();
            var destinationTiles = GetDestinationTiles();
            var index = 0;
            foreach (var bottle in targetBottles) {
                BoardManager.Instance.Move(bottle, destinationTiles[index++], GetMoveDirection());
            }
        }

        private Vector2Int GetMoveDirection()
        {
            switch (_targetDirection) {
                case EGimmickDirection.ToLeft:
                    return Vector2Int.left;
                case EGimmickDirection.ToRight:
                    return Vector2Int.right;
                case EGimmickDirection.ToUp:
                    return Vector2Int.up;
                case EGimmickDirection.ToBottom:
                    return Vector2Int.down;
                case EGimmickDirection.Random:
                default:
                    throw new NotImplementedException();
            }
        }

        private int[] GetDestinationTiles()
        {
            switch (_targetDirection) {
                case EGimmickDirection.ToLeft: {
                        var start = (_targetLine - 1) * StageSize.COLUMN + 1;
                        return Enumerable.Range(start, StageSize.COLUMN).ToArray();
                    }
                case EGimmickDirection.ToRight: {
                        var start = (_targetLine - 1) * StageSize.COLUMN + 1;
                        return Enumerable.Range(start, StageSize.COLUMN).Reverse().ToArray();
                    }
                case EGimmickDirection.ToBottom: {
                        var ret = new int[StageSize.ROW];
                        for (var i = 0 ; i < StageSize.ROW ; ++i) {
                            ret[i] = _targetLine + StageSize.COLUMN * i;
                        }
                        return ret;
                    }
                case EGimmickDirection.ToUp: {
                        var ret = new int[StageSize.ROW];
                        for (var i = 0 ; i < StageSize.ROW ; ++i) {
                            ret[i] = _targetLine + StageSize.COLUMN * i;
                        }
                        return ret.Reverse().ToArray();
                    }
                case EGimmickDirection.Random:
                default:
                    throw new NotImplementedException();
            }
        }

        private DynamicBottleController[] GetTargetBottles()
        {
            var bottleObjsOnTargetLine = GimmickLibrary.IsHorizontal(_targetDirection) ?
                BoardManager.Instance.GetBottlesOnRow((ERow) _targetLine) :
                BoardManager.Instance.GetBottlesOnColumn((EColumn) _targetLine);

            var targetBottles = bottleObjsOnTargetLine
                .Where(go => go.GetComponent<DynamicBottleController>() != null)
                .Select(go => go.GetComponent<DynamicBottleController>())
                .OrderBy(go => BoardManager.Instance.GetBottlePos(go.GetComponent<AbstractBottleController>()))
                .ToArray();

            return targetBottles;
        }
    }
}
