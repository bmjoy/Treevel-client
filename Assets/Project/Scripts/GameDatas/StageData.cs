using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;
using RotaryHeart.Lib.SerializableDictionary;
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
        public class PanelData
        {
            private const string PARAMETER_TOOLTIP = "Dynamic： None\nStatic: None\nNumber: 1. number, 2. target position\nLife: 1. number, 2. target position, 3. life";
            public int position;
            public EPanelType type;

            // TODO: draw parameter list after panel type selected;
            /// <summary>
            /// パネルのパラメータ
            /// Dynamic： None
            /// Static: None
            /// Number: 1. number, 2. target position
            /// Life: 1. number, 2. target position, 3. life
            /// </summary>
            [Tooltip(PARAMETER_TOOLTIP)] public List<string> parameters;
        }

        [System.Serializable]
        private class Tile
        {

        }
        [System.Serializable] public class PanelDictionary : SerializableDictionaryBase<int, PanelData> {}

        [SerializeField] private int id;
        [SerializeField, ID("position")] private PanelDictionary panels;
        [SerializeField] List<BulletGroup> bulletGroups;



        public PanelDictionary PanelDatas
        {
            get {
                return panels;
            }
        }
    }
}
