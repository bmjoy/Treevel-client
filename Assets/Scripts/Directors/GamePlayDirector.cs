﻿using System;
using Bullet;
using Panel;
using Tile;
using UnityEngine;
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

        public GameState currentState;

        private GameObject tileGenerator;

        private GameObject panelGenerator;

        private GameObject bulletGenerator;

        private GameObject resultText;

        private void Start()
        {
            tileGenerator = GameObject.Find("TileGenerator");
            panelGenerator = GameObject.Find("PanelGenerator");
            bulletGenerator = GameObject.Find("BulletGenerator");

            resultText = GameObject.Find("Result");
            resultText.SetActive(false);

            Dispatch(GameState.Opening);
        }

        private void Update()
        {
            // 成功判定検出をする
            // Dispatch(GameState.Success);
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

        private void GameSucceed()
        {
            resultText.SetActive(true);
            resultText.GetComponent<Text>().text = "成功！";
            if (OnSucceed != null) OnSucceed();
            Destroy(bulletGenerator);
        }

        private void GameFail()
        {
            resultText.SetActive(true);
            resultText.GetComponent<Text>().text = "失敗！";
            if (OnFail != null) OnFail();
            Destroy(bulletGenerator);
        }
    }
}
