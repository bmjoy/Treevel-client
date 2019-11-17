using System.Collections;
using System.Collections.Generic;

namespace Project.Scripts.GameDatas
{
    [System.Serializable]
    public class BulletGroupData
    {
        public float appearTime;
        public float interval;
        public bool loop;
        public List<BulletData> bullets;
    }
}
