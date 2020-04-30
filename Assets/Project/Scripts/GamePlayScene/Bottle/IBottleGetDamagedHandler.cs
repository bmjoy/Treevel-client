using Project.Scripts.Utils.Definitions;
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
    /// 撃たれたら即ゲーム終了のボトル用ハンドラー
    /// </summary>
    internal class NormalGetDamagedHandler : IBottleGetDamagedHandler
    {
        /// <summary>
        /// ボトルのインスタンス
        /// </summary>
        private readonly AbstractBottleController _bottle;

        internal NormalGetDamagedHandler(AbstractBottleController bottle)
        {
            _bottle = bottle;
        }

        void IBottleGetDamagedHandler.OnGetDamaged(GameObject gimmick)
        {
            // 失敗演出
            _bottle.GetComponent<Animation>()?.Play(AnimationClipName.NORMAL_BOTTLE_DEAD, PlayMode.StopAll);

            // 破壊フラグ
            _bottle.IsDead = true;

            // 失敗状態に移行する
            GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Failure);
        }
    }

    /// <summary>
    /// ライフ付きボトル用ハンドラー
    /// </summary>
    internal class LifeBottleGetDamagedHandler : IBottleGetDamagedHandler
    {
        /// <summary>
        /// ボトルのインスタンス
        /// </summary>
        private readonly AbstractBottleController _bottle;

        /// <summary>
        /// ボトルが銃弾の攻撃に耐えられる回数
        /// </summary>
        [Min(2)] private int _maxHp;

        /// <summary>
        /// 残ってる回数
        /// </summary>
        private int _currentHp;

        internal LifeBottleGetDamagedHandler(AbstractBottleController bottle, int maxHp)
        {
            _bottle = bottle;
            _maxHp = _currentHp = maxHp;
        }

        void IBottleGetDamagedHandler.OnGetDamaged(GameObject gimmick)
        {
            var anim = _bottle.GetComponent<Animation>();
            --_currentHp;
            if (_currentHp <= 0) {
                // 失敗演出
                anim.Play(AnimationClipName.NORMAL_BOTTLE_DEAD, PlayMode.StopAll);

                // 自身が破壊された
                _bottle.IsDead = true;

                // 失敗状態に移行する
                GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Failure);
            } else if (_currentHp == 1) {
                // ループさせて危機感っぽい
                anim.wrapMode = WrapMode.Loop;
                anim.Play(AnimationClipName.LIFE_BOTTLE_GET_ATTACKED, PlayMode.StopAll);
            } else {
                anim.Play(AnimationClipName.LIFE_BOTTLE_GET_ATTACKED, PlayMode.StopAll);
            }
        }
    }
}
