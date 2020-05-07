﻿using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet.Controllers
{
    public class NormalHoleController : BulletController
    {
        /// <summary>
        /// 表示フレーム数
        /// </summary>
        private const int _HOLE_DISPLAYED_FRAMES = 100;

        /// <summary>
        /// 出現する行
        /// </summary>
        protected int row;

        /// <summary>
        /// 出現する列
        /// </summary>
        protected int column;

        /// <summary>
        /// 各フレームの透明度の減少量
        /// </summary>
        private float _alphaChange = 1.0f / _HOLE_DISPLAYED_FRAMES;

        protected override void Awake()
        {
            base.Awake();
            // タイルとボトルの間のレイヤー(Hole)に描画する
            gameObject.GetComponent<Renderer>().sortingLayerName = SortingLayerName.HOLE;
            transform.localScale = new Vector2(HoleSize.WIDTH / originalWidth, HoleSize.HEIGHT / originalHeight) * LOCAL_SCALE;
        }

        /// <summary>
        /// 出現する行、出現する列、銃痕の座標を設定する
        /// </summary>
        /// <param name="row"> 出現する行 </param>
        /// <param name="column"> 出現する列 </param>
        /// <param name="holeWarningPosition"> 警告の座標 </param>
        public void Initialize(int row, int column, Vector2 holeWarningPosition)
        {
            this.row = row;
            this.column = column;
            // 銃痕のz座標が0のときのみ衝突判定を行う
            // 銃痕の出現直後の1フレームで奥行き方向に移動する分を加算しておく
            transform.position = new Vector3(holeWarningPosition.x, holeWarningPosition.y, speed);
            GetComponent<SpriteRenderer>().enabled = true;
        }

        protected void Update()
        {
            // 指定のフレーム以上経過していたら銃弾を消す
            if (transform.position.z < (-1) * _HOLE_DISPLAYED_FRAMES * speed && gamePlayDirector.State == GamePlayDirector.EGameState.Playing) Destroy(gameObject);
        }

        protected void FixedUpdate()
        {
            // 奥行き方向に移動させる(見た目の変化はない)
            transform.Translate(Vector3.back * speed, Space.World);
            // 透明度をあげてだんだんと見えなくする
            gameObject.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, _alphaChange);
        }

        protected override void OnFail()
        {
            _alphaChange = 0.0f;
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            // 銃痕(hole)が出現したフレーム以外では衝突を考えない
            if (transform.position.z < 0) return;

            // ボトルとの衝突
            var bottle = other.GetComponent<AbstractBottleController>();
            if (bottle != null && bottle.IsAttackable) {
                // 数字ボトルとの衝突
                // 衝突したオブジェクトは赤色に変える
                gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            }

            // ボトルより手前のレイヤー(BULLET)に描画する
            gameObject.GetComponent<Renderer>().sortingLayerName = SortingLayerName.BULLET;

            // holeを衝突したボトルに追従させる
            gameObject.transform.SetParent(other.transform);
        }
    }
}
