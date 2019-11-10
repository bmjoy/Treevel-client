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
        private GameObject _stageId;

        /// <summary>
        /// ステージ難易度
        /// </summary>
        private GameObject _stageDifficulty;

        /// <summary>
        /// クリア割合
        /// </summary>
        private GameObject _clearPercentage;

        /// <summary>
        /// 出現する銃弾
        /// </summary>
        private GameObject _appearingBullets;

        /// <summary>
        /// ゲームを開始するボタン
        /// </summary>
        public GameObject goToGame;

        /// <summary>
        /// 今後ウィンドウを表示するかのボタン
        /// </summary>
        private GameObject _hideOverview;

        private void Awake()
        {
            _stageId = transform.Find("StageId").gameObject;
            _stageDifficulty = transform.Find("StageDifficulty").gameObject;
            _clearPercentage = transform.Find("ClearPercentage").gameObject;
            _appearingBullets = transform.Find("AppearingBullets").gameObject;
            goToGame = transform.Find("GoToGame").gameObject;
            _hideOverview = transform.Find("HideOverview").gameObject;

            _hideOverview.GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                ToggleValueChanged(_hideOverview.GetComponent<Toggle>());
            });
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="stageId"> ステージID </param>
        public void Initialize(int stageId)
        {
            // ステージID
            _stageId.GetComponent<Text>().text = stageId.ToString();

            // TODO:ステージが"ステージ難易度"を持ったら実装
            // _stageDifficulty.GetComponent<Text>().text = ...

            // TODO:サーバで全ユーザのデータを持ったら実装
            // _clearPercentage.GetComponent<Text>().text = ...

            // TODO:ステージが"出現する銃弾"を持ったら実装
            // _appearingBullets... = ...

            // ゲームを開始するボタン
            goToGame.GetComponent<Button>().onClick.AddListener(() => FindObjectOfType<StageSelectDirector>().GoToGame(stageId));
        }

        private static void ToggleValueChanged(Toggle toggle)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.DO_NOT_SHOW, toggle.isOn ? 1 : 0);
        }
    }
}
