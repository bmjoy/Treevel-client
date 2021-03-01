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
        private static readonly Dictionary<string, StageData> _stageDataMap = new Dictionary<string, StageData>();

        public static async UniTask Initialize()
        {
            // Stageラベルがついてる全てのアセットのアドレスを取得
            var locations = await Addressables.LoadResourceLocationsAsync("Stage").ToUniTask();

            var stageDatas =
                await UniTask.WhenAll(locations.Select(loc => Addressables.LoadAssetAsync<StageData>(loc).ToUniTask()));
            foreach (var stage in stageDatas) {
                _stageDataMap.Add(StageData.EncodeStageIdKey(stage.TreeId, stage.StageNumber), stage);
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
            return StageData.EncodeStageIdKeys(treeId)
                .Where(stageKey => _stageDataMap.ContainsKey(stageKey))
                .Select(stageKey => _stageDataMap[stageKey]);
        }

        public static IEnumerable<StageData> GetAllStages()
        {
            return _stageDataMap.Values;
        }
    }
}
