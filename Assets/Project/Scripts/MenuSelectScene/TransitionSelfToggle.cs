﻿using Project.Scripts.Utils.Definitions;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene
{
    public class TransitionSelfToggle : MenuSelectToggle
    {
        /// <summary>
        /// ON状態でToggleを押せるかどうか
        /// </summary>
        /// <value></value>
        public bool _isTransition {
            private get;
            set;
        }

        [SerializeField] private ESceneName tiedSecondSceneName;

        /// <summary>
        /// ToggleGroup の AllowSwitchOff=false を再現
        /// <see cref="Toggle.OnPointerClick"/>.
        /// </summary>
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            if (!IsActive() || !IsInteractable())
                return;
            
            // ON状態で自身を押下可能なとき
            if (isOn && _isTransition) {
                SceneManager.UnloadSceneAsync(GetSceneName());
                _isTransition = false;
                isOn = true;
                ToggleValueChanged(gameObject);
            }
            // ON 状態のものを OFF にすることは許さない
            else if (!isOn) isOn = true;
        }

        /// <summary>
        /// Toggleに紐づいているscene nameを返す
        /// </summary>
        /// <returns></returns>
        public override string GetSceneName() {
            if(!_isTransition) return tiedSceneName.ToString();
            return tiedSecondSceneName.ToString();
        }
    }
}