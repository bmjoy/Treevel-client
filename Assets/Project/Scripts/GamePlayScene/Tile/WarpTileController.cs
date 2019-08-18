using Project.Scripts.Utils.Attributes;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
    public class WarpTileController : NormalTileController
    {
        // 相方のWarpTile
        [SerializeField, NonEditable] private GameObject _pairTile;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="position"> 座標 </param>
        /// <param name="tileNum"> タイルの番号 </param>
        /// <param name="pairTile"> 相方の WarpTile </param>
        public void Initialize(Vector2 position, int tileNum, GameObject pairTile)
        {
            base.Initialize(position, tileNum);
            name = TileName.WARP_TILE;
            this._pairTile = pairTile;
        }

        /// <inheritdoc />
        public override void HandlePanel(GameObject panel)
        {
            if (_pairTile.transform.childCount == 0) {
                panel.transform.parent = _pairTile.transform;
                panel.transform.position = _pairTile.transform.position;
            }
        }
    }
}
