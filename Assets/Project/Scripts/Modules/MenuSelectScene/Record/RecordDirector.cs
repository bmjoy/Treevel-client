using SnapScroll;
using Treevel.Common.Patterns.Singleton;
using Treevel.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.MenuSelectScene.Record
{
    public class RecordDirector : SingletonObject<RecordDirector>
    {
        /// <summary>
        /// [UI] "Share" ボタン
        /// </summary>
        [SerializeField] private Button _shareButton;

        /// <summary>
        /// [UI] "個別記録へ" ボタン
        /// </summary>
        [SerializeField] private Button _toIndividualButton;

        /// <summary>
        /// [UI] "総合記録へ" ボタン
        /// </summary>
        [SerializeField] private Button _toGeneralButton;

        private SnapScrollView _snapScrollView;

        private void Awake()
        {
            // 取得
            _snapScrollView = FindObjectOfType<SnapScrollView>();
            // ページの最大値を設定
            _snapScrollView.MaxPage = 2;
            // ページの横幅の設定
            _snapScrollView.PageSize = RuntimeConstants.ScaledCanvasSize.SIZE_DELTA.x;

            _shareButton.onClick.AddListener(ShareRecord);
            _toIndividualButton.onClick.AddListener(MoveToRight);
            _toGeneralButton.onClick.AddListener(MoveToLeft);

            _toGeneralButton.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _snapScrollView.OnPageChanged += HandleOnPageChanged;
        }

        private void OnDisable()
        {
            _snapScrollView.OnPageChanged -= HandleOnPageChanged;
        }

        private void HandleOnPageChanged()
        {
            if (_snapScrollView.Page == 0) {
                _toIndividualButton.gameObject.SetActive(true);
                _toGeneralButton.gameObject.SetActive(false);
            } else {
                _toIndividualButton.gameObject.SetActive(false);
                _toGeneralButton.gameObject.SetActive(true);
            }
        }

        private static void ShareRecord()
        {
            Application.OpenURL("https://twitter.com/intent/tweet?hashtags=Treevel");
        }

        private void MoveToRight()
        {
            _snapScrollView.Page = 1;
            _snapScrollView.RefreshPage();
        }

        private void MoveToLeft()
        {
            _snapScrollView.Page = 0;
            _snapScrollView.RefreshPage();
        }
    }
}
