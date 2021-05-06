using System;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
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

        public static IObservable<Unit> DataReset => _dataResetSubject.AsObservable();
        private static readonly Subject<Unit> _dataResetSubject = new Subject<Unit>();

        private void Awake()
        {
            _resetButton = GetComponent<Button>();
            _resetButton.onClick.AsObservable().Subscribe(_ => {
                SoundManager.Instance.PlaySE(ESEKey.UI_Button_Click_General);
                UIManager.Instance.CreateOkCancelMessageDialog(
                    ETextIndex.RecordResetConfirmDialogMessage,
                    ETextIndex.MessageDlgOkBtnText,
                    ResetData
                );
            }).AddTo(this);
        }

        private static void ResetData()
        {
            _dataResetSubject.OnNext(Unit.Default);

            // 全ステージをリセット
            StageRecordService.Instance.ResetAsync().Forget();

            // ステージ選択画面のブランチをリセット
            PlayerPrefs.DeleteKey(Constants.PlayerPrefsKeys.BRANCH_STATE);

            // キャンバスの設定をリセット
            UserSettings.LevelSelectCanvasScale = Default.LEVEL_SELECT_CANVAS_SCALE;
            UserSettings.LevelSelectScrollPosition = Default.LEVEL_SELECT_SCROLL_POSITION;
        }
    }
}
