using SnapScroll;
using Treevel.Common.Patterns.Singleton;
using Treevel.Common.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.MenuSelectScene.Record
{
    public class RecordDirector : SingletonObjectBase<RecordDirector>
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

            // 共有ボタン
            _shareButton.onClick.AsObservable().Subscribe(_ => {
                Application.OpenURL("https://twitter.com/intent/tweet?hashtags=Treevel");
            }).AddTo(this);

            // 「→」ボタン
            _toIndividualButton.onClick.AsObservable().Subscribe(_ => {
                _snapScrollView.Page = 1;
                _snapScrollView.RefreshPage();
            }).AddTo(this);

            // 「←」ボタン
            _toGeneralButton.onClick.AsObservable().Subscribe(_ => {
                _snapScrollView.Page = 0;
                _snapScrollView.RefreshPage();
            }).AddTo(this);

            // スクロール時のイベント
            Observable.FromEvent(
                h => _snapScrollView.OnPageChanged += HandleOnPageChanged,
                h => _snapScrollView.OnPageChanged -= HandleOnPageChanged
            ).Subscribe().AddTo(this);

            _toGeneralButton.gameObject.SetActive(false);
        }

        private void HandleOnPageChanged()
        {
            _toIndividualButton.gameObject.SetActive(_snapScrollView.Page == 0);
            _toGeneralButton.gameObject.SetActive(_snapScrollView.Page != 0);
        }
    }
}
