using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene.Record
{
    public class GeneralRecordDirector : MonoBehaviour
    {
        /// <summary>
        /// [UI] "Share" ボタン
        /// </summary>
        [SerializeField] private Button _shareButton;

        /// <summary>
        /// [UI] "個別記録へ" ボタン
        /// </summary>
        [SerializeField] private Button _individualButton;

        /// <summary>
        /// [UI] "ステージクリア数" テキスト
        /// </summary>
        [SerializeField] private Text _clearStageNum;

        /// <summary>
        /// [UI] "ステージ数" テキスト
        /// </summary>
        [SerializeField] private Text _stageNum;

        /// <summary>
        /// [UI] ステージクリア割合ゲージ
        /// </summary>
        [SerializeField] private Image _clearStageGauge;

        /// <summary>
        /// [UI] ステージクリア割合ゲージの先端
        /// </summary>
        [SerializeField] private Image _clearStageIndicator;

        /// <summary>
        /// [UI] "プレイ回数" テキスト
        /// </summary>
        [SerializeField] private Text _playNum;

        /// <summary>
        /// [UI] "起動日数" テキスト
        /// </summary>
        [SerializeField] private Text _playDays;

        /// <summary>
        /// [UI] "フリック回数" テキスト
        /// </summary>
        [SerializeField] private Text _flickNum;

        /// <summary>
        /// [UI] "失敗回数" テキスト
        /// </summary>
        [SerializeField] private Text _failureNum;

        /// <summary>
        /// [GameObject] RecordDirector
        /// </summary>
        [SerializeField] private GameObject _recordDirectorGameObject;

        /// <summary>
        /// [Script] RecordDirector
        /// </summary>
        private RecordDirector _recordDirector;

        /// <summary>
        /// 全ステージの記録情報
        /// </summary>
        private readonly List<StageStatus> _stageStatuses = new List<StageStatus>();

        private void Awake()
        {
            _recordDirector = _recordDirectorGameObject.GetComponent<RecordDirector>();

            foreach (ETreeId treeId in Enum.GetValues(typeof(ETreeId))) {
                foreach (var stageNumber in Enumerable.Range(1, TreeInfo.NUM[treeId])) {
                    _stageStatuses.Add(StageStatus.Get(treeId, stageNumber));
                }
            }

            _shareButton.onClick.AddListener(ShareGeneralRecord);
            _individualButton.onClick.AddListener(_recordDirector.MoveToRight);
            _clearStageNum.text = GetClearStageNum();
            _stageNum.text = GetStageNum();
            _clearStageGauge.fillAmount = GetClearStagePercentage();
            _clearStageIndicator.GetComponent<RectTransform>().localPosition = GetClearStageIndicatorPosition();
            _playNum.text = GetPlayNum();
            _playDays.text = GetPlayDays();
            _flickNum.text = GetFlickNum();
            _failureNum.text = GetFailureNum();
        }

        private static void ShareGeneralRecord()
        {
            Application.OpenURL("https://twitter.com/intent/tweet?hashtags=Treevel");
        }

        private string GetClearStageNum()
        {
            var clearStageNum = _stageStatuses.Select(stageStatuses => stageStatuses.successNum > 0 ? 1 : 0).Sum();

            return clearStageNum.ToString();
        }

        private string GetStageNum()
        {
            var stageNum = _stageStatuses.Count;

            return stageNum.ToString();
        }

        private float GetClearStagePercentage()
        {
            var stageNum = _stageStatuses.Count;
            var clearStageNum = _stageStatuses.Select(stageStatuses => stageStatuses.successNum > 0 ? 1 : 0).Sum();

            var percentage = (float) clearStageNum / stageNum;

            return percentage;
        }

        private Vector3 GetClearStageIndicatorPosition()
        {
            var radius = _clearStageGauge.GetComponent<RectTransform>().rect.width / 2;
            var angle = GetClearStagePercentage() * 2 * Mathf.PI;

            // FIXME: 0.95 はゲージの太さを考慮するための Magic Number です
            var x = radius * Mathf.Sin(angle) * 0.95f;
            var y = radius * Mathf.Cos(angle) * 0.95f;

            return new Vector3(x, y);
        }

        private string GetPlayNum()
        {
            var playNum = _stageStatuses.Select(stageStatuses => stageStatuses.challengeNum).Sum();

            return playNum.ToString();
        }

        private static string GetPlayDays()
        {
            var playDays = RecordData.Instance.StartupDays;

            return playDays.ToString();
        }

        private string GetFlickNum()
        {
            var flickNum = _stageStatuses.Select(stageStatuses => stageStatuses.flickNum).Sum();

            return flickNum.ToString();
        }

        private string GetFailureNum()
        {
            var failureNum = _stageStatuses.Select(stageStatuses => stageStatuses.failureNum).Sum();

            return failureNum.ToString();
        }
    }
}
