using System.Linq;
using UnityEditor;

namespace Treevel.Editor
{
    public static class AssetsReserializer
    {
        [MenuItem("Tools/Reserialize Stage Data")]
        public static void ReserializeStageData()
        {
            // アセット群を取得
            var stageDataAssets = AssetDatabase.FindAssets("t:StageData", new[] { "Assets/Project/GameDatas/Stages" });
            // パスに変換
            var stageDataAssetPaths = stageDataAssets.Select(AssetDatabase.GUIDToAssetPath);
            // リシアライズ
            AssetDatabase.ForceReserializeAssets(stageDataAssetPaths);
        }
    }
}
