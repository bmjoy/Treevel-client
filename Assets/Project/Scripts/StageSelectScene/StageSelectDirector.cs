using Project.Scripts.GamePlayScene;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Scripts.StageSelectScene
{
    public class StageSelectDirector : MonoBehaviour
    {
        public GameObject stageButtonPrefab;

        public GameObject dummyBackgroundPrefab;

        private void Awake()
        {
            // ボタンの作成
            // MakeButtons();
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
    }
}
