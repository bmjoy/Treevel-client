using Project.Scripts.Utils;
using Project.Scripts.Utils.PlayerPrefsUtils;
using Project.Scripts.Utils.Definitions;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene
{
    public class OverviewPopup : MonoBehaviour
    {
        /// <summary>
        /// ステージID
        /// </summary>
        private Text _stageNumberText;

        /// <summary>
        /// ステージ難易度
        /// </summary>
        private Text _stageDifficultyText;

        /// <summary>
        /// クリア割合
        /// </summary>
        private Text _clearPercentageText;

        /// <summary>
        /// 出現する銃弾
        /// </summary>
        private GameObject _appearingBullets;

        /// <summary>
        /// ゲームを開始するボタン
        /// </summary>
        public GameObject goToGame;

        private ETreeId _treeId;

        private int _stageNumber;

        private void Awake()
        {
            _stageNumberText = transform.Find("PanelBackground/StageId").GetComponent<Text>();
            _stageDifficultyText = transform.Find("PanelBackground/StageDifficulty").GetComponent<Text>();
            _clearPercentageText = transform.Find("PanelBackground/ClearPercentage").GetComponent<Text>();
            _appearingBullets = transform.Find("PanelBackground/AppearingBullets").gameObject;
            goToGame = transform.Find("PanelBackground/GoToGame").gameObject;

            // ゲームを開始するボタン
            goToGame.GetComponent<Button>().onClick.AddListener(() => FindObjectOfType<StageSelectDirector>().GoToGame(_treeId, _stageNumber));
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="stageId"> ステージID </param>
        public void SetStageId(ETreeId treeId, int stageNumber)
        {
            _treeId = treeId;
            _stageNumber = stageNumber;
        }

        void OnEnable()
        {
            var stageData = GameDataBase.GetStage(_treeId, _stageNumber);

            // ステージID
            _stageNumberText.text = _stageNumber.ToString();

            if (stageData == null)
                return;

            _stageDifficultyText.GetComponent<Text>().text = _treeId.ToString();

            // TODO:サーバで全ユーザのデータを持ったら実装
            // _clearPercentage.GetComponent<Text>().text = ...

            var overviewBullets = stageData.OverviewGimmicks;
            for (int i = 1 ; i <= 3 ; ++i) {
                var bulletOverviewBottle = _appearingBullets.transform.Find($"GimmickOverview{i}");
                if (overviewBullets.Count >= i) {
                    bulletOverviewBottle.GetComponentInChildren<Text>().text = overviewBullets[i - 1].ToString();
                    bulletOverviewBottle.gameObject.SetActive(true);
                } else {
                    bulletOverviewBottle.gameObject.SetActive(false);
                }
            }
        }
    }
}
