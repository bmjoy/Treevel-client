using JetBrains.Annotations;
using UnityEngine;

namespace Tile
{
    public class TileController : MonoBehaviour
    {
        [CanBeNull] public GameObject rightTile;
        [CanBeNull] public GameObject leftTile;
        [CanBeNull] public GameObject upperTile;
        [CanBeNull] public GameObject lowerTile;

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
