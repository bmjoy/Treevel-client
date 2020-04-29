using Project.Scripts.GameDatas;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using SpriteGlow;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Project.Scripts.GamePlayScene.Bottle
{
    [RequireComponent(typeof(PostProcessVolume))]
    [RequireComponent(typeof(SpriteGlowEffect))]
    public class NumberBottleController : DynamicBottleController, IBottleSuccessHandler, IEnterTileHandler
    {
        /// <summary>
        /// ボトルのゴールとなるタイル
        /// </summary>
        private GameObject _finalTile;

        /// <summary>
        /// ボトルの初期位置
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// ボトルの目標位置
        /// </summary>
        private int _finalPos;

        /// <summary>
        /// 失敗時のアニメーション
        /// </summary>
        [SerializeField] protected AnimationClip deadAnimation;

        /// <summary>
        /// 自身が壊されたかどうか
        /// </summary>
        protected bool dead = false;

        protected override void Awake()
        {
            base.Awake();
            anim.AddClip(deadAnimation, AnimationClipName.NUMBER_PANEL_DEAD);
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="panelData">ボトルデータ</param>
        public override void Initialize(BottleData panelData)
        {
            Id = panelData.initPos;
            _finalPos = panelData.targetPos;
            var panelSprite = AddressableAssetManager.GetAsset<Sprite>(panelData.panelSprite);
            var targetTileSprite = AddressableAssetManager.GetAsset<Sprite>(panelData.targetTileSprite);
            GetComponent<SpriteRenderer>().sprite = panelSprite;

            base.Initialize(panelData);

            #if UNITY_EDITOR
            name = BottleName.NUMBER_BOTTLE + Id.ToString();
            #endif

            // 目標とするタイルのスプライトを設定
            var finalTile = BoardManager.GetTile(_finalPos);
            finalTile.GetComponent<NormalTileController>().SetSprite(targetTileSprite);
        }

        /// <summary>
        /// 衝突イベントを処理する
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 銃弾との衝突以外は考えない（現状は，ボトル同士での衝突は起こりえない）
            if (!other.gameObject.CompareTag(TagName.BULLET)) return;
            // 銃痕(hole)が出現したフレーム以外では衝突を考えない
            if (other.gameObject.transform.position.z < 0) return;

            // 失敗演出
            anim.Play(AnimationClipName.NUMBER_PANEL_DEAD, PlayMode.StopAll);

            // 自身が破壊された
            dead = true;

            // 失敗状態に移行する
            GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Failure);
        }

        /// <summary>
        /// ゲーム成功時の処理
        /// </summary>
        protected override void OnSucceed()
        {
            base.OnSucceed();
            EndProcess();
        }

        /// <summary>
        /// ゲーム失敗時の処理
        /// </summary>
        protected override void OnFail()
        {
            base.OnFail();
            EndProcess();
        }

        /// <summary>
        /// ゲーム終了時の共通処理
        /// </summary>
        private void EndProcess()
        {
            // 自身が破壊されてない場合には，自身のアニメーションの繰り返しを停止
            if (!dead) {
                anim.wrapMode = WrapMode.Default;
            }
        }

        /// <inheritdoc/>
        public void DoWhenSuccess()
        {
            // ステージの成功判定
            GameObject.FindObjectOfType<GamePlayDirector>().CheckClear();
        }

        /// <inheritdoc/>
        public bool IsSuccess()
        {
            var currPos = BoardManager.GetPanelPos(this);
            return currPos == _finalPos;
        }

        /// <inheritdoc/>
        public void OnEnterTile(GameObject tile)
        {
            if (IsSuccess()) {
                // 最終タイルにいるかどうかで，光らせるかを決める
                GetComponent<SpriteGlowEffect>().enabled = true;

                DoWhenSuccess();
            } else {
                GetComponent<SpriteGlowEffect>().enabled = false;
            }
        }
    }
}
