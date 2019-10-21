using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Attributes;

namespace Project.Scripts.GameDatas
{
    [System.Serializable]
    public class BulletData
    {
        public EBulletType type;
        public int ratio;

        public int line;

        public ECartridgeDirection direction;
        public List<int> randomCartridgeDirection;
        public List<int> randomRow;
        public List<int> randomColumn;
    }
}
