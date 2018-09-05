using JetBrains.Annotations;
using UnityEngine;

namespace Tile
{
    public class TileController : MonoBehaviour
    {
        [CanBeNull] private GameObject rightTile;
        [CanBeNull] private GameObject leftTile;
        [CanBeNull] private GameObject upperTile;
        [CanBeNull] private GameObject lowerTile;

        // Use this for initialization
        void Start ()
        {
        }

        // Update is called once per frame
        void Update ()
        {
        }

        // 自身タイルの上下左右のタイルへの参照を入れる
        public void MakeRelation(GameObject rightTile, GameObject leftTile, GameObject upperTile, GameObject lowerTile)
        {
            this.rightTile = rightTile;
            this.leftTile = leftTile;
            this.upperTile = upperTile;
            this.lowerTile = lowerTile;
        }
    }
}
