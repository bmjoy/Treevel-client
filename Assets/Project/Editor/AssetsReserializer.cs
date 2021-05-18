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

        [MenuItem("Tools/Reserialize Tree Data")]
        public static void ReserializeTreeData()
        {
            // アセット群を取得
            var treeDataAssets = AssetDatabase.FindAssets("t:TreeData", new[] { "Assets/Project/GameDatas/Trees" });
            // パスに変換
            var treeDataAssetPaths = treeDataAssets.Select(AssetDatabase.GUIDToAssetPath);
            // リシアライズ
            AssetDatabase.ForceReserializeAssets(treeDataAssetPaths);
        }
    }
}
