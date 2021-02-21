using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace Treevel.Common.Entities.GameDatas
{
    [Serializable]
    public class BottleData
    {
        public EBottleType type;
        [Range(1, 15)] public short initPos;
        public EGoalColor goalColor;
        public short life;
        public bool isSelfish;
        public bool isDark;
        public bool isReverse;
    }
}
