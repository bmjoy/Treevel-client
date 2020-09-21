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

            var coreSprite = transform.Find("Core").GetComponent<SpriteRenderer>();
            _targetDirection = gimmickData.targetDirection;
            // 縦の時は、ピッタリ画面端の外から画面もう一端の外まで動かせるように、
            // 横の時は、ピッタリ画面端からスタートし、縦と同じ距離を移動する（アニメーションの秒数でスピードを決めているので）
            _attackMoveDistance = WindowSize.HEIGHT + coreSprite.size.y * transform.localScale.y;
            switch (_targetDirection) {
                case EGimmickDirection.ToLeft:
                case EGimmickDirection.ToRight: {
                        _targetLine = (int)gimmickData.targetRow;

                        var sign = _targetDirection == EGimmickDirection.ToRight ? 1 : -1;
                        var yPos = BoardManager.Instance.GetTilePos(gimmickData.targetRow, EColumn.Center).y;
                        var startX = -sign * (WindowSize.WIDTH + coreSprite.size.y * transform.localScale.y) * 0.5f;
                        var endX = startX + sign * _attackMoveDistance;

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
                        var xPos = BoardManager.Instance.GetTilePos(ERow.Third, gimmickData.targetColumn).x;
                        var startY = sign * _attackMoveDistance * 0.5f;
                        var endY = -startY;

                        _attackStartPos = new Vector2(xPos, startY);
                        _attackEndPos = new Vector2(xPos, endY);

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

            var direction = (Vector3)(Vector3Int)GimmickLibrary.GetDirectionVector(_targetDirection);
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
            foreach (var bottle in targetBottles) {
                // ボトルが今いるタイルのインデックスを探す
                var currTileIdx = Array.FindIndex(destinationTiles, n => n == BoardManager.Instance.GetBottlePos(bottle));
                while (currTileIdx + 1 < destinationTiles.Length) {
                    var bottleOnTargetTile = BoardManager.Instance.GetBottle(destinationTiles[currTileIdx + 1]);
                    // 次のタイルがStaticBottleの場合はループ終了、そうでない場合は次のタイルへ進む
                    if (bottleOnTargetTile && bottleOnTargetTile.GetComponent<StaticBottleController>() != null) {
                        break;
                    } else {
                        currTileIdx++;
                    }
                }

                BoardManager.Instance.Move(bottle, destinationTiles[currTileIdx], GimmickLibrary.GetDirectionVector(_targetDirection));
                // 移動完了のボトルがいるタイル以降は次のボトルの選択肢から外す
                destinationTiles = destinationTiles.Take(currTileIdx).ToArray();
            }
        }

        /// <summary>
        /// ボトルの移動先配列
        /// </summary>
        private int[] GetDestinationTiles()
        {
            switch (_targetDirection) {
                case EGimmickDirection.ToLeft: {
                        var start = (_targetLine - 1) * StageSize.COLUMN + 1;
                        return Enumerable.Range(start, StageSize.COLUMN).Reverse().ToArray();
                    }
                case EGimmickDirection.ToRight: {
                        var start = (_targetLine - 1) * StageSize.COLUMN + 1;
                        return Enumerable.Range(start, StageSize.COLUMN).ToArray();
                    }
                case EGimmickDirection.ToUp: {
                        var ret = new int[StageSize.ROW];
                        for (var i = 0 ; i < StageSize.ROW ; ++i) {
                            ret[i] = _targetLine + StageSize.COLUMN * i;
                        }
                        return ret.Reverse().ToArray();
                    }
                case EGimmickDirection.ToBottom: {
                        var ret = new int[StageSize.ROW];
                        for (var i = 0 ; i < StageSize.ROW ; ++i) {
                            ret[i] = _targetLine + StageSize.COLUMN * i;
                        }
                        return ret;
                    }
                case EGimmickDirection.Random:
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 目標行・列上の全てのボトルを取得し、攻撃方向から移動する順序を決めてソートする
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
