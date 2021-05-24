using Treevel.Common.Components;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Common.Managers;
using Treevel.Modules.GamePlayScene.Gimmick;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    public class LifeAttributeController : BottleAttributeControllerBase
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

        private static readonly int _ANIMATOR_PARAM_BOOL_LAST_LIFE = Animator.StringToHash("LifeLast");
        private Animator _bottleAnimator;
        private static readonly int _ANIMATOR_PARAM_TRIGGER_ATTACKED = Animator.StringToHash("LifeAttacked");
        public static readonly int ANIMATOR_PARAM_TRIGGER_DEAD = Animator.StringToHash("LifeDead");
        private static readonly int _ANIMATOR_PARAM_FLOAT_SPEED = Animator.StringToHash("LifeSpeed");
        private static readonly int _ANIMATOR_PARAM_BOOL_ATTACKED_LOOP = Animator.StringToHash("LifeAttackedLoop");

        protected override void Awake()
        {
            base.Awake();
            _lifeSpriteRenderer = _lifeObject.GetComponent<SpriteRenderer>();
            // 描画順序の設定
            spriteRenderer.sortingOrder = EBottleAttributeType.Life.GetOrderInLayer();
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

            if (_life < 1 || MAX_LIFE < _life) UIManager.Instance.ShowErrorMessageAsync(EErrorCode.InvalidLifeValue);

            if (_life == 1) {
                // lifeの初期値が1ならハートを表示しない
                _lifeObject.SetActive(false);
            } else {
                // ゲーム開始時に描画する
                GamePlayDirector.Instance.StagePrepared.Subscribe(_ => {
                    spriteRenderer.enabled = true;
                    _lifeSpriteRenderer.enabled = true;
                }).AddTo(compositeDisposableOnGameEnd, this);
                GamePlayDirector.Instance.GameStart.Subscribe(_ => animator.enabled = true).AddTo(this);
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
                    SoundManager.Instance.PlaySE(ESEKey.Bottle_Destroy);

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
                    animator.SetBool(_ANIMATOR_PARAM_BOOL_LAST_LIFE, true);
                    // 演出をループさせる
                    _bottleAnimator.SetBool(_ANIMATOR_PARAM_BOOL_ATTACKED_LOOP, true);
                    _bottleAnimator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_ATTACKED);
                    SoundManager.Instance.PlaySE(ESEKey.Bottle_Break);
                } else {
                    _bottleAnimator.SetTrigger(_ANIMATOR_PARAM_TRIGGER_ATTACKED);
                    SoundManager.Instance.PlaySE(ESEKey.Bottle_Break);
                }
            }).AddTo(this);
            GamePlayDirector.Instance.GameEnd.Where(_ => !_isDead)
                .Subscribe(_ => {
                    // 自身が破壊されていない場合はアニメーションを止める
                    animator.enabled = false;
                    _bottleAnimator.SetFloat(_ANIMATOR_PARAM_FLOAT_SPEED, 0f);
                }).AddTo(this);
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
