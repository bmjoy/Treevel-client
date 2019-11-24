﻿using Project.Scripts.Utils.PlayerPrefsUtils;
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

        /// <summary>
        /// 今後ウィンドウを表示するかのボタン
        /// </summary>
        private GameObject _hideOverview;

        private int _stageId;

        private void Awake()
        {
            _stageIdText = transform.Find("StageId").GetComponent<Text>();
            _stageDifficulty = transform.Find("StageDifficulty").GetComponent<Text>();
            _clearPercentage = transform.Find("ClearPercentage").GetComponent<Text>();
            _appearingBullets = transform.Find("AppearingBullets").gameObject;
            goToGame = transform.Find("GoToGame").gameObject;
            _hideOverview = transform.Find("HideOverview").gameObject;

            _hideOverview.GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                ToggleValueChanged(_hideOverview.GetComponent<Toggle>());
            });

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
            // ステージID
            _stageIdText.text = _stageId.ToString();

            // TODO:ステージが"ステージ難易度"を持ったら実装
            // _stageDifficulty.GetComponent<Text>().text = ...

            // TODO:サーバで全ユーザのデータを持ったら実装
            // _clearPercentage.GetComponent<Text>().text = ...

            // TODO:ステージが"出現する銃弾"を持ったら実装
            // _appearingBullets... = ...
        }

        private static void ToggleValueChanged(Toggle toggle)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.DO_NOT_SHOW, toggle.isOn ? 1 : 0);
        }
    }
}
