using Project.Scripts.Utils.Attribute;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
    public class WarpTileController : NormalTileController
    {
        // 相方のwarpTile
        [SerializeField, NonEditable] private GameObject _pairTile;

        public void Initialize(Vector2 position, int tileNum, GameObject pairTile)
        {
            base.Initialize(position, tileNum);
            name = TileName.WARP_TILE;
            this._pairTile = pairTile;
        }

        public override void HandlePanel(GameObject panel)
        {
            if (_pairTile.transform.childCount == 0) {
                panel.transform.parent = _pairTile.transform;
                panel.transform.position = _pairTile.transform.position;
            }
        }
    }
}
