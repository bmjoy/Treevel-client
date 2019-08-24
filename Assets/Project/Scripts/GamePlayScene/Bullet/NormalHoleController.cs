using UnityEngine;
using System.Collections;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;

namespace Project.Scripts.GamePlayScene.Bullet
{
    public class NormalHoleController : BulletController
    {
        /// <summary>
        /// 表示時間
        /// </summary>
        private const float _HOLE_DISPLAYED_TIME = 0.50f;

        /// <summary>
        /// 出現する行
        /// </summary>
        protected int row;
        /// <summary>
        /// 出現する列
        /// </summary>
        protected int column;

        protected override void Awake()
        {
            base.Awake();
            transform.localScale = new Vector2(HoleSize.WIDTH / originalWidth, HoleSize.HEIGHT / originalHeight) *
            LOCAL_SCALE;
        }

        /// <summary>
        /// 出現する行、出現する列、銃痕の座標を設定する
        /// </summary>
        /// <param name="row"> 出現する行 </param>
        /// <param name="column"> 出現する列 </param>
        /// <param name="holeWarningPosition"> 警告の座標 </param>
        public virtual void Initialize(int row, int column, Vector2 holeWarningPosition)
        {
            this.row = row;
            this.column = column;
            transform.position = holeWarningPosition;
        }

        protected override void OnFail()
        {
        }

        /// <summary>
        /// NumberPanelとの当たり判定
        /// </summary>
        /// <returns></returns>
        public IEnumerator CollisionCheck()
        {
            var gamePlayDirector = FindObjectOfType<GamePlayDirector>();
            var tile = TileLibrary.GetTile(row, column);
            if (tile.transform.childCount != 0) {
                // 銃弾の出現するタイル上にパネルが存在する
                var panel = tile.transform.GetChild(0);
                if (panel.CompareTag(TagName.NUMBER_PANEL)) {
                    // 銃弾の出現するタイル上に数字パネルが存在する
                    // 当たり判定が起きる
                    gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                    gamePlayDirector.Dispatch(GamePlayDirector.EGameState.Failure);
                } else {
                    // 銃弾の出現するタイル上に数字パネル以外のタイルが存在する
                    // 当たり判定は起きない
                    yield return new WaitForSeconds(_HOLE_DISPLAYED_TIME);
                    Destroy(gameObject);
                }
            } else {
                // 銃弾の出現するタイル上にパネルが存在しない
                // タイルとパネルの間のレイヤー(Hole)に描画する
                gameObject.GetComponent<Renderer>().sortingLayerName = SortingLayerName.HOLE;
                yield return new WaitForSeconds(_HOLE_DISPLAYED_TIME);
                Destroy(gameObject);
            }
        }
    }
}
