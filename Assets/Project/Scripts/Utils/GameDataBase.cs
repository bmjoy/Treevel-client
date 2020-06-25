using System.Collections.Generic;
using Project.Scripts.GameDatas;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.Utils
{
    public static class GameDataBase
    {
        private static Dictionary<string, StageData> _stageDataMap = new Dictionary<string, StageData>();

        public static void Initialize()
        {
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
        }

        public static StageData GetStage(ETreeId treeId, int stageNumber)
        {
            var stageKey = StageData.EncodeStageIdKey(treeId, stageNumber);
            if (_stageDataMap.ContainsKey(stageKey))
                return _stageDataMap[stageKey];
            else
                return null;
        }
    }
}
