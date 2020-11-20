using UnityEngine;

namespace Treevel.Common.Networks.Objects
{
    [System.Serializable]
    public struct StageStats {
        public float clearRate => clear_rate;
        public float minClearTime => min_clear_time;
        public string stageId => stage_id;

        [SerializeField] float clear_rate;
        [SerializeField] float min_clear_time;
        [SerializeField] string stage_id;
    }
}
