using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GameDatas
{
    [CreateAssetMenu(fileName = "stage.asset", menuName = "Stage")]
    public class StageData : ScriptableObject
    {
        [System.Serializable]
        private class BulletGroup
        {
            public float appearTime;
            public float interval;
            public bool loop;
            public List<Bullet> bullets;
        }

        [System.Serializable]
        private class Bullet
        {
            public EBulletType type;
            public int ratio;
            public List<string> parameters;
        }

        [System.Serializable]
        private class Tile
        {

        }
        [SerializeField] private int id;
        [SerializeField] private List<BulletGroup> bulletGroups;
        [SerializeField] private List<PanelData> panels;



        public List<PanelData> PanelDatas
        {
            get {
                return panels;
            }
        }
    }
}
