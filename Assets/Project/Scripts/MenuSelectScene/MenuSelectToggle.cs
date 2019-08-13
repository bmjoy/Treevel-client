using Project.Scripts.MenuSelectScene;
﻿using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.Utils.MyUiConponents
{
    public class MenuSelectToggle : Toggle
    {
        protected override void Awake()
        {
            base.Awake();
            onValueChanged.AddListener(delegate {
                ToggleValueChanged(gameObject);
            });
        }

        private void ToggleValueChanged(GameObject toggle)
        {
            // ONになった場合のみ処理
            if (isOn && EditorApplication.isPlaying) {
                var nowScene = MenuSelectDirector.Instance.NowScene;
                // 現在チェックされている Toggle を取得
                var checkedToggle = GameObject.Find(nowScene.Replace("Scene", ""));
                checkedToggle.GetComponent<MenuSelectToggle>().isOn = false;
                // 今のシーンをアンロード
                SceneManager.UnloadSceneAsync(nowScene);
                // 新しいシーンをロード
                StartCoroutine(MenuSelectDirector.Instance.AddScene(name + "Scene"));
            }
        }

        /// <summary>
        /// ToggleGroup の AllowSwitchOff=false を再現
        /// <see cref="Toggle.OnPointerClick"/>.
        /// </summary>
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            if (!IsActive() || !IsInteractable())
                return;

            // ON 状態のものを OFF にすることは許さない
            if (!isOn) isOn = true;
        }
    }
}
