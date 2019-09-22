using Project.Scripts.GamePlayScene;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Scripts.GamePauseScene
{
    public class GamePauseDirector : MonoBehaviour
    {
        /// <summary>
        /// ゲーム再開ボタン押下時の処理
        /// </summary>
        public void BackButtonDown(){
            Time.timeScale = 1.0f;
            // 今のシーンをアンロード
            SceneManager.UnloadSceneAsync(SceneName.GAME_PAUSE_SCENE);
        }

        /// <summary>
        /// ゲームを諦めるボタン押下時の処理
        /// </summary>
        public void QuitButtonDown()
        {
            Time.timeScale = 1.0f;
            // 失敗回数をインクリメント
            var ss = StageStatus.Get(GamePlayDirector.stageId);
            ss.IncFailureNum(GamePlayDirector.stageId);
            // StageSelectSceneに戻る
            SceneManager.LoadScene(SceneName.MENU_SELECT_SCENE);
        }

        /// <summary>
        /// 一時停止中にアプリを終了した時に呼ばれる
        /// </summary>
        // TODO : 実機で検証する
        private void OnApplicationQuit()
        {
            // 失敗回数をインクリメント
            var ss = StageStatus.Get(GamePlayDirector.stageId);
            ss.IncFailureNum(GamePlayDirector.stageId);
        }
    }
}

