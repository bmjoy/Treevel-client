﻿using System;
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

        public IObservable<Unit> DataReset => _dataResetSubject.AsObservable();
        private readonly Subject<Unit> _dataResetSubject = new Subject<Unit>();

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

            _dataResetSubject.AddTo(this);
        }

        private void ResetData()
        {
            _dataResetSubject.OnNext(Unit.Default);

            // 全ステージをリセット
            StageStatus.Reset();

            // 記録情報をリセット
            RecordData.Instance.Reset();

            // ステージ選択画面のブランチをリセット
            PlayerPrefs.DeleteKey(Constants.PlayerPrefsKeys.BRANCH_STATE);

            // キャンバスの設定をリセット
            UserSettings.LevelSelectCanvasScale = Default.LEVEL_SELECT_CANVAS_SCALE;
            UserSettings.LevelSelectScrollPosition = Default.LEVEL_SELECT_SCROLL_POSITION;
        }
    }
}
