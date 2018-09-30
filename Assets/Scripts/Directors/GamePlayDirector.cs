using System;
using Bullet;
using Panel;
using Tile;
using UnityEngine;

namespace Directors
{
    public class GamePlayDirector : MonoBehaviour
    {
        public delegate void FailureAction();

        public static event FailureAction OnFail;

        public enum GameState
        {
            Opening,
            Playing,
            Failure
        }

        public GameState currentState;

        private GameObject tileGenerator;

        private GameObject panelGenerator;

        private GameObject bulletGenerator;

        private void Start()
        {
            tileGenerator = GameObject.Find("TileGenerator");
            panelGenerator = GameObject.Find("PanelGenerator");
            bulletGenerator = GameObject.Find("BulletGenerator");

            Dispatch(GameState.Opening);
        }

        private void Update()
        {
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
            tileGenerator.GetComponent<TileGenerator>().CreateTiles();
            // パネル作成スクリプトを起動
            panelGenerator.GetComponent<PanelGenerator>().CreatePanels();
            // 銃弾作成スクリプトを起動
            bulletGenerator.GetComponent<BulletGenerator>().CreateBullets();

            Destroy(tileGenerator);
            Destroy(panelGenerator);
            // 状態を変更する
            Dispatch(GameState.Playing);
        }

        private void GameFail()
        {
            if (OnFail != null) OnFail();
            Destroy(bulletGenerator);
        }
    }
}
