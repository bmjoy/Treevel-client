using System.Collections.Generic;
using Project.Scripts.GameDatas;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Project.Scripts.Utils
{
    public static class GameDataBase
    {
        private static  Dictionary<int, StageData> _stageDataMap = new Dictionary<int, StageData>();

        [RuntimeInitializeOnLoadMethod]
        private static void Load()
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
                            _stageDataMap.Add(stage.Id, stage);
                        }
                    };
                }
            });

            Debug.Log("Loading Game Data Finished.");
        }

        public static StageData GetStage(int id)
        {
            if (_stageDataMap.ContainsKey(id))
                return _stageDataMap[id];
            else
                return null;
        }
    }
}
