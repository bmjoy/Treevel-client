using Project.Scripts.Utils;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene
{
    public class OverviewPopup : MonoBehaviour
    {
        /// <summary>
        /// ステージID
        /// </summary>
        private Text _stageIdText;

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

        private int _stageId;

        private void Awake()
        {
            _stageIdText = transform.Find("PanelBackground/StageId").GetComponent<Text>();
            _stageDifficultyText = transform.Find("PanelBackground/StageDifficulty").GetComponent<Text>();
            _clearPercentageText = transform.Find("PanelBackground/ClearPercentage").GetComponent<Text>();
            _appearingBullets = transform.Find("PanelBackground/AppearingBullets").gameObject;
            goToGame = transform.Find("PanelBackground/GoToGame").gameObject;

            // ゲームを開始するボタン
            goToGame.GetComponent<Button>().onClick.AddListener(() => FindObjectOfType<StageSelectDirector>().GoToGame(_stageId));
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="stageId"> ステージID </param>
        public void SetStageId(int stageId)
        {
            _stageId = stageId;
        }

        void OnEnable()
        {
            var stageData = GameDataBase.GetStage(_stageId);

            // ステージID
            _stageIdText.text = _stageId.ToString();

            if (stageData == null)
                return;

            // TODO:ステージが"ステージ難易度"を持ったら実装
            // _stageDifficulty.GetComponent<Text>().text = ...

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
