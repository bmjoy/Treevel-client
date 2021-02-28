using System;
using UnityEngine;

namespace Treevel.Common.Entities.GameDatas
{
    [Serializable]
    public class TileData
    {
        public ETileType type;
        public EGoalColor goalColor;
        [Range(1, 15)] public short number;

        // for warp tile
        [Range(1, 15)] public short pairNumber;
    }
}
