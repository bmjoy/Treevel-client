using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
    public class NormalHoleWarningController : BulletWarningController
    {
        protected override void Awake()
        {
            base.Awake();
            transform.localScale =
                new Vector2(HoleWarningSize.WIDTH / originalWidth, HoleWarningSize.HEIGHT / originalHeight);
        }

        /// <summary>
        /// 座標を設定する
        /// </summary>
        /// <param name="row"> 出現する行</param>
        /// <param name="column"> 出現する列 </param>
        public void Initialize(int row, int column)
        {
            transform.position =
                new Vector2(TileSize.WIDTH * (column - ((StageSize.COLUMN / 2 + 1))), TileSize.HEIGHT * (StageSize.ROW / 2 + 1 - row));
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
