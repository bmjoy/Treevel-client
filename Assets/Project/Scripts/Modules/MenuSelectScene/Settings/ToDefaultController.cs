﻿using Treevel.Common.Entities;
using UnityEngine;

namespace Treevel.Modules.MenuSelectScene.Settings
{
    public class ToDefaultController : MonoBehaviour
    {
        public delegate void ChangeAction();

        /// <summary>
        /// デフォルトボタンが押された際のイベント
        /// </summary>
        public static event ChangeAction OnUpdate;

        /// <summary>
        /// デフォルトに戻すボタンを押した場合の処理
        /// </summary>
        public void ToDefaultButtonDown()
        {
            // 設定の更新
            UserSettings.ToDefault();

            // Canvasの更新
            OnUpdate?.Invoke();
        }
    }
}
