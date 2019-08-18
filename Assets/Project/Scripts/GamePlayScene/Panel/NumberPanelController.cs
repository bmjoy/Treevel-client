using System;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using SpriteGlow;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Project.Scripts.GamePlayScene.Panel
{
    public class NumberPanelController : DynamicPanelController
    {
        // 最終タイル
        private GameObject _finalTile;

        private int panelNum;

        // パネルが最終タイルにいるかどうかの状態
        [NonSerialized] public bool adapted;

        protected override void Awake()
        {
            base.Awake();
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

        public void Initialize(int panelNum, int initialTileNum, int finalTileNum)
        {
            Initialize(initialTileNum);
            name = PanelName.NUMBER_PANEL + panelNum;
            _finalTile = TileLibrary.GetTile(finalTileNum);
            this.panelNum = panelNum;
        }

        public GameObject GetNumberPanel(int panelNum)
        {
            if (this.panelNum == panelNum) {
                return gameObject;
            }

            return null;
        }

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

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 銃弾との衝突以外は考えない（現状は，パネル同士での衝突は起こりえない）
            if (!other.gameObject.CompareTag(TagName.BULLET)) return;
            gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
            // 失敗状態に移行する
            gamePlayDirector.Dispatch(GamePlayDirector.EGameState.Failure);
        }

        /* SpriteGlowEffect コンポーネントに必要なコンポーネント */
        private void AddPostProcessVolume()
        {
            gameObject.AddComponent<PostProcessVolume>();
            GetComponent<PostProcessVolume>().isGlobal = true;
            var profile = Resources.Load<PostProcessProfile>("PostProcessProfile/GamePlayScene/numberPanelPrefab");
            GetComponent<PostProcessVolume>().profile = profile;
        }

        /* オブジェクトを光らせる */
        private void AddSpriteGlowEffect()
        {
            gameObject.AddComponent<SpriteGlowEffect>();
            GetComponent<SpriteGlowEffect>().GlowColor = new Color32(0, 255, 255, 255);
            GetComponent<SpriteGlowEffect>().GlowBrightness = 3.0f;
            GetComponent<SpriteGlowEffect>().OutlineWidth = 6;
        }
    }
}
