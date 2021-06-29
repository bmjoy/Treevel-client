using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Gimmick
{
    [RequireComponent(typeof(Animator))]
    public class GustWindController : GimmickControllerBase
    {
        /// <summary>
        /// 攻撃方向
        /// </summary>
        private EDirection _targetDirection;

        /// <summary>
        /// 対象行・列
        /// </summary>
        private int _targetLine;

        /// <summary>
        /// 攻撃アニメーション開始時の位置
        /// </summary>
        private Vector3 _attackStartPos;

        /// <summary>
        /// 攻撃アニメーション終了時の位置
        /// </summary>
        private Vector3 _attackEndPos;

        /// <summary>
        /// 攻撃アニメーション中の移動総距離（縦の場合を基準とする）
        /// </summary>
        private float _attackMoveDistance;

        /// <summary>
        /// ボトル動かしたか
        /// </summary>
        private bool _isBottleMoved = false;

        /// <summary>
        /// GustWind本体のRenderer
        /// </summary>
        private SpriteRenderer _coreSpriteRenderer;

        /// <summary>
        /// GustWind本体の当たり判定
        /// </summary>
        private BoxCollider2D _coreBoxCollider2D;

        private const string _ANIMATOR_PARAM_TRIGGER_WARNING = "Warning";
        private const string _ATTACK_ANIMATION_CLIP_NAME = "GustWind@attack";
        private static readonly int _ATTACK_STATE_NAME_HASH = Animator.StringToHash("GustWind@attack");

        private Animator _animator;

        [SerializeField] private ParticleSystem _leafParticle;

        private void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
            var core = transform.Find("Core");
            _coreSpriteRenderer = core.GetComponent<SpriteRenderer>();
            _coreBoxCollider2D = core.GetComponent<BoxCollider2D>();
            this.OnTriggerEnter2DAsObservable()
                .Where(_ => !_isBottleMoved)
                .Subscribe(_ => {
                    _isBottleMoved = true;
                    MoveBottles();
                }).AddTo(this);
            GamePlayDirector.Instance.GameEnd.Subscribe(_ => {
                _animator.speed = 0;
                _leafParticle.Pause();
            }).AddTo(this);
        }

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);

            _targetDirection = gimmickData.targetDirection;
            // 縦の時は、ピッタリ画面端の外から画面もう一端の外まで動かせるように、
            // 横の時は、ピッタリ画面端からスタートし、縦と同じ距離を移動する（アニメーションの秒数でスピードを決めているので）
            _attackMoveDistance = Constants.WindowSize.HEIGHT + _coreSpriteRenderer.size.y * transform.localScale.y;
            switch (_targetDirection) {
                case EDirection.ToLeft:
                case EDirection.ToRight: {
                    _targetLine = (int)gimmickData.targetRow;

                    var sign = _targetDirection == EDirection.ToLeft ? 1 : -1;
                    var centerTilePos = BoardManager.Instance.GetTilePos(gimmickData.targetRow, EColumn.Center);
                    var yPos = centerTilePos.y;
                    var startX = sign * (Constants.WindowSize.WIDTH + _coreSpriteRenderer.size.y * transform.localScale.y) *
                                 0.5f;
                    var endX = startX + -sign * _attackMoveDistance;

                    _attackStartPos = new Vector2(startX, yPos);
                    _attackEndPos = new Vector2(endX, yPos);


                    transform.position = centerTilePos;
                    transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-sign, 1, 1));
                    transform.Rotate(Quaternion.Euler(0, 0, sign * 90).eulerAngles);

                    break;
                }
                case EDirection.ToDown:
                case EDirection.ToUp: {
                    _targetLine = (int)gimmickData.targetColumn;

                    var sign = _targetDirection == EDirection.ToUp ? -1 : 1;
                    var centerTilePos = BoardManager.Instance.GetTilePos(ERow.Third, gimmickData.targetColumn);
                    var xPos = centerTilePos.x;
                    var startY = sign * _attackMoveDistance * 0.5f;
                    var endY = -startY;

                    _attackStartPos = new Vector2(xPos, startY);
                    _attackEndPos = new Vector2(xPos, endY);

                    transform.position = centerTilePos;
                    transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1, -sign, 1));
                    break;
                }
                default:
                    throw new NotImplementedException();
            }
        }

        public override IEnumerator Trigger()
        {
            // 警告出す
            _animator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_WARNING);

            // Attackまで待つ
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == _ATTACK_STATE_NAME_HASH);

            transform.position = _attackStartPos;
            _coreSpriteRenderer.enabled = true;
            _coreBoxCollider2D.enabled = true;
            SoundManager.Instance.PlaySE(ESEKey.Gimmick_Gust);
            yield return MoveDuringAttack();

            Destroy(gameObject);
        }

        /// <summary>
        /// 攻撃アニメーション中の移動計算
        /// </summary>
        private IEnumerator MoveDuringAttack()
        {
            // 攻撃のクリップの長さ、スタート位置、終了位置からスピードを算出
            var attackAnimClip = _animator.runtimeAnimatorController.animationClips.Single(c => c.name == _ATTACK_ANIMATION_CLIP_NAME);
            var attackAnimationTime = attackAnimClip.length;
            var speed = _attackMoveDistance / attackAnimationTime;

            var direction = (Vector3)(Vector3Int)_targetDirection.GetVectorInt();
            while ((_attackEndPos - transform.position).normalized == direction) {
                transform.Translate(direction * speed * Time.fixedDeltaTime, Space.World);
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// 対象行・列上のボトルを一斉動かす
        /// </summary>
        private void MoveBottles()
        {
            // 目標ボトルを取得
            var targetBottles = GetTargetBottles();
            // 目標行・列上のタイルを取得
            var destinationTiles = GetDestinationTiles().ToArray();
            foreach (var bottle in targetBottles) {
                // ボトルが今いるタイルのインデックスを探す
                var currTileIdx =
                    Array.FindIndex(destinationTiles, n => n == BoardManager.Instance.GetBottlePos(bottle));
                while (currTileIdx + 1 < destinationTiles.Length) {
                    var bottleOnTargetTile = BoardManager.Instance.GetBottle(destinationTiles[currTileIdx + 1]);
                    // 次のタイルがStaticBottleの場合はループ終了、そうでない場合は次のタイルへ進む
                    if (bottleOnTargetTile && bottleOnTargetTile.GetComponent<StaticBottleController>() != null) {
                        break;
                    } else {
                        currTileIdx++;
                    }
                }

                BoardManager.Instance.MoveAsync(bottle, destinationTiles[currTileIdx], _targetDirection.GetVectorInt());
                // 移動完了のボトルがいるタイル以降は次のボトルの選択肢から外す
                destinationTiles = destinationTiles.Take(currTileIdx).ToArray();
            }
        }

        /// <summary>
        /// ボトルの移動先配列
        /// </summary>
        private IEnumerable<int> GetDestinationTiles()
        {
            switch (_targetDirection) {
                case EDirection.ToLeft: {
                    var start = _targetLine * Constants.StageSize.COLUMN + 1;
                    return Enumerable.Range(start, Constants.StageSize.COLUMN).Reverse();
                }
                case EDirection.ToRight: {
                    var start = _targetLine * Constants.StageSize.COLUMN + 1;
                    return Enumerable.Range(start, Constants.StageSize.COLUMN);
                }
                case EDirection.ToUp: {
                    var ret = new int[Constants.StageSize.ROW];
                    for (var i = 0; i < Constants.StageSize.ROW; ++i) {
                        ret[i] = _targetLine + 1 + Constants.StageSize.COLUMN * i;
                    }

                    return ret.Reverse();
                }
                case EDirection.ToDown: {
                    var ret = new int[Constants.StageSize.ROW];
                    for (var i = 0; i < Constants.StageSize.ROW; ++i) {
                        ret[i] = _targetLine + 1 + Constants.StageSize.COLUMN * i;
                    }

                    return ret;
                }
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 目標行・列上の全てのボトルを取得し、攻撃方向から移動する順序を決めてソートする
        /// </summary>
        private IEnumerable<DynamicBottleController> GetTargetBottles()
        {
            var bottleObjsOnTargetLine = GimmickLibrary.IsHorizontal(_targetDirection)
                ? BoardManager.Instance.GetBottlesOnRow((ERow)_targetLine)
                : BoardManager.Instance.GetBottlesOnColumn((EColumn)_targetLine);

            var targetBottles = bottleObjsOnTargetLine
                .Where(go => go.GetComponent<DynamicBottleController>() != null)
                .Select(go => go.GetComponent<DynamicBottleController>());

            switch (_targetDirection) {
                case EDirection.ToLeft:
                case EDirection.ToUp:
                    return targetBottles
                        .OrderBy(go => BoardManager.Instance.GetBottlePos(go.GetComponent<BottleControllerBase>()));
                case EDirection.ToDown:
                case EDirection.ToRight:
                    return targetBottles
                        .OrderByDescending(go => BoardManager.Instance.GetBottlePos(go.GetComponent<BottleControllerBase>()));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
