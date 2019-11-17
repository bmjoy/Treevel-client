using Project.Scripts.Utils.Definitions;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene
{
    public class TransitionSelfToggle : MenuSelectToggle
    {
        private string _tiedScene;

        protected override void Awake()
        {
            base.Awake();
            _tiedScene = SceneName.LEVEL_SELECT_SCENE;
        }

        /// <summary>
        /// トグルの状態が変更した場合の処理
        /// </summary>
        /// <param name="toggle"> 変更したトグル </param>
        protected override void ToggleValueChanged(GameObject toggle)
        {
            #if UNITY_EDITOR
            if (!EditorApplication.isPlaying) return;
            #endif

            // ONになった場合のみ処理
            if (isOn) {
                // 現在チェックされている Toggle を取得
                var nowScene = MenuSelectDirector.Instance.NowScene;
                var checkedToggle = GameObject.Find(nowScene.Replace("Scene", ""));

                if (checkedToggle != null) {
                    if (nowScene != SceneName.STAGE_SELECT_SCENE) checkedToggle.GetComponent<MenuSelectToggle>().isOn = false;
                    // 今のシーンをアンロード
                    SceneManager.UnloadSceneAsync(nowScene);
                    // 新しいシーンをロード
                    StartCoroutine(MenuSelectDirector.Instance.ChangeScene(GetSceneName()));
                }
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
            
            // StageSelctSceneにある時のみ、再度押すとLevelSelectSceneに遷移する
            if (isOn && _tiedScene == SceneName.STAGE_SELECT_SCENE) {
                _tiedScene = SceneName.LEVEL_SELECT_SCENE;
                isOn = true;
                ToggleValueChanged(gameObject);
            }
            // ON 状態のものを OFF にすることは許さない
            else if (!isOn) isOn = true;
        }


        /// <summary>
        /// Toggleに紐づいているscene nameを返す(Toggle nameと必ずしも一致しない)
        /// </summary>
        /// <returns></returns>
        protected override string GetSceneName() {
            return _tiedScene;
        }

        public void SetSceneName(string sceneName) {
            _tiedScene = sceneName;
        }
    }
}