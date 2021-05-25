using System;
using System.Collections;
using System.Collections.Generic;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Modules.GamePlayScene.Bottle;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Gimmick
{
    public abstract class GimmickControllerBase : GameObjectControllerBase
    {
        /// <summary>
        /// 警告の表示秒数
        /// </summary>
        [SerializeField] protected float _warningDisplayTime = 1.0f;

        public EGimmickType GimmickType { get; private set; }

        private static short _gimmickId = short.MinValue;

        /// <summary>
        /// OnEnterで購読したボトルとOnExitで購読解除するボトルのDisposableの対応関係を保持するための辞書
        /// </summary>
        private readonly Dictionary<BottleControllerBase, IDisposable> _invincibleAfterDamagedExpiredDisposables = new Dictionary<BottleControllerBase, IDisposable>();

        protected void Awake()
        {
            // 衝突イベントを処理する
            this.OnTriggerEnter2DAsObservable()
                .Select(other => other.GetComponent<BottleControllerBase>())
                .Where(bottle => bottle && bottle.IsInvincibleAfterDamaged)
                .Subscribe(bottle => {
                    var disposable = bottle.OnInvincibleAfterDamagedExpired.Subscribe(bottleObject => {
                        HandleCollision(bottleObject);
                        bottleObject.GetComponent<BottleControllerBase>().HandleCollision(GetComponent<Collider2D>());
                    }).AddTo(this);
                    _invincibleAfterDamagedExpiredDisposables.Add(bottle, disposable);
                }).AddTo(this);

            this.OnTriggerExit2DAsObservable()
                .Select(other => other.GetComponent<BottleControllerBase>())
                .Where(bottle => bottle && _invincibleAfterDamagedExpiredDisposables.ContainsKey(bottle))
                .Subscribe(bottle => {
                    _invincibleAfterDamagedExpiredDisposables[bottle].Dispose();
                    _invincibleAfterDamagedExpiredDisposables.Remove(bottle);
                }).AddTo(this);

            GamePlayDirector.Instance.GameEnd.Subscribe(_ => {
                foreach (var disposable in _invincibleAfterDamagedExpiredDisposables) {
                    disposable.Value.Dispose();
                }

                _invincibleAfterDamagedExpiredDisposables.Clear();
            });
        }

        protected virtual void HandleCollision(GameObject other) { }

        public virtual void Initialize(GimmickData gimmickData)
        {
            GimmickType = gimmickData.type;

            foreach (var renderer in GetComponentsInChildren<Renderer>()) {
                renderer.sortingOrder = _gimmickId;
            }

            try {
                _gimmickId = checked((short)(_gimmickId + 1));
            } catch (OverflowException) {
                _gimmickId = short.MinValue;
            }
        }

        /// <summary>
        /// ギミック発動（初期化の後に呼ぶ）
        /// </summary>
        public abstract IEnumerator Trigger();
    }
}
