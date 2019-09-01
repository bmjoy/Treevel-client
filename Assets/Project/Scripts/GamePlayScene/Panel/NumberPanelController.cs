using System;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using SpriteGlow;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Project.Scripts.GamePlayScene.Panel
{
    [RequireComponent(typeof(Animation))]
    public class NumberPanelController : DynamicPanelController
    {
        /// <summary>
        /// パネルのゴールとなるタイル
        /// </summary>
        private GameObject _finalTile;

        /// <summary>
        /// パネルの番号
        /// </summary>
        private int panelNum;

        /// <summary>
        /// パネルがゴールタイルにいるかどうか
        /// </summary>
        [NonSerialized] public bool adapted;
        /// <summary>
        /// 失敗時のアニメーション
        /// </summary>
        [SerializeField] protected AnimationClip _deadAnimation;

        protected Animation _anim;

        protected override void Awake()
        {
            base.Awake();
            _anim = GetComponent<Animation>();
            _anim.AddClip(_deadAnimation, _deadAnimation.name);

            // 光らせるためのコンポーネントをアタッチ
            AddPostProcessVolume();
            AddSpriteGlowEffect();
        }

        protected override void Start()
        {
            base.Start();
            // 初期状態で最終タイルにいるかどうかの状態を変える
            adapted = transform.parent.gameObject == _finalTile;
            // 最終タイルにいるかどうかで，光らせるかを決める
            GetComponent<SpriteGlowEffect>().enabled = adapted;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="panelNum"> パネルの番号 </param>
        /// <param name="initialTileNum"> 最初に配置するタイルの番号 </param>
        /// <param name="finalTileNum"> パネルのゴールタイル </param>
        public void Initialize(int panelNum, int initialTileNum, int finalTileNum)
        {
            Initialize(initialTileNum);
            name = PanelName.NUMBER_PANEL + panelNum;
            _finalTile = TileLibrary.GetTile(finalTileNum);
            this.panelNum = panelNum;
        }

        /// <summary>
        /// パネルの番号と一致していたら自身を返す
        /// </summary>
        /// <param name="panelNum"> 取得したいパネルの番号 </param>
        public GameObject GetNumberPanel(int panelNum)
        {
            if (this.panelNum == panelNum) {
                return gameObject;
            }

            return null;
        }

        /// <inheritdoc />
        protected override void UpdateTile(GameObject targetTile)
        {
            base.UpdateTile(targetTile);

            // 最終タイルにいるかどうかで状態を変える
            adapted = transform.parent.gameObject == _finalTile;
            // 最終タイルにいるかどうかで，光らせるかを決める
            GetComponent<SpriteGlowEffect>().enabled = adapted;
            // adapted が true になっていれば (必要条件) 成功判定をする
            if (adapted) gamePlayDirector.CheckClear();
        }

        /// <summary>
        /// 衝突イベントを処理する
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 銃弾との衝突以外は考えない（現状は，パネル同士での衝突は起こりえない）
            if (!other.gameObject.CompareTag(TagName.BULLET)) return;
            // 銃痕(hole)が出現したフレーム以外では衝突を考えない
            if (other.gameObject.transform.position.z < 0) return;

            // 失敗演出
            _anim.Play(_deadAnimation.name, PlayMode.StopAll);

            // 失敗状態に移行する
            gamePlayDirector.Dispatch(GamePlayDirector.EGameState.Failure);
        }

        /// <summary>
        /// PostProcessVolume のアタッチ
        /// </summary>
        private void AddPostProcessVolume()
        {
            gameObject.AddComponent<PostProcessVolume>();
            GetComponent<PostProcessVolume>().isGlobal = true;
            var profile = Resources.Load<PostProcessProfile>("PostProcessProfile/GamePlayScene/numberPanelPrefab");
            GetComponent<PostProcessVolume>().profile = profile;
        }

        /// <summary>
        /// SpriteGlowEffect のアタッチ
        /// </summary>
        private void AddSpriteGlowEffect()
        {
            gameObject.AddComponent<SpriteGlowEffect>();
            GetComponent<SpriteGlowEffect>().GlowColor = new Color32(0, 255, 255, 255);
            GetComponent<SpriteGlowEffect>().GlowBrightness = 3.0f;
            GetComponent<SpriteGlowEffect>().OutlineWidth = 6;
        }
    }
}
