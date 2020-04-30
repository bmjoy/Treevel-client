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
    public class NormalBottleController : DynamicBottleController, IBottleSuccessHandler, IEnterTileHandler
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

        protected override void Awake()
        {
            base.Awake();
            anim.AddClip(deadAnimation, AnimationClipName.NORMAL_BOTTLE_DEAD);

            _getDamagedHandler = new NormalGetDamagedHandler(this);
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="bottleData">ボトルデータ</param>
        public override void Initialize(BottleData bottleData)
        {
            Id = bottleData.initPos;
            _finalPos = bottleData.targetPos;
            var bottleSprite = AddressableAssetManager.GetAsset<Sprite>(bottleData.bottleSprite);
            var targetTileSprite = AddressableAssetManager.GetAsset<Sprite>(bottleData.targetTileSprite);
            GetComponent<SpriteRenderer>().sprite = bottleSprite;

            base.Initialize(bottleData);

            #if UNITY_EDITOR
            name = BottleName.NUMBER_BOTTLE + Id.ToString();
            #endif

            // 目標とするタイルのスプライトを設定
            var finalTile = BoardManager.GetTile(_finalPos);
            finalTile.GetComponent<NormalTileController>().SetSprite(targetTileSprite);
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
            if (!IsDead) {
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
            var currPos = BoardManager.GetBottlePos(this);
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
