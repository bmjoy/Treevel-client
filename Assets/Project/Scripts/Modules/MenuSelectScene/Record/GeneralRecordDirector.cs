using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.MenuSelectScene.Record
{
    public class GeneralRecordDirector : MonoBehaviour
    {
        /// <summary>
        /// Model
        /// </summary>
        private GeneralRecordModel _model;

        /// <summary>
        /// [View] "ステージクリア数" の prefab
        /// </summary>
        [SerializeField] private GameObject _clearStageNum;

        /// <summary>
        /// [View] "プレイ回数" テキスト
        /// </summary>
        [SerializeField] private Text _playNum;

        /// <summary>
        /// [View] "起動日数" テキスト
        /// </summary>
        [SerializeField] private Text _playDays;

        /// <summary>
        /// [View] "フリック回数" テキスト
        /// </summary>
        [SerializeField] private Text _flickNum;

        /// <summary>
        /// [View] "失敗回数" テキスト
        /// </summary>
        [SerializeField] private Text _failureNum;

        /// <summary>
        /// [View] 失敗理由グラフの背景
        /// </summary>
        [SerializeField] private GameObject _failureReasonGraphBackground;

        /// <summary>
        /// [View] 失敗理由グラフの "No Data" テキスト
        /// </summary>
        [SerializeField] private Text _failureReasonGraphNoData;

        /// <summary>
        /// [View] 失敗理由グラフの要素（Prefab）
        /// </summary>
        [SerializeField] private GameObject _failureReasonGraphElementPrefab;

        /// <summary>
        /// [View] 失敗理由グラフのアイコン（Prefab）
        /// </summary>
        [SerializeField] private GameObject _failureReasonGraphIconPrefab;

        /// <summary>
        /// [View] Others のアイコン（Sprite）
        /// </summary>
        [SerializeField] private Sprite _failureReasonOthersIconSprite;

        /// <summary>
        /// [View] Tornado のアイコン（Sprite）
        /// </summary>
        [SerializeField] private Sprite _failureReasonTornadoIconSprite;

        /// <summary>
        /// [View] Meteorite のアイコン（Sprite）
        /// </summary>
        [SerializeField] private Sprite _failureReasonMeteoriteIconSprite;

        /// <summary>
        /// [View] AimingMeteorite のアイコン（Sprite）
        /// </summary>
        [SerializeField] private Sprite _failureReasonAimingMeteoriteIconSprite;

        /// <summary>
        /// [View] Thunder のアイコン（Sprite）
        /// </summary>
        [SerializeField] private Sprite _failureReasonThunderIconSprite;

        /// <summary>
        /// [View] SolarBeam のアイコン（Sprite）
        /// </summary>
        [SerializeField] private Sprite _failureReasonSolarBeamIconSprite;

        /// <summary>
        /// [View] Powder のアイコン（Sprite）
        /// </summary>
        [SerializeField] private Sprite _failureReasonPowderIconSprite;

        /// <summary>
        /// 失敗理由を表示する最低割合
        /// </summary>
        private const float _FAILURE_REASON_SHOW_PERCENTAGE = 0.1f;

        private readonly List<GameObject> _shouldDestroyPrefabsOnDisable = new List<GameObject>();

        private void Awake()
        {
            _model = new GeneralRecordModel();

            /**
             * Model -> View
             */

            _model.stageRecordArray
                .Subscribe(stageRecordArray => {
                    var clearStageNum = stageRecordArray.Count(stageRecord => stageRecord.IsCleared);
                    var totalStageNum = stageRecordArray.Length;
                    _clearStageNum.GetComponent<ClearStageNumController>().SetUp(clearStageNum, totalStageNum, Color.blue);

                    _playNum.text = stageRecordArray.Select(stageRecord => stageRecord.challengeNum).Sum().ToString();
                    _flickNum.text = stageRecordArray.Select(stageRecord => stageRecord.flickNum).Sum().ToString();
                    _failureNum.text = stageRecordArray.Select(stageRecord => stageRecord.failureNum).Sum().ToString();
                })
                .AddTo(this);

            _model.startupDays
                .Subscribe(startupDays => _playDays.text = startupDays.ToString())
                .AddTo(this);

            _model.failureReasonCount
                .Subscribe(SetupFailureReasonGraph)
                .AddTo(this);
        }

        private void OnEnable()
        {
            _model.FetchStageRecordArray();
        }

        private void OnDisable()
        {
            _shouldDestroyPrefabsOnDisable.ForEach(Destroy);
            _shouldDestroyPrefabsOnDisable.Clear();
        }

        private void OnDestroy()
        {
            _model.Dispose();
        }

        private void SetupFailureReasonGraph(Dictionary<EFailureReasonType, int> failureReasonCount)
        {
            // 失敗回数の合計
            var sum = failureReasonCount.Sum(pair => pair.Value);

            if (sum == 0) {
                _failureReasonGraphNoData.gameObject.SetActive(true);
                return;
            }

            float startPoint = 0;

            var showOthers = failureReasonCount
                .Where(pair => pair.Key == EFailureReasonType.Others)
                .Select(pair => pair.Value != 0)
                .First();

            foreach (var pair in failureReasonCount) {
                // Others は別途扱う
                if (pair.Key.Equals(EFailureReasonType.Others)) continue;
                // 0 の場合は無視する
                if (pair.Value == 0) continue;

                var fillAmount = (float)pair.Value / sum;

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
            _shouldDestroyPrefabsOnDisable.Add(element);

            // 親を指定すると，localPosition や sizeDelta が prefab の時と変わるので調整する
            element.transform.localPosition = Vector3.zero;
            element.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            element.GetComponent<Image>().fillAmount = fillAmount;

            element.GetComponent<Image>().color = type.GetColor();

            // z 軸を変えることで fillAmount の開始地点を変える
            element.transform.localEulerAngles = new Vector3(0, 0, -360 * startPoint);

            Sprite iconSprite;
            switch (type) {
                case EFailureReasonType.Tornado:
                    iconSprite = _failureReasonTornadoIconSprite;
                    break;
                case EFailureReasonType.Meteorite:
                    iconSprite = _failureReasonMeteoriteIconSprite;
                    break;
                case EFailureReasonType.AimingMeteorite:
                    iconSprite = _failureReasonAimingMeteoriteIconSprite;
                    break;
                case EFailureReasonType.Thunder:
                    iconSprite = _failureReasonThunderIconSprite;
                    break;
                case EFailureReasonType.SolarBeam:
                    iconSprite = _failureReasonSolarBeamIconSprite;
                    break;
                case EFailureReasonType.Powder:
                    iconSprite = _failureReasonPowderIconSprite;
                    break;
                case EFailureReasonType.Others:
                    iconSprite = _failureReasonOthersIconSprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            var elementIcon = Instantiate(_failureReasonGraphIconPrefab, element.transform, true);
            elementIcon.GetComponent<Image>().sprite = iconSprite;

            _shouldDestroyPrefabsOnDisable.Add(elementIcon);
            elementIcon.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            var fillAmountInt = (int)fillAmount;
            if (fillAmountInt == 1) {
                elementIcon.transform.localPosition = Vector3.zero;
                return;
            }

            var radius = _failureReasonGraphBackground.GetComponent<RectTransform>().rect.width / 2;
            var angle = (fillAmount / 2) * 2 * Mathf.PI;

            var x = radius * Mathf.Sin(angle) * 0.5f;
            var y = radius * Mathf.Cos(angle) * 0.5f;

            // アイコンを円グラフの領域の中央に配置する
            elementIcon.transform.localPosition = new Vector3(x, y);
        }
    }
}
