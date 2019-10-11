using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GameDatas
{
    [System.Serializable]
    public class PanelData
    {
        public EPanelType type;
        [Range(1,15)] public int position;
        public short number;
        public short targetPos;
        public short life;
    }
}
