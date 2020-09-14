using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Utils;
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
        /// [UI] 失敗理由グラフの背景
        /// </summary>
        [SerializeField] private GameObject _failureReasonGraphBackground;

        /// <summary>
        /// [UI] 失敗理由グラフの "No Data" テキスト
        /// </summary>
        [SerializeField] private Text _failureReasonGraphNoData;

        /// <summary>
        /// [UI] 失敗理由グラフの要素（Prefab）
        /// </summary>
        [SerializeField] private GameObject _failureReasonGraphElementPrefab;

        /// <summary>
        /// [UI] Others のアイコン（Prefab）
        /// </summary>
        [SerializeField] private GameObject _failureReasonOthersIconPrefab;

        /// <summary>
        /// [UI] Tornado のアイコン（Prefab）
        /// </summary>
        [SerializeField] private GameObject _failureReasonTornadoIconPrefab;

        /// <summary>
        /// [UI] Meteorite のアイコン（Prefab）
        /// </summary>
        [SerializeField] private GameObject _failureReasonMeteoriteIconPrefab;

        /// <summary>
        /// [UI] AimingMeteorite のアイコン（Prefab）
        /// </summary>
        [SerializeField] private GameObject _failureReasonAimingMeteoriteIconPrefab;

        /// <summary>
        /// [UI] Thunder のアイコン（Prefab）
        /// </summary>
        [SerializeField] private GameObject _failureReasonThunderIconPrefab;

        /// <summary>
        /// [UI] SolarBeam のアイコン（Prefab）
        /// </summary>
        [SerializeField] private GameObject _failureReasonSolarBeamIconPrefab;

        /// <summary>
        /// 全ステージの記録情報
        /// </summary>
        private List<StageStatus> _stageStatuses;

        /// <summary>
        /// 失敗理由を表示する最低割合
        /// </summary>
        private const float _FAILURE_REASON_SHOW_PERCENTAGE = 0.1f;

        private void Awake()
        {
            _stageStatuses = GameDataBase.GetAllStages()
                .Select(stage => StageStatus.Get(stage.TreeId, stage.StageNumber))
                .ToList();

            _shareButton.onClick.AddListener(ShareGeneralRecord);
            _individualButton.onClick.AddListener(RecordDirector.Instance.MoveToRight);
            _clearStageNum.text = GetClearStageNum();
            _stageNum.text = GetStageNum();
            _clearStageGauge.fillAmount = GetClearStagePercentage();
            _clearStageIndicator.GetComponent<RectTransform>().localPosition = GetClearStageIndicatorPosition();
            _playNum.text = GetPlayNum();
            _playDays.text = GetPlayDays();
            _flickNum.text = GetFlickNum();
            _failureNum.text = GetFailureNum();

            SetupFailureReasonGraph();
        }

        private static void ShareGeneralRecord()
        {
            Application.OpenURL("https://twitter.com/intent/tweet?hashtags=Treevel");
        }

        private string GetClearStageNum()
        {
            var clearStageNum = _stageStatuses.Select(stageStatus => stageStatus.successNum > 0 ? 1 : 0).Sum();

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
            var clearStageNum = _stageStatuses.Select(stageStatus => stageStatus.successNum > 0 ? 1 : 0).Sum();

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
            var playNum = _stageStatuses.Select(stageStatus => stageStatus.challengeNum).Sum();

            return playNum.ToString();
        }

        private static string GetPlayDays()
        {
            var playDays = RecordData.Instance.StartupDays;

            return playDays.ToString();
        }

        private string GetFlickNum()
        {
            var flickNum = _stageStatuses.Select(stageStatus => stageStatus.flickNum).Sum();

            return flickNum.ToString();
        }

        private string GetFailureNum()
        {
            var failureNum = _stageStatuses.Select(stageStatus => stageStatus.failureNum).Sum();

            return failureNum.ToString();
        }

        private void SetupFailureReasonGraph()
        {
            // 失敗回数の合計
            var sum = RecordData.Instance.FailureReasonCount.Sum(pair => pair.Value);

            if (sum == 0) {
                _failureReasonGraphNoData.gameObject.SetActive(true);
                return;
            }

            float startPoint = 0;

            var showOthers = RecordData.Instance.FailureReasonCount
                .Where(pair => pair.Key == EFailureReasonType.Others)
                .Select(pair => pair.Value != 0)
                .First();

            foreach (var pair in RecordData.Instance.FailureReasonCount) {
                // Others は別途扱う
                if (pair.Key.Equals(EFailureReasonType.Others)) continue;

                var fillAmount = (float) pair.Value / sum;

                // 10 % 未満なら Others に含める
                if (fillAmount < _FAILURE_REASON_SHOW_PERCENTAGE) {
                    showOthers = true;
                    continue;
                }

                CreateFailureReasonGraphElement(pair.Key, startPoint, fillAmount);

                // 開始地点をずらす
                startPoint += fillAmount;
            }

            if (showOthers) {
                CreateFailureReasonGraphElement(EFailureReasonType.Others, startPoint, 1 - startPoint);
            }
        }

        private void CreateFailureReasonGraphElement(EFailureReasonType type, float startPoint, float fillAmount)
        {
            var element = Instantiate(_failureReasonGraphElementPrefab, _failureReasonGraphBackground.transform, true);

            // 親を指定すると，localPosition や sizeDelta が prefab の時と変わるので調整する
            element.transform.localPosition = Vector3.zero;
            element.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            element.GetComponent<Image>().fillAmount = fillAmount;

            element.GetComponent<Image>().color = type.GetColor();

            // z 軸を変えることで fillAmount の開始地点を変える
            element.transform.localEulerAngles = new Vector3(0, 0, -360 * startPoint);

            GameObject iconPrefab;
            switch (type) {
                case EFailureReasonType.Tornado:
                    iconPrefab = _failureReasonTornadoIconPrefab;
                    break;
                case EFailureReasonType.Meteorite:
                    iconPrefab = _failureReasonMeteoriteIconPrefab;
                    break;
                case EFailureReasonType.AimingMeteorite:
                    iconPrefab = _failureReasonAimingMeteoriteIconPrefab;
                    break;
                case EFailureReasonType.Thunder:
                    iconPrefab = _failureReasonThunderIconPrefab;
                    break;
                case EFailureReasonType.SolarBeam:
                    iconPrefab = _failureReasonSolarBeamIconPrefab;
                    break;
                case EFailureReasonType.Others:
                    iconPrefab = _failureReasonOthersIconPrefab;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            var elementIcon = Instantiate(iconPrefab, element.transform, true);

            var radius = _failureReasonGraphBackground.GetComponent<RectTransform>().rect.width / 2;
            var angle = (fillAmount / 2) * 2 * Mathf.PI;

            var x = radius * Mathf.Sin(angle) * 0.5f;
            var y = radius * Mathf.Cos(angle) * 0.5f;

            // アイコンを円グラフの領域の中央に配置する
            elementIcon.transform.localPosition = new Vector3(x, y);
            // 親を指定すると，sizeDelta が prefab の時と変わるので調整する
            elementIcon.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        }
    }
}
