using UnityEngine;
using System;
using System.Collections;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;

namespace Project.Scripts.GamePlayScene.Bullet
{
    public class NormalHoleController : BulletController
    {
        /// <summary>
        /// 表示フレーム数
        /// </summary>
        private const int _HOLE_DISPLAYED_FRAMES = 25;



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
            // 銃痕のz座標が0のときのみ衝突判定を行う
            // 銃痕の出現直後の1フレームで奥行き方向に移動する分を加算しておく
            transform.position = new Vector3(holeWarningPosition.x, holeWarningPosition.y, speed);
        }

        protected void Update()
        {
            // 指定のフレーム以上経過していたら銃弾を消す
            if (transform.position.z < (-1) * _HOLE_DISPLAYED_FRAMES * speed && gamePlayDirector.state == GamePlayDirector.EGameState.Playing) Destroy(gameObject);
        }

        protected void FixedUpdate()
        {
            // 奥行き方向に移動させる(見た目の変化はない)
            transform.Translate(new Vector3(0, 0, -1) * speed, Space.World);
        }

        protected override void OnFail()
        {
        }

        /// <summary>
        /// NumberPanelとの当たり判定
        /// </summary>
        /// <returns></returns>
        [Obsolete("このメソッドは使われておりません")]
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
                    yield return new WaitForSeconds(_HOLE_DISPLAYED_FRAMES / 50);
                    Destroy(gameObject);
                }
            } else {
                // 銃弾の出現するタイル上にパネルが存在しない
                // タイルとパネルの間のレイヤー(Hole)に描画する
                gameObject.GetComponent<Renderer>().sortingLayerName = SortingLayerName.HOLE;
                yield return new WaitForSeconds(_HOLE_DISPLAYED_FRAMES / 50);
                Destroy(gameObject);
            }
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            // 数字パネルとの衝突以外は考えない
            if (!other.gameObject.CompareTag(TagName.NUMBER_PANEL)) return;
            // 銃痕(hole)が出現したフレーム以外では衝突を考えない
            if (transform.position.z < 0) return;

            // 衝突したオブジェクトは赤色に変える
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
