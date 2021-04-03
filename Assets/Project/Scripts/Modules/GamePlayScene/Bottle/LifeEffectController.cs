using Treevel.Common.Components;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Gimmick;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(Animator))]
    public class LifeEffectController : GameObjectControllerBase
    {
        public const int MAX_LIFE = 3;

        private DynamicBottleController _bottleController;

        /// <summary>
        /// 付与されるGoalBottleの色
        /// </summary>
        private string _goalBottleAddress;

        private SpriteRendererUnifier _bottleSpriteRendererUnifier;

        /// <summary>
        /// 自身のライフが0になったかどうか
        /// </summary>
        private bool _isDead;

        /// <summary>
        /// 自身のライフ
        /// </summary>
        private int _life;

        /// <summary>
        /// 残りライフの数字オブジェクト
        /// </summary>
        [SerializeField] private GameObject _lifeObject;

        private SpriteRenderer _lifeSpriteRenderer;

        /// <summary>
        /// Life画像をBottleの中心からどれくらいずらすか
        /// </summary>
        private static readonly Vector3 _POSITION = new Vector3(-75f, 85f);

        private Animator _animator;
        private Animator _bottleAnimator;
        private static readonly int _ANIMATOR_PARAM_TRIGGER_ATTACKED = Animator.StringToHash("LifeAttacked");
        public static readonly int ANIMATOR_PARAM_TRIGGER_DEAD = Animator.StringToHash("LifeDead");
        private static readonly int _ANIMATOR_PARAM_FLOAT_SPEED = Animator.StringToHash("LifeSpeed");
        private static readonly int _ANIMATOR_PARAM_BOOL_ATTACKED_LOOP = Animator.StringToHash("LifeAttackedLoop");

        private void Awake()
        {
            // 現状、LifeEffectについてのアニメーション演出はない
            _animator = GetComponent<Animator>();
            _lifeSpriteRenderer = _lifeObject.GetComponent<SpriteRenderer>();
        }

        public void Initialize(GoalBottleController bottleController, int life)
        {
            _goalBottleAddress = bottleController.GoalColor.GetBottleAddress();
            _bottleSpriteRendererUnifier = bottleController.spriteRendererUnifier;
            Initialize((DynamicBottleController)bottleController, life);
        }

        public void Initialize(DynamicBottleController bottleController, int life)
        {
            _life = life;
            _bottleController = bottleController;
            _bottleAnimator = bottleController.GetComponent<Animator>();
            transform.parent = bottleController.transform;

            if (_life < 1 || MAX_LIFE < _life) UIManager.Instance.ShowErrorMessage(EErrorCode.InvalidLifeValue);

            if (_life == 1) {
                // lifeの初期値が1ならハートを表示しない
                GetComponent<SpriteRenderer>().enabled = false;
                _lifeObject.SetActive(false);
            } else {
                if (_life == 2) {
                    // lifeの初期値が2ならボトル画像にヒビを入れる
                    SetCrackSprite(_life);
                }

                // Bottleの左上に配置する
                transform.localPosition = _POSITION;
                SetLifeValueSprite();
            }


            // イベントに処理を登録する
            _bottleController.GetDamaged.Subscribe(gimmick => {
                _life--;
                SetLifeValueSprite();
                SetCrackSprite(_life);
                if (_life < 0) {
                    Debug.LogError("_currentLife が負の値になっている");
                } else if (_life == 0) {
                    // 失敗演出
                    GetComponent<SpriteRenderer>().enabled = false;
                    _lifeObject.SetActive(false);

                    _bottleAnimator.SetTrigger(ANIMATOR_PARAM_TRIGGER_DEAD);

                    // ボトルを死んだ状態にする
                    _isDead = true;

                    // 失敗原因を保持する
                    var controller = gimmick.GetComponent<GimmickControllerBase>();
                    if (controller == null) controller = gimmick.GetComponentInParent<GimmickControllerBase>();

                    var gimmickType = controller.GimmickType;
                    GamePlayDirector.Instance.failureReason = gimmickType.GetFailureReason();

                    // 失敗状態に移行する
                    GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Failure);
                } else if (_life == 1) {
                    // 演出をループさせる
                    _bottleAnimator.SetBool(_ANIMATOR_PARAM_BOOL_ATTACKED_LOOP, true);
                    _bottleAnimator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_ATTACKED);
                } else {
                    _bottleAnimator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_ATTACKED);
                }
            }).AddTo(this);
            GamePlayDirector.Instance.GameEnd.Where(_ => !_isDead)
                .Subscribe(_ => {
                    // 自身が破壊されていない場合はアニメーションを止める
                    _animator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
                    _bottleAnimator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
                }).AddTo(this);

            // 描画順序の設定
            GetComponent<SpriteRenderer>().sortingOrder = EBottleEffectType.Life.GetOrderInLayer();
        }

        // 数字画像を設定する
        private void SetLifeValueSprite()
        {
            if (1 <= _life && _life <= MAX_LIFE) _lifeSpriteRenderer.sprite = AddressableAssetManager.GetAsset<Sprite>(Constants.Address.LIFE_VALUE_SPRITE_PREFIX + _life);
        }

        private void SetCrackSprite(int life)
        {
            // lifeが1, 2に変わる時のみ画像を替える
            if (life == 1 || life == 2) _bottleSpriteRendererUnifier.SetSprite(AddressableAssetManager.GetAsset<Sprite>(_goalBottleAddress + Constants.Address.LIFE_CRACK_SPRITE_INFIX + life));
        }
    }
}
