using Project.Scripts.Utils.Definitions;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene
{
    public class MenuSelectToggle : Toggle
    {
        protected enum ESceneName {
            LevelSelectScene, 
            StageSelectScene,
            RecordScene,
            TutorialScene,
            ConfigScene,
        }

        [SerializeField] protected ESceneName tiedSceneName;

        protected override void Awake()
        {
            base.Awake();
            onValueChanged.AddListener(delegate {
                ToggleValueChanged(gameObject);
            });
        }

        /// <summary>
        /// トグルの状態が変更した場合の処理
        /// </summary>
        /// <param name="toggle"> 変更したトグル </param>
        protected void ToggleValueChanged(GameObject toggle)
        {
            #if UNITY_EDITOR
            if (!EditorApplication.isPlaying) return;
            #endif

            if (isOn) {
                // ONになった場合の処理
                // Toggleに基づいて表示されているシーンを更新
                MenuSelectDirector.Instance.SetNowScene(this);
            } else {
                // OFFになった場合の処理
                // シーンをアンロード
                SceneManager.UnloadSceneAsync(GetSceneName());
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

        /// <summary>
        /// Toggleに紐づいているscene nameを返す
        /// </summary>
        /// <returns></returns>
        public virtual string GetSceneName() {
            return tiedSceneName.ToString();
        }
    }
}
