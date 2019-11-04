using System;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene
{
    public class PopupWindow : MonoBehaviour
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
        private GameObject _goToGame;

        /// <summary>
        /// 今後ウィンドウを表示するかのボタン
        /// </summary>
        private GameObject _doNotShow;

        private void Awake()
        {
            _stageId = transform.Find("StageId").gameObject;
            _stageDifficulty = transform.Find("StageDifficulty").gameObject;
            _clearPercentage = transform.Find("ClearPercentage").gameObject;
            _appearingBullets = transform.Find("AppearingBullets").gameObject;
            _goToGame = transform.Find("GoToGame").gameObject;
            _doNotShow = transform.Find("DoNotShow").gameObject;

            _doNotShow.GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                ToggleValueChanged(_doNotShow.GetComponent<Toggle>());
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
            _goToGame.GetComponent<Button>().onClick.AddListener(() => FindObjectOfType<StageSelectDirector>().GoToGame(stageId));
        }

        private static void ToggleValueChanged(Toggle toggle)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.DO_NOT_SHOW, toggle.isOn ? 1 : 0);
        }
    }
}
