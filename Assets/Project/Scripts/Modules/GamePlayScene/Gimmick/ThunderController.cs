using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Gimmick
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class ThunderController : AbstractGimmickController
    {
        /// <summary>
        /// 雲をタイルの比率でオフセットさせる
        /// </summary>
        private const float _CLOUD_OFFSET_BY_TILE_RATIO = 0.6f;

        /// <summary>
        /// Idleステートのハッシュ値
        /// </summary>
        private static readonly int _IDLE_STATE_NAME_HASH = Animator.StringToHash("Idle");

        /// <summary>
        /// 移動速度
        /// </summary>
        [SerializeField] private float _moveSpeed = 1.5f;

        /// <summary>
        /// 目標タイル
        /// </summary>
        private List<(ERow row, EColumn column)> _targets;

        /// <summary>
        /// 雲オブジェクト
        /// </summary>
        [SerializeField] private GameObject _cloud;

        // 各コンポーネント
        private Animator _animator;
        private Rigidbody2D _rigidBody;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidBody = GetComponent<Rigidbody2D>();

            // 雲をタイルの少し上に移動する
            _cloud.transform.Translate(0, _CLOUD_OFFSET_BY_TILE_RATIO * GameWindowController.Instance.GetTileHeight(), 0);
        }

        protected virtual void OnEnable()
        {
            base.OnEnable();
            Observable.Merge(GamePlayDirector.Instance.GameSucceeded, GamePlayDirector.Instance.GameFailed)
            .Subscribe(_ => {
                // 動きを止める
                _rigidBody.velocity = Vector2.zero;
                // アニメーション、SEを止める
                _animator.speed = 0;
                SoundManager.Instance.StopSE(ESEKey.SE_ThunderAttack);
            }).AddTo(this);
        }

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);

            _targets = gimmickData.targets
                // 無効な値を除外する
                .SkipWhile((vec) => vec.x < (int)ERow.First || (int)ERow.Fifth < vec.x || vec.y < (int)EColumn.Left || (int)EColumn.Right < vec.y)
                // List<(ERow, EColumn)>に変換する
                .Select((vec) => ((ERow)vec.x, (EColumn)vec.y)).ToList();

            Debug.Assert(_targets.Count > 0, "Invalid Gimmick Data");

            // 初期位置設定
            var initPos = BoardManager.Instance.GetTilePos(_targets[0].row, _targets[0].column);
            transform.position = initPos;

            // 描画順序の調整
            _cloud.GetComponent<Renderer>().sortingOrder++;
        }

        public override async UniTask Trigger(CancellationToken token)
        {
            try {
                // 表示ON
                foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>()) {
                    spriteRenderer.enabled = true;
                }

                var targetEnumerator = _targets.GetEnumerator();
                while (targetEnumerator.MoveNext()) {
                    var targetPos = BoardManager.Instance.GetTilePos(targetEnumerator.Current.row,
                            targetEnumerator.Current.column);

                    // 移動ベクトル設定
                    _rigidBody.velocity = (targetPos - (Vector2) transform.position).normalized * _moveSpeed;

                    // 目標地まで移動する
                    await UniTask.WaitUntil(() =>
                        Vector2.Dot(targetPos - (Vector2) transform.position, _rigidBody.velocity) <= 0, cancellationToken: token);

                    // 移動停止
                    _rigidBody.velocity = Vector2.zero;

                    // 警告 -> 攻撃
                    _animator.SetTrigger("Warning");

                    // すぐには遷移してくれないそうなので、次のステート（警告）に遷移するまでちょっと待つ
                    await UniTask.WaitUntil(() =>
                        _animator.GetCurrentAnimatorStateInfo(0).shortNameHash != _IDLE_STATE_NAME_HASH, cancellationToken: token);

                    // 攻撃アニメーション終わるまで待つ
                    await UniTask.WaitUntil(() =>
                        _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == _IDLE_STATE_NAME_HASH &&
                        !SoundManager.Instance.IsPlayingSE(ESEKey.SE_ThunderAttack), cancellationToken: token);
                }

                // TODO:退場演出
                Destroy(gameObject);
            } catch (OperationCanceledException) {
            }
        }

        /// <summary>
        /// 攻撃する（アニメーションイベントから呼び出す）
        /// </summary>
        private void Attack()
        {
            SoundManager.Instance.PlaySE(ESEKey.SE_ThunderAttack);
        }
    }
}
