using System;
using System.Runtime.InteropServices;
using Bullet;
using Panel;
using Tile;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.Analytics;

namespace Directors
{
    public class GamePlayDirector : MonoBehaviour
    {
        public enum GameState{
            Opening,
            Playing,
            Failure
        }

        public GameState currentState;

        private GameObject tileGenerator;

        private GameObject panelGenerator;

        private GameObject bulletGenerator;

        // Use this for initialization
        private void Start()
        {
            tileGenerator = GameObject.Find("TileGenerator");
            panelGenerator = GameObject.Find("PanelGenerator");
            bulletGenerator = GameObject.Find("BulletGenerator");

            Dispatch(GameState.Opening);
        }

        // Update is called once per frame
        private void Update()
        {
        }

        // 状態による振り分け処理

        public void Dispatch (GameState state)
        {
            currentState = state;
            switch (state) {
                case GameState.Opening:
                    GameOpening ();
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
            Destroy(bulletGenerator);
        }
    }
}
