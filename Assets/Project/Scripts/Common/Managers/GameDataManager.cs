using System.Collections.Generic;
using System.Linq;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Treevel.Common.Managers
{
    public static class GameDataManager
    {
        private static Dictionary<string, StageData> _stageDataMap = new Dictionary<string, StageData>();

        private static bool _isinitialized = false;

        public static void Initialize()
        {
            if (_isinitialized)
                return;

            Debug.Log("Start Loading Game Data.");

            // Stageラベルがついてる全てのアセットのアドレスを取得
            Addressables.LoadResourceLocationsAsync("Stage").Completed += (op => {
                var locations = op.Result;

                // アドレス毎に読み込み、マップに追加
                foreach (var location in locations) {
                    Addressables.LoadAssetAsync<StageData>(location).Completed += (op1) => {
                        var stage = op1.Result;
                        lock (_stageDataMap) {
                            _stageDataMap.Add(StageData.EncodeStageIdKey(stage.TreeId, stage.StageNumber), stage);
                        }
                    };
                }
            });

            Debug.Log("Loading Game Data Finished.");

            _isinitialized = true;
        }

        public static StageData GetStage(ETreeId treeId, int stageNumber)
        {
            var stageKey = StageData.EncodeStageIdKey(treeId, stageNumber);
            if (_stageDataMap.ContainsKey(stageKey))
                return _stageDataMap[stageKey];
            else
                return null;
        }

        public static StageData[] GetStages(ETreeId treeId)
        {
            return StageData.EncodeStageIdKeys(treeId)
                .Where(stageKey => _stageDataMap.ContainsKey(stageKey))
                .Select(stageKey => _stageDataMap[stageKey])
                .ToArray();
        }

        public static StageData[] GetAllStages()
        {
            return _stageDataMap.Values.ToArray();
        }
    }
}
