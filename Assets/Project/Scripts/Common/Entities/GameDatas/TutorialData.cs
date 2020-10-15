using UnityEngine.AddressableAssets;
using UnityEngine.Video;

namespace Treevel.Common.Entities.GameDatas
{

    public enum ETutorialType {
        None,
        Image,
        Video
    }

    [System.Serializable]
    public class TutorialData
    {
        public ETutorialType type;

        public AssetReferenceTexture2D image;
        public AssetReferenceVideoClip video;
    }

    [System.Serializable]
    public class AssetReferenceVideoClip : AssetReferenceT<VideoClip>
    {
        public AssetReferenceVideoClip(string guid) : base(guid) {}
    }
}
