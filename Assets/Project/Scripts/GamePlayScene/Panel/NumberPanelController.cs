using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using SpriteGlow;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Project.Scripts.GamePlayScene.Panel
{
    [RequireComponent(typeof(PostProcessVolume))]
    [RequireComponent(typeof(SpriteGlowEffect))]
    public class NumberPanelController : DynamicPanelController
    {
        /// <summary>
        /// パネルのゴールとなるタイル
        /// </summary>
        private GameObject _finalTile;

        /// <summary>
        /// パネルの番号
        /// </summary>
        private int _panelNum;

        /// <summary>
        /// パネルがゴールタイルにいるかどうか
        /// </summary>
        public bool Adapted
        {
            get;
            private set;
        }

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
        /// <param name="panelNum"> パネルの番号 </param>
        /// <param name="initialTileNum"> 最初に配置するタイルの番号 </param>
        /// <param name="finalTileNum"> パネルのゴールタイル </param>
        public void Initialize(int panelNum, int initialTileNum, int finalTileNum)
        {
            Initialize(initialTileNum);
            name = PanelName.NUMBER_PANEL + panelNum;
            _finalTile = TileLibrary.GetTile(finalTileNum);
            _finalTile.GetComponent<NormalTileController>().SetSprite(panelNum);
            _panelNum = panelNum;

            // 初期状態で最終タイルにいるかどうかの状態を変える
            Adapted = transform.parent.gameObject == _finalTile;
            // 最終タイルにいるかどうかで，光らせるかを決める
            GetComponent<SpriteGlowEffect>().enabled = Adapted;
        }

        /// <summary>
        /// パネルの番号と一致していたら自身を返す
        /// </summary>
        /// <param name="panelNum"> 取得したいパネルの番号 </param>
        public GameObject GetNumberPanel(int panelNum)
        {
            return _panelNum == panelNum ? gameObject : null;
        }

        /// <inheritdoc />
        protected override void UpdateTile(GameObject targetTile)
        {
            base.UpdateTile(targetTile);

            // 最終タイルにいるかどうかで状態を変える
            Adapted = transform.parent.gameObject == _finalTile;
            // 最終タイルにいるかどうかで，光らせるかを決める
            GetComponent<SpriteGlowEffect>().enabled = Adapted;
            // adapted が true になっていれば (必要条件) 成功判定をする
            if (Adapted) gamePlayDirector.CheckClear();
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
            anim.Play(AnimationClipName.NUMBER_PANEL_DEAD, PlayMode.StopAll);

            // 自身が破壊された
            dead = true;

            // 失敗状態に移行する
            gamePlayDirector.Dispatch(GamePlayDirector.EGameState.Failure);
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
    }
}
