using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using UnityEngine.AddressableAssets;

namespace Treevel.Common.Managers
{
    public static class GameDataManager
    {
        /// <summary>
        /// 初期化済みか
        /// </summary>
        private static bool _initialized;

        private static readonly Dictionary<string, StageData> _stageDataMap = new Dictionary<string, StageData>();
        private static readonly Dictionary<ETreeId, TreeData> _treeDataMap = new Dictionary<ETreeId, TreeData>();
        public static async UniTask InitializeAsync()
        {
            if (!_initialized) {
                // Stageラベルがついてる全てのアセットのアドレスを取得
                var locations = await Addressables.LoadResourceLocationsAsync("Stage").ToUniTask();

                var stageDataList =
                    await UniTask.WhenAll(locations.Select(loc => Addressables.LoadAssetAsync<StageData>(loc).ToUniTask()));
                foreach (var stage in stageDataList) {
                    _stageDataMap.Add(StageData.EncodeStageIdKey(stage.TreeId, stage.StageNumber), stage);
                }

                // Treeラベルがついてる全てのアセットのアドレスを取得
                locations = await Addressables.LoadResourceLocationsAsync("Tree").ToUniTask();

                var treeDataList =
                    await UniTask.WhenAll(locations.Select(loc => Addressables.LoadAssetAsync<TreeData>(loc).ToUniTask()));
                foreach (var tree in treeDataList) {
                    tree.stages = _stageDataMap.Values.Where(stage => stage.TreeId == tree.id).ToList();
                    _treeDataMap.Add(tree.id, tree);
                }

                _initialized = true;
            }
        }

        public static StageData GetStage(ETreeId treeId, int stageNumber)
        {
            var stageKey = StageData.EncodeStageIdKey(treeId, stageNumber);
            if (_stageDataMap.ContainsKey(stageKey)) {
                return _stageDataMap[stageKey];
            }

            return null;
        }

        public static IEnumerable<StageData> GetStages(ETreeId treeId)
        {
            return _treeDataMap[treeId].stages;
        }

        public static IEnumerable<StageData> GetAllStages()
        {
            return _stageDataMap.Values;
        }

        public static TreeData GetTreeData(ETreeId treeId)
        {
            return _treeDataMap[treeId];
        }
    }
}
