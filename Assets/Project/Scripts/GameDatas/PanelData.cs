using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GameDatas
{
    [System.Serializable]
    public class PanelData
    {
        public EPanelType type;
        [Range(1, 15)] public int initPos;
        [Range(1, 8)] public short number;
        [Range(1, 15)] public short targetPos;
        public short life;
    }
}
