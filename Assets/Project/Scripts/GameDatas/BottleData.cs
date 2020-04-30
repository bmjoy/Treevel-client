using Project.Scripts.Utils.Definitions;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Project.Scripts.GameDatas
{
    [System.Serializable]
    public class BottleData
    {
        public EBottleType type;
        [Range(1, 15)] public int initPos;
        [Range(1, 15)] public short targetPos;
        public AssetReferenceSprite bottleSprite;
        public AssetReferenceSprite targetTileSprite;
        [Min(2)] public short life;
    }
}
