using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GameDatas
{
    [System.Serializable]
    public class TileData
    {
        public ETileType type;
        [Range(1, 15)] public short number;

        // for warp tile
        [Range(1, 15)] public int pairNumber;
    }
}
