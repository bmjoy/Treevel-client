using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Project.Scripts.GameDatas
{
    [System.Serializable]
    public class GimmickData
    {
        public bool isLoop;
        public float appearanceTime;
        public float interval;
        public EGimmickType type;

        public ERow row;
        public EColumn column;

        public int line;

        public ECartridgeDirection direction;

        public List<ECartridgeDirection> turnDirections;

        public List<int> turnLines;

        [Range(1, 15)] public List<int> aimingBottles;

        public List<int> randomCartridgeDirection;
        public List<int> randomRow;
        public List<int> randomColumn;

        public List<int> randomTurnDirection;
        public List<int> randomTurnRow;
        public List<int> randomTurnColumn;

        public List<int> randomAttackableBottles;
    }
}
