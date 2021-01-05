using System;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.MenuSelectScene.LevelSelect;
using Treevel.Modules.StageSelectScene;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.MenuSelectScene.Settings
{
    public class ResetController : MonoBehaviour
    {
        /// <summary>
        /// ステージリセットボタン
        /// </summary>
        private Button _resetButton;

        public IObservable<Unit> OnResetData => _onResetDataSubject.AsObservable();
        private readonly Subject<Unit> _onResetDataSubject = new Subject<Unit>();

        private void Awake()
        {
            _resetButton = GetComponent<Button>();
            _resetButton.onClick.AsObservable().Subscribe(_ => {
                UIManager.Instance.CreateOkCancelMessageDialog(
                    ETextIndex.RecordResetConfirmDialogMessage,
                    ETextIndex.MessageDlgOkBtnText,
                    ResetData
                );
            }).AddTo(this);

            _onResetDataSubject.AddTo(this);
        }

        private void ResetData()
        {
            if (_onResetDataSubject.HasObservers) {
                _onResetDataSubject.OnNext(Unit.Default);
            }

            // 全ステージをリセット
            StageStatus.Reset();

            // 記録情報をリセット
            RecordData.Instance.Reset();

            // キャンバスの設定をリセット
            UserSettings.LevelSelectCanvasScale = Default.LEVEL_SELECT_CANVAS_SCALE;
            UserSettings.LevelSelectScrollPosition = Default.LEVEL_SELECT_SCROLL_POSITION;
        }
    }
}
