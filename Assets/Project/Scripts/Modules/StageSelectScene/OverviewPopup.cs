using Treevel.Common.Entities;
using Treevel.Common.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.StageSelectScene
{
    public class OverviewPopup : MonoBehaviour
    {
        /// <summary>
        /// ステージID
        /// </summary>
        private Text _stageNumberText;

        /// <summary>
        /// クリア割合
        /// </summary>
        private Text _clearPercentageText;

        /// <summary>
        /// 出現する銃弾
        /// </summary>
        private GameObject _appearingGimmicks;

        /// <summary>
        /// ゲームを開始するボタン
        /// </summary>
        public GameObject goToGame;

        private ETreeId _treeId;

        private int _stageNumber;

        private void Awake()
        {
            _stageNumberText = transform.Find("PanelBackground/StageNumIconBase/StageNumText").GetComponent<Text>();
            _clearPercentageText = transform.Find("PanelBackground/StatusPanel/SuccessPercentage/Title").GetComponent<Text>();
            _appearingGimmicks = transform.Find("PanelBackground/AppearingGimmicks").gameObject;
            goToGame = transform.Find("PanelBackground/GoToGame").gameObject;

            // ゲームを開始するボタン
            goToGame.GetComponent<Button>().onClick.AddListener(() => FindObjectOfType<StageSelectDirector>().GoToGame(_treeId, _stageNumber));
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="treeId"> 木のID </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        public void SetStageId(ETreeId treeId, int stageNumber)
        {
            _treeId = treeId;
            _stageNumber = stageNumber;
        }

        void OnEnable()
        {
            var stageData = GameDataManager.GetStage(_treeId, _stageNumber);

            // ステージID
            _stageNumberText.text = _stageNumber.ToString();

            if (stageData == null)
                return;

            // _stageDifficultyText.GetComponent<Text>().text = _treeId.ToString();

            // TODO:サーバで全ユーザのデータを持ったら実装
            // _clearPercentage.GetComponent<Text>().text = ...

            var overviewGimmicks = stageData.OverviewGimmicks;
            for (int i = 1 ; i <= 3 ; ++i) {
                var gimmickOverviewBottle = _appearingGimmicks.transform.Find($"GimmickOverview{i}");
                if (overviewGimmicks.Count >= i) {
                    gimmickOverviewBottle.GetComponentInChildren<Text>().text = overviewGimmicks[i - 1].ToString();
                    gimmickOverviewBottle.gameObject.SetActive(true);
                } else {
                    gimmickOverviewBottle.gameObject.SetActive(false);
                }
            }
        }
    }
}
