using System;
using Treevel.Common.Entities;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.MenuSelectScene.Settings
{
    public class ToDefaultController : MonoBehaviour
    {
        public delegate void ChangeAction();

        /// <summary>
        /// デフォルトボタンが押された際のイベント
        /// </summary>
        private static readonly Subject<Unit> _onToDefaultClickedSubject = new Subject<Unit>();
        public static readonly IObservable<Unit> OnToDefaultClicked = _onToDefaultClickedSubject.AsObservable();

        /// <summary>
        /// デフォルトに戻すボタンを押した場合の処理
        /// </summary>
        public void ToDefaultButtonDown()
        {
            // 設定の更新
            UserSettings.ToDefault();

            // Canvasの更新
            _onToDefaultClickedSubject.OnNext(Unit.Default);
        }
    }
}
