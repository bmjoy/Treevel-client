using Project.Scripts.GamePlayScene;
using Project.Scripts.LevelSelectScene;
using Project.Scripts.MenuSelectScene;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene
{
    public abstract class StageSelectDirector : MonoBehaviour
    {
        public GameObject stageButtonPrefab;

        public GameObject dummyBackgroundPrefab;

        private void Awake()
        {
            // ボタンの作成
            MakeButtons();
        }

        protected void StageButtonDown(GameObject clickedButton)
        {
            // タップされたステージidを取得（暫定的にボタンの名前）
            var stageId = int.Parse(clickedButton.name);
            // 挑戦回数をインクリメント
            var ss = StageStatus.Get(stageId);
            ss.IncChallengeNum(stageId);
            // ステージ番号を渡す
            GamePlayDirector.stageId = stageId;
            // 背景画像をCanvasの下に置く
            var canvas = GetCanvas().GetComponent<RectTransform>();
            var background = Instantiate(dummyBackgroundPrefab);
            background.transform.SetParent(canvas, false);

            // ToggleGroupを削除する前に、allowSwitchOffをtrueにする
            var levelTab = GameObject.Find(LevelSelectDirector.LEVEL_TAB_TOGGLE_GROUP_NAME);
            levelTab.GetComponent<ToggleGroup>().allowSwitchOff = true;
            var menuTab = GameObject.Find(MenuSelectDirector.MENU_TAB_TOGGLE_GROUP_NAME);
            menuTab.GetComponent<ToggleGroup>().allowSwitchOff = true;

            // シーン遷移
            SceneManager.LoadScene(SceneName.GAME_PLAY_SCENE);
        }

        /* 現在アクティブなシーンの Canvas を取得 */
        private static GameObject GetCanvas()
        {
            var scene = SceneManager.GetActiveScene();
            foreach (var rootGameObject in scene.GetRootGameObjects()) {
                if (rootGameObject.name == "Canvas") {
                    return rootGameObject;
                }
            }

            return null;
        }

        /* ボタンの生成 */
        protected abstract void MakeButtons();
    }
}
