using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Networks;
using Treevel.Common.Networks.Objects;
using Treevel.Common.Networks.Requests;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.MenuSelectScene.Record
{
    public class GraphPopupController : MonoBehaviour
    {
        /// <summary>
        /// [Text] 挑戦回数
        /// </summary>
        [SerializeField] private Text _challengeNumText;

        /// <summary>
        /// [Text] 成功率
        /// </summary>
        [SerializeField] private Text _clearRateText;

        public async void Initialize(ETreeId treeId, int stageNumber)
        {
            // TODO: 季節が拡張されたら、季節に応じた色に設定する
            gameObject.GetComponent<Image>().color = Color.magenta;

            var challengeNum = StageStatus.Get(treeId, stageNumber).challengeNum;
            _challengeNumText.text = challengeNum.ToString();

            var data = await NetworkService.Execute(new GetStageStatsRequest(StageData.EncodeStageIdKey(treeId, stageNumber)));
            var stageStats = (StageStats) data;
            _clearRateText.text = $"{stageStats.ClearRate * 100f:n2}";
        }
    }
}
