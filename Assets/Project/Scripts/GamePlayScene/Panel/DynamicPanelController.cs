using System;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils.Definitions;
using TouchScript.Gestures;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Panel
{
    [RequireComponent(typeof(Animation))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(FlickGesture))]
    public class DynamicPanelController : PanelController
    {
        protected GamePlayDirector gamePlayDirector;

        protected Animation _anim;

        /// <summary>
        /// ワープタイルでワープする時のアニメーション
        /// </summary>
        [SerializeField] protected AnimationClip warpAnimation;
        /// <summary>
        /// ワープタイルでワープした後のアニメーション
        /// </summary>
        [SerializeField] protected AnimationClip warpReverseAnimation;

        protected override void Awake()
        {
            base.Awake();
            name = PanelName.DYNAMIC_DUMMY_PANEL;

            // FlickGesture の設定
            GetComponent<FlickGesture>().MinDistance = 0.2f;
            GetComponent<FlickGesture>().FlickTime = 0.2f;

            // アニメーションの追加
            _anim = GetComponent<Animation>();
            _anim.AddClip(warpAnimation, AnimationClipName.PANEL_WARP);
            _anim.AddClip(warpReverseAnimation, AnimationClipName.PANEL_WARP_REVERSE);
        }

        protected virtual void Start()
        {
            gamePlayDirector = FindObjectOfType<GamePlayDirector>();
        }

        private void OnEnable()
        {
            GetComponent<FlickGesture>().Flicked += HandleFlick;
            GamePlayDirector.OnSucceed += OnSucceed;
            GamePlayDirector.OnFail += OnFail;
        }

        private void OnDisable()
        {
            GetComponent<FlickGesture>().Flicked -= HandleFlick;
            GamePlayDirector.OnSucceed -= OnSucceed;
            GamePlayDirector.OnFail -= OnFail;
        }

        /// <summary>
        /// フリックイベントを処理する
        /// </summary>
        private void HandleFlick(object sender, EventArgs e)
        {
            var gesture = sender as FlickGesture;

            if (gesture.State != FlickGesture.GestureState.Recognized) return;

            // 親タイルオブジェクトのスクリプトを取得
            var parentTile = transform.parent.gameObject.GetComponent<NormalTileController>();
            // フリック方向
            var x = gesture.ScreenFlickVector.x;
            var y = gesture.ScreenFlickVector.y;

            // 方向検知に加えて，上下と左右の変化量を比べることで，検知精度をあげる
            if (x > 0 && Math.Abs(x) >= Math.Abs(y)) {
                // 右
                var rightTile = parentTile._rightTile;
                UpdateTile(rightTile);
            } else if (x < 0 && Math.Abs(x) >= Math.Abs(y)) {
                // 左
                var leftTile = parentTile._leftTile;
                UpdateTile(leftTile);
            } else if (y > 0 && Math.Abs(y) >= Math.Abs(x)) {
                // 上
                var upperTile = parentTile._upperTile;
                UpdateTile(upperTile);
            } else if (y < 0 && Math.Abs(y) >= Math.Abs(x)) {
                // 下
                var lowerTile = parentTile._lowerTile;
                UpdateTile(lowerTile);
            }
        }

        /// <summary>
        /// パネルの位置を更新する
        /// </summary>
        /// <param name="targetTile"> パネルの移動先となるタイル </param>
        protected virtual void UpdateTile(GameObject targetTile)
        {
            // 移動先のタイルのスクリプト
            var targetScript = targetTile.GetComponent<NormalTileController>();
            // 移動先にタイルがなければ何もしない
            if (targetTile == null) return;
            // 移動先のタイルに子パネルがあれば何もしない
            if (targetScript.hasPanel) return;
            // 親タイルの更新
            transform.parent.GetComponent<NormalTileController>().LeavePanel(gameObject);
            transform.parent = targetTile.transform;
            // 親タイルへ移動
            transform.position = transform.parent.position;
            // 親タイルの副作用
            targetTile.GetComponent<NormalTileController>().HandlePanel(gameObject);
        }

        /// <summary>
        /// ゲーム成功時の処理
        /// </summary>
        protected virtual void OnSucceed()
        {
            GetComponent<FlickGesture>().Flicked -= HandleFlick;
            // アニメーションを止める
            _anim[AnimationClipName.PANEL_WARP].speed = 0.0f;
            _anim[AnimationClipName.PANEL_WARP_REVERSE].speed = 0.0f;
        }

        /// <summary>
        /// ゲーム失敗時の処理
        /// </summary>
        protected virtual void OnFail()
        {
            GetComponent<FlickGesture>().Flicked -= HandleFlick;
            _anim[AnimationClipName.PANEL_WARP].speed = 0.0f;
            _anim[AnimationClipName.PANEL_WARP_REVERSE].speed = 0.0f;
        }
    }
}
