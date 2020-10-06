using Project.Scripts.GamePlayScene.Gimmick;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bottle
{
    [RequireComponent(typeof(Animator))]
    public class LifeEffectController : MonoBehaviour
    {
        private DynamicBottleController _bottleController;

        /// <summary>
        /// 自身のライフが0になったかどうか
        /// </summary>
        private bool _isDead;

        /// <summary>
        /// 自身のライフ
        /// </summary>
        private int _life;

        private Animation _bottleAnimation;

        public void Initialize(DynamicBottleController bottleController, int life)
        {
            transform.parent = bottleController.transform;
            transform.localPosition = Vector3.zero;
            _bottleController = bottleController;
            _bottleAnimation = bottleController.GetComponent<Animation>();
            // ボトルが死んだときのアニメーション
            AddressableAssetManager.LoadAsset<AnimationClip>(AnimationClipName.BOTTLE_DEAD).Completed += (handle) => {
                _bottleAnimation.AddClip(handle.Result, AnimationClipName.BOTTLE_DEAD);
            };

            if (life > 1) {
                // ボトルがギミックに攻撃されたときのアニメーション
                AddressableAssetManager.LoadAsset<AnimationClip>(AnimationClipName.BOTTLE_GET_ATTACKED).Completed += (handle) => {
                    _bottleAnimation.AddClip(handle.Result, AnimationClipName.BOTTLE_GET_ATTACKED);
                };
            }

            // イベントに処理を登録する
            _bottleController.HandleOnGetDamaged += HandleOnGetDamaged;
            _bottleController.HandleOnEndProcess += HandleOnEndProcess;

            _life = life;
        }

        /// <summary>
        /// ギミックに攻撃されたときの処理
        /// </summary>
        /// <param name="gimmick"></param>
        private void HandleOnGetDamaged(GameObject gimmick)
        {
            _life--;
            if (_life < 0) {
                Debug.LogError("_currentLife が負の値になっている");
            } else if (_life == 0) {
                // 失敗演出
                _bottleAnimation.Play(AnimationClipName.BOTTLE_DEAD, PlayMode.StopAll);

                // ボトルを死んだ状態にする
                _isDead = true;

                // 失敗原因を保持する
                var controller = gimmick.GetComponent<AbstractGimmickController>();
                if (controller == null)
                    controller = gimmick.GetComponentInParent<AbstractGimmickController>();

                var gimmickType = controller.GimmickType;
                GamePlayDirector.Instance.failureReason = gimmickType.GetFailureReason();

                // 失敗状態に移行する
                GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Failure);
            } else if (_life == 1) {
                // ループさせて危機感っぽい
                _bottleAnimation.wrapMode = WrapMode.Loop;
                _bottleAnimation.Play(AnimationClipName.BOTTLE_GET_ATTACKED, PlayMode.StopAll);
            } else {
                _bottleAnimation.Play(AnimationClipName.BOTTLE_GET_ATTACKED, PlayMode.StopAll);
            }
        }

        /// <summary>
        /// ゲーム終了時の処理
        /// </summary>
        private void HandleOnEndProcess()
        {
            // 自身が破壊されていない場合はアニメーションを止める
            if (!_isDead) {
                _bottleAnimation.wrapMode = WrapMode.Default;
            }
        }
    }
}
