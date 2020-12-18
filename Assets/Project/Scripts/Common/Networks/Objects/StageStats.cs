using UnityEngine;

namespace Treevel.Common.Networks.Objects
{
    [System.Serializable]
    public struct StageStats {

        [SerializeField] private float clear_rate;
        public float ClearRate => clear_rate;

        [SerializeField] private float min_clear_time;
        public float MinClearTime => min_clear_time;

        [SerializeField] private string stage_id;
        public string StageId => stage_id;

        public StageStats(float clearRate, float minClearTime, string stageID)
        {
            clear_rate = clearRate;
            min_clear_time = minClearTime;
            stage_id = stageID;
        }
    }
}
