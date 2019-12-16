using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Patterns;
using UnityEngine;

namespace Project.Scripts.Utils
{
    public class GameDataBase : SingletonObject<GameDataBase>
    {
        private static  Dictionary<int, StageData> _stageDataMap = new Dictionary<int, StageData>();

        void Awake()
        {
            // TODO: 非同期で読み込み
            Load();
        }

        private static void Load()
        {
            Debug.Log("Start Loading Game Data.");
            var stageDataList = Resources.LoadAll<StageData>("GameDatas/Stages/");
            _stageDataMap = stageDataList.ToDictionary(x => x.Id);

            Debug.Log("Loading Game Data Finished.");
        }

        public StageData GetStage(int id)
        {
            if (_stageDataMap.ContainsKey(id))
                return _stageDataMap[id];
            else 
                return null;
        }
    }
}
