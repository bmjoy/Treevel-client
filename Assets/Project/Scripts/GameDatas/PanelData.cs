using Project.Scripts.Utils.Definitions;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Project.Scripts.GameDatas
{
    [System.Serializable]
    public class PanelData
    {
        public EPanelType type;
        [Range(1, 15)] public int initPos;
        [Range(1, 8)] public short number;
        [Range(1, 15)] public short targetPos;
        public AssetReferenceSprite panelSprite;
        public AssetReferenceSprite targetTileSprite;
        public short life;
    }
}
