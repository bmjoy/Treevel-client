using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GameDatas
{
    [System.Serializable]
    public class BulletData
    {
        public EBulletType type;
        public int ratio;

        public ERow row;
        public EColumn column;

        [Range(1, 15)] public List<int> aimingBottles;

        public List<int> randomRow;
        public List<int> randomColumn;

        public List<int> randomAttackableBottles;
    }
}
