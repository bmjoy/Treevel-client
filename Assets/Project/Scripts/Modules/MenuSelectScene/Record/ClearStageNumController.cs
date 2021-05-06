using Treevel.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.MenuSelectScene.Record
{
    public class ClearStageNumController : MonoBehaviour
    {
        /// <summary>
        /// [UI] "ステージクリア数" テキスト
        /// </summary>
        [SerializeField] private Text _clearStageNum;

        /// <summary>
        /// [UI] "ステージ合計数" テキスト
        /// </summary>
        [SerializeField] private Text _totalStageNum;

        /// <summary>
        /// [UI] ステージクリア割合ゲージ
        /// </summary>
        [SerializeField] private Image _clearStageGauge;

        /// <summary>
        /// [UI] ステージクリア割合ゲージの先端
        /// </summary>
        [SerializeField] private Image _clearStageGaugeIndicator;

        public void SetUp(int clearStageNum, int totalStageNum, Color mainColor)
        {
            _clearStageNum.text = clearStageNum.ToString();
            _totalStageNum.text = totalStageNum.ToString();

            var clearStagePercentage = (float)clearStageNum / totalStageNum;
            _clearStageGauge.fillAmount = clearStagePercentage;

            // 画面の縦横比によって横方向、縦方向の倍率が異なるので調整する
            var gaugeRadiusWidth = _clearStageGauge.GetComponent<RectTransform>().rect.width / 2f * Screen.width / Constants.DeviceSize.WIDTH;
            var gaugeRadiusHeight = _clearStageGauge.GetComponent<RectTransform>().rect.height / 2f * Screen.height / Constants.DeviceSize.HEIGHT;
            var indicatorAngle = clearStagePercentage * 2 * Mathf.PI;

            // FIXME: 0.95 はゲージの太さを考慮するための Magic Number です
            var indicatorX = gaugeRadiusWidth * Mathf.Sin(indicatorAngle) * 0.95f;
            var indicatorY = gaugeRadiusHeight * Mathf.Cos(indicatorAngle) * 0.95f;

            _clearStageGaugeIndicator.GetComponent<RectTransform>().localPosition = new Vector3(indicatorX, indicatorY);

            _clearStageGauge.color = mainColor;
            _clearStageGaugeIndicator.color = mainColor;
        }
    }
}
