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
        private Text _stageDifficulty;

        /// <summary>
        /// クリア割合
        /// </summary>
        private Text _clearPercentage;

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
            _stageIdText = transform.Find("StageId").GetComponent<Text>();
            _stageDifficulty = transform.Find("StageDifficulty").GetComponent<Text>();
            _clearPercentage = transform.Find("ClearPercentage").GetComponent<Text>();
            _appearingBullets = transform.Find("AppearingBullets").gameObject;
            goToGame = transform.Find("GoToGame").gameObject;

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
            var stageData = GameDataBase.Instance.GetStage(_stageId);

            // ステージID
            _stageIdText.text = _stageId.ToString();

            if (stageData == null)
                return;

            // TODO:ステージが"ステージ難易度"を持ったら実装
            // _stageDifficulty.GetComponent<Text>().text = ...

            // TODO:サーバで全ユーザのデータを持ったら実装
            // _clearPercentage.GetComponent<Text>().text = ...

            var overviewBullets = stageData.OverviewBullets;
            for (int i = 1 ; i <= 3 ; ++i) {
                var bulletOverviewPanel = _appearingBullets.transform.Find($"GimmickOverview{i}");
                if (overviewBullets.Count >= i) {
                    bulletOverviewPanel.GetComponentInChildren<Text>().text = overviewBullets[i - 1].ToString();
                    bulletOverviewPanel.gameObject.SetActive(true);
                } else {
                    bulletOverviewPanel.gameObject.SetActive(false);
                }
            }
        }
    }
}
