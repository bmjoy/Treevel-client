using Project.Scripts.GamePlayScene.Gimmick;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Unity.UNetWeaver;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bottle
{
    public interface IBottleGetDamagedHandler
    {
        /// <summary>
        /// ギミックに攻撃されたときの挙動
        /// </summary>
        /// <param name="gimmick"></param>
        void OnGetDamaged(GameObject gimmick);
    }

    /// <summary>
    /// ライフが1であるボトル用のハンドラ
    /// </summary>
    internal class OneLifeBottleGetDamagedHandler : IBottleGetDamagedHandler
    {
        /// <summary>
        /// ボトルのインスタンス
        /// </summary>
        private readonly AbstractBottleController _bottle;

        internal OneLifeBottleGetDamagedHandler(AbstractBottleController bottle)
        {
            _bottle = bottle;

            // アニメーション初期化
            if (_bottle.GetComponent<Animation>() == null) {
                _bottle.gameObject.AddComponent<Animation>();
            }

            // ボトルが死んだときのアニメーション
            AddressableAssetManager.LoadAsset<AnimationClip>(AnimationClipName.BOTTLE_DEAD).Completed += (handle) => {
                _bottle.GetComponent<Animation>().AddClip(handle.Result, AnimationClipName.BOTTLE_DEAD);
            };
        }

        void IBottleGetDamagedHandler.OnGetDamaged(GameObject gimmick)
        {
            // 失敗演出
            _bottle.GetComponent<Animation>()?.Play(AnimationClipName.BOTTLE_DEAD, PlayMode.StopAll);

            // ボトルを死んだ状態にする
            _bottle.IsDead = true;

            // 失敗原因を保持する
            var gimmickType = gimmick.GetComponent<AbstractGimmickController>().GimmickType;
            GamePlayDirector.Instance.failureReason = gimmickType.GetFailureReason();

            // 失敗状態に移行する
            GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Failure);
        }
    }

    /// <summary>
    /// ライフが2以上あるボトル用のハンドラ
    /// </summary>
    internal class MultiLifeBottleGetDamagedHandler : IBottleGetDamagedHandler
    {
        /// <summary>
        /// ボトルのインスタンス
        /// </summary>
        private readonly AbstractBottleController _bottle;

        /// <summary>
        /// 初期ライフの最小値
        /// </summary>
        private const int _MIN_LIFE = 2;

        /// <summary>
        /// 現在のライフ
        /// </summary>
        private int _currentLife;

        internal MultiLifeBottleGetDamagedHandler(AbstractBottleController bottle, int life)
        {
            _bottle = bottle;

            if (life < _MIN_LIFE) {
                Log.Error($"ライフは{_MIN_LIFE}以上にしてください");
                return;
            }

            _currentLife = life;

            // アニメーション初期化
            if (_bottle.GetComponent<Animation>() == null) {
                _bottle.gameObject.AddComponent<Animation>();
            }

            // ボトルが死んだときのアニメーション
            AddressableAssetManager.LoadAsset<AnimationClip>(AnimationClipName.BOTTLE_DEAD).Completed += (handle) => {
                _bottle.GetComponent<Animation>().AddClip(handle.Result, AnimationClipName.BOTTLE_DEAD);
            };

            // ボトルがギミックに攻撃されたときのアニメーション
            AddressableAssetManager.LoadAsset<AnimationClip>(AnimationClipName.BOTTLE_GET_ATTACKED).Completed += (handle) => {
                _bottle.GetComponent<Animation>().AddClip(handle.Result, AnimationClipName.BOTTLE_GET_ATTACKED);
            };
        }

        void IBottleGetDamagedHandler.OnGetDamaged(GameObject gimmick)
        {
            var anim = _bottle.GetComponent<Animation>();

            // 現在のライフを一つ削る
            _currentLife--;

            if (_currentLife < 0) {
                Debug.LogError("_currentLife が負の値になっている");

            } else if (_currentLife == 0) {
                // 失敗演出
                anim.Play(AnimationClipName.BOTTLE_DEAD, PlayMode.StopAll);

                // 自身が破壊された
                _bottle.IsDead = true;

                // 失敗原因を保持する
                var gimmickType = gimmick.GetComponent<AbstractGimmickController>().GimmickType;
                GamePlayDirector.Instance.failureReason = gimmickType.GetFailureReason();

                // 失敗状態に移行する
                GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Failure);

            } else if (_currentLife == 1) {
                // ループさせて危機感っぽい
                anim.wrapMode = WrapMode.Loop;
                anim.Play(AnimationClipName.BOTTLE_GET_ATTACKED, PlayMode.StopAll);

            } else {
                anim.Play(AnimationClipName.BOTTLE_GET_ATTACKED, PlayMode.StopAll);
            }
        }
    }
}
