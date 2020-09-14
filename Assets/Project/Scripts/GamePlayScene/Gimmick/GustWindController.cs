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
    [RequireComponent(typeof(Animator))]
    public class GustWindController : AbstractGimmickController
    {
        [SerializeField] private float MoveBottleSpeed = 6.0f;

        private const float TIME_BEFORE_WARNING = 2.0f;

        private EGimmickDirection _targetDirection;

        private int _targetLine;

        private Animator _animator;

        private const string _ATTACK_ANIMATION_CLIP_NAME = "GustWind@attack";

        private static readonly int _ATTACK_STATE_NAME_HASH = Animator.StringToHash("GustWind@attack");

        private Vector3 _attackStartPos;
        private Vector3 _attackEndPos;
        private float _attackMoveDistance;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);
            _targetDirection = gimmickData.targetDirection;
            _attackMoveDistance = WindowSize.HEIGHT + GetComponent<SpriteRenderer>().size.y * transform.localScale.y;
            switch (_targetDirection) {
                case EGimmickDirection.ToLeft:
                case EGimmickDirection.ToRight: {
                        _targetLine = (int)gimmickData.targetRow;

                        var sign = _targetDirection == EGimmickDirection.ToRight ? 1 : -1;
                        var yPos = BoardManager.Instance.GetTilePos(gimmickData.targetRow, EColumn.Center).y;
                        var startX = -sign * _attackMoveDistance * 0.5f;
                        var endX = -startX;

                        _attackStartPos = new Vector2(startX, yPos);
                        _attackEndPos = new Vector2(endX, yPos);

                        transform.position = _attackStartPos;
                        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-sign, 1, 1));
                        transform.Rotate(Quaternion.Euler(0, 0, sign * 90).eulerAngles);
                        break;
                    }
                case EGimmickDirection.ToBottom:
                case EGimmickDirection.ToUp: {
                        _targetLine = (int)gimmickData.targetColumn;

                        var sign = _targetDirection == EGimmickDirection.ToBottom ? 1 : -1;
                        var xCord = BoardManager.Instance.GetTilePos(ERow.Third, gimmickData.targetColumn).x;
                        var startY = sign * _attackMoveDistance * 0.5f;
                        var endY = -startY;

                        _attackStartPos = new Vector2(xCord, startY);
                        _attackEndPos = new Vector2(xCord, endY);

                        transform.position = _attackStartPos;
                        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1, sign, 1));
                        break;
                    }
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
            _animator.SetTrigger("Attack");

            // AttackまでのTransition待ち
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash != _ATTACK_STATE_NAME_HASH);

            yield return MoveDuringAttack();

            Destroy(gameObject);
        }

        private IEnumerator MoveDuringAttack()
        {
            // 攻撃のクリップの長さ、スタート位置、終了位置からスピードを算出
            var attackAnimClip = _animator.runtimeAnimatorController.animationClips.Single(c => c.name == _ATTACK_ANIMATION_CLIP_NAME);
            var attackAnimationTime = attackAnimClip.length;
            var speed = _attackMoveDistance / attackAnimationTime;

            var direction = (_attackEndPos - _attackStartPos).normalized;
            while ((_attackEndPos - transform.position).normalized == direction) {
                transform.Translate(direction * speed * Time.fixedDeltaTime, Space.World);
                yield return new WaitForFixedUpdate();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            MoveBottles();
        }

        private void MoveBottles()
        {
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

        /// <summary>
        /// ボトルの移動先配列
        /// </summary>
        private int[] GetDestinationTiles()
        {
            int[] candidateList = null;
            switch (_targetDirection) {
                case EGimmickDirection.ToLeft: {
                        var start = (_targetLine - 1) * StageSize.COLUMN + 1;
                        candidateList = Enumerable.Range(start, StageSize.COLUMN).Reverse().ToArray();
                        break;
                    }
                case EGimmickDirection.ToRight: {
                        var start = (_targetLine - 1) * StageSize.COLUMN + 1;
                        candidateList = Enumerable.Range(start, StageSize.COLUMN).ToArray();
                        break;
                    }
                case EGimmickDirection.ToUp: {
                        var ret = new int[StageSize.ROW];
                        for (var i = 0 ; i < StageSize.ROW ; ++i) {
                            ret[i] = _targetLine + StageSize.COLUMN * i;
                        }
                        candidateList = ret.Reverse().ToArray();
                        break;
                    }
                case EGimmickDirection.ToBottom: {
                        var ret = new int[StageSize.ROW];
                        for (var i = 0 ; i < StageSize.ROW ; ++i) {
                            ret[i] = _targetLine + StageSize.COLUMN * i;
                        }
                        candidateList = ret;
                        break;
                    }
                case EGimmickDirection.Random:
                default:
                    throw new NotImplementedException();
            }

            // 障害物があるまで取る
            return candidateList.TakeWhile(tileNum => {
                var bottleOnTile = BoardManager.Instance.GetBottle(tileNum);
                return bottleOnTile == null || bottleOnTile.GetComponent<AbstractBottleController>() is DynamicBottleController;
            }).Reverse().ToArray();
        }

        /// <summary>
        /// 目標行・列上の全てのボトルを取得
        /// </summary>
        private DynamicBottleController[] GetTargetBottles()
        {
            var bottleObjsOnTargetLine = GimmickLibrary.IsHorizontal(_targetDirection) ?
                BoardManager.Instance.GetBottlesOnRow((ERow) _targetLine) :
                BoardManager.Instance.GetBottlesOnColumn((EColumn) _targetLine);

            var targetBottles = bottleObjsOnTargetLine
                .Where(go => go.GetComponent<DynamicBottleController>() != null)
                .Select(go => go.GetComponent<DynamicBottleController>());

            switch (_targetDirection) {
                case EGimmickDirection.ToLeft:
                case EGimmickDirection.ToUp:
                    return targetBottles.OrderBy(go => BoardManager.Instance.GetBottlePos(go.GetComponent<AbstractBottleController>())).ToArray();
                case EGimmickDirection.ToBottom:
                case EGimmickDirection.ToRight:
                    return targetBottles.OrderByDescending(go => BoardManager.Instance.GetBottlePos(go.GetComponent<AbstractBottleController>())).ToArray();
                case EGimmickDirection.Random:
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
