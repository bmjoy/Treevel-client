using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Utils;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Gimmick
{
    [RequireComponent(typeof(Animator))]
    public class SolarBeamController : AbstractGimmickController
    {
        [SerializeField] private GameObject _sunObject;

        [SerializeField] private GameObject _beamObject;

        /// <summary>
        /// 登場から警告出すまでの時間
        /// </summary>
        [SerializeField] private float _idleTime = 2.0f;

        #region アニメータ用パラメータ
        private const string _ANIMATOR_PARAM_TRIGGER_WARNING = "Warning";
        private const string _ANIMATOR_PARAM_BOOL_LAST_ATTACK = "LastAttack";

        /// <summary>
        /// IDLEステートのハッシュ値
        /// </summary>
        private static readonly int _IDLE_STATE_NAME_HASH = Animator.StringToHash("SolarBeam@idle");

        #endregion

        private Animator _animator;

        /// <summary>
        /// 攻撃回数
        /// </summary>
        private int _attackTimes;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        protected virtual void OnEnable()
        {
            base.OnEnable();
            Observable.Merge(GamePlayDirector.Instance.GameSucceeded, GamePlayDirector.Instance.GameFailed)
            .Subscribe(_ => {
                _animator.speed = 0;
            }).AddTo(this);
        }

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);

            _attackTimes = gimmickData.attackTimes;
            var direction = gimmickData.targetDirection;
            var sunRenderer = _sunObject.GetComponent<SpriteRenderer>();
            // TODO: ランダム方向の実装
            switch (direction) {
                case EDirection.ToLeft:
                case EDirection.ToRight: {
                        var row = gimmickData.targetRow;

                        // 親オブジェクトの位置設定
                        var initialPos = BoardManager.Instance.GetTilePos(row, EColumn.Center);
                        transform.position = initialPos;

                        // 太陽とビームの位置を調整する
                        var sign = direction == EDirection.ToLeft ? 1 : -1;
                        // 中央から1.5タイルサイズ＋1.5太陽の幅分ずらす
                        var offsetTileCount = Constants.StageSize.COLUMN / 2.0f;
                        // 0.5個分太陽の幅をずらす
                        var offsetSunCount = 0.5f * sunRenderer.bounds.size.x;
                        var offset = new Vector2(GameWindowController.Instance.GetTileWidth() * offsetTileCount + offsetSunCount, 0);
                        _sunObject.transform.position = initialPos + sign * offset;
                        // ToRightの場合はx反転
                        _sunObject.transform.localScale = Vector3.Scale(_sunObject.transform.localScale, new Vector3(sign, 1, 1));
                        sunRenderer.enabled = true;

                        break;
                    }
                case EDirection.ToUp:
                case EDirection.ToDown: {
                        var col = gimmickData.targetColumn;

                        // 親オブジェクトの位置設定
                        var initialPos = BoardManager.Instance.GetTilePos(ERow.Third, col);
                        transform.position = initialPos;

                        // 太陽とビームの位置を調整する
                        var sign = direction == EDirection.ToDown ? 1 : -1;

                        // 中央からタイル2.5個分ずらす
                        var offsetTileCount = Constants.StageSize.ROW / 2.0f;
                        // 0.5個分太陽の高さをずらす
                        var offsetSunCount = 0.5f * sunRenderer.bounds.size.y;
                        var offset = new Vector2(0, GameWindowController.Instance.GetTileHeight() * offsetTileCount + offsetSunCount);
                        _sunObject.transform.position = initialPos + sign * offset;
                        sunRenderer.enabled = true;

                        // ビームを縦にする
                        _beamObject.transform.Rotate(Quaternion.Euler(0, 0, sign * 90).eulerAngles);
                        // ビームと太陽の距離を保持
                        var beamSunDistance = Vector3.Distance(_beamObject.transform.position, _sunObject.transform.position);
                        // ビームの位置を太陽の上／下にする
                        _beamObject.transform.position = _sunObject.transform.position + sign * Vector3.down * beamSunDistance;
                        break;
                    }
                default:
                    throw new System.NotImplementedException();
            }
        }

        public override async UniTask Trigger(CancellationToken token)
        {
            try
            {
                // 入場アニメーション再生完了まで待つ
                await UniTask.WaitUntil(() =>
                    _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == _IDLE_STATE_NAME_HASH, cancellationToken: token);

                while (_attackTimes-- > 0)
                {
                    if (_attackTimes == 0)
                        _animator.SetBool(_ANIMATOR_PARAM_BOOL_LAST_ATTACK, true);

                    // 待機時間待つ
                    await UniTask.DelayFrame((int) (GamePlayDirector.FRAME_RATE * _idleTime), cancellationToken: token);

                    // 警告→攻撃→退場
                    _animator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_WARNING);

                    // idle -> warning のTransition待ち
                    await UniTask.WaitUntil(() =>
                        _animator.GetCurrentAnimatorStateInfo(0).shortNameHash != _IDLE_STATE_NAME_HASH, cancellationToken: token);

                    // 最終回以外はIDLEに戻るまで待つ
                    if (_attackTimes > 0)
                        await UniTask.WaitUntil(() =>
                            _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == _IDLE_STATE_NAME_HASH, cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        /// 破棄する（アニメーションイベントから呼び出す）
        /// </summary>
        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
