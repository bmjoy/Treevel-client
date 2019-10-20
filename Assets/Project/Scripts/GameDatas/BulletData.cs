using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GameDatas
{
    [System.Serializable]
    public class BulletData
    {
        public EBulletType type;
        public int ratio;

        public ERow row;
        public EColumn column;
        public ECartridgeDirection direction;
    }
}
