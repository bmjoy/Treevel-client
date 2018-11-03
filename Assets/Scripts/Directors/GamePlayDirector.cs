using System;
using System.Linq;
using Bullet;
using Panel;
using Tile;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Directors
{
    public class GamePlayDirector : MonoBehaviour
    {
        public delegate void ChangeAction();

        public static event ChangeAction OnFail;
        public static event ChangeAction OnSucceed;

        public enum GameState
        {
            Opening,
            Playing,
            Success,
            Failure
        }

        // 型がstringなのかintなのか
        // 1-2のように"-"区切りにするか（変数を"1"と"2"で分けるか）
        // 上記のように，ステージが増えた際に決めること多数有り
        public static string stageNum = "";

        public GameState currentState;

        private GameObject tileGenerator;

        private GameObject panelGenerator;

        private GameObject bulletGenerator;

        private GameObject resultWindow;

        private GameObject resultText;

        private GameObject warningText;

        private GameObject stageNumberText;

        private void Start()
        {
            tileGenerator = GameObject.Find("TileGenerator");
            panelGenerator = GameObject.Find("PanelGenerator");
            bulletGenerator = GameObject.Find("BulletGenerator");

            resultWindow = GameObject.Find("ResultWindow");
            resultWindow.SetActive(false);

            resultText = resultWindow.transform.Find("Result").gameObject;
            warningText = resultWindow.transform.Find("Warning").gameObject;
            stageNumberText = GameObject.Find("StageNumberText");
            // 現在のステージ番号を格納
            stageNumberText.GetComponent<Text>().text = stageNum;

            Dispatch(GameState.Opening);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)    // アプリがバックグラウンドに移動した時
            {
                Dispatch(GameState.Failure);
                warningText.GetComponent<Text>().text = "アプリが\nバックグラウンドに\n移動しました";
            }
        }

        private void Update()
        {
        }

        public void CheckClear()
        {
            GameObject[] panels = GameObject.FindGameObjectsWithTag("Panel");
            // 全てのパネルが最終位置にいたら，成功状態に遷移
            if (panels.Any(panel => panel.GetComponent<NormalPanelController>().adapted == false))
            {
                return;
            }

            Dispatch(GameState.Success);
        }

        // 状態による振り分け処理
        public void Dispatch(GameState state)
        {
            currentState = state;
            switch (state)
            {
                case GameState.Opening:
                    GameOpening();
                    break;
                case GameState.Playing:
                    break;
                case GameState.Success:
                    GameSucceed();
                    break;
                case GameState.Failure:
                    GameFail();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("state", state, null);
            }
        }

        private void GameOpening()
        {
            // タイル作成スクリプトを起動
            tileGenerator.GetComponent<TileGenerator>().CreateTiles(stageNum);
            // パネル作成スクリプトを起動
            panelGenerator.GetComponent<PanelGenerator>().CreatePanels(stageNum);
            // 銃弾作成スクリプトを起動
            bulletGenerator.GetComponent<BulletGenerator>().CreateBullets(stageNum);

            Destroy(tileGenerator);
            Destroy(panelGenerator);
            // 状態を変更する
            Dispatch(GameState.Playing);
        }

        private void GameSucceed()
        {
            resultWindow.SetActive(true);
            resultText.GetComponent<Text>().text = "成功！";
            Destroy(bulletGenerator);
            if (OnSucceed != null) OnSucceed();
        }

        private void GameFail()
        {
            resultWindow.SetActive(true);
            resultText.GetComponent<Text>().text = "失敗！";
            Destroy(bulletGenerator);
            if (OnFail != null) OnFail();
        }

        public void RetryButtonDown()
        {
            // 現在のScene名を取得する
            Scene loadScene = SceneManager.GetActiveScene();
            // Sceneの読み直し
            SceneManager.LoadScene(loadScene.name);
        }

        public void BackButtonDown()
        {
            // StageSelectSceneに戻る
            SceneManager.LoadScene("StageSelectScene");
        }
    }
}
