using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.GamePlayScene.Bullet;
using Project.Scripts.GamePlayScene.Panel;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils;

namespace Project.Scripts.GamePlayScene
{
    public class StageGenerator
    {
        /// <summary>
        /// ステージを作成する
        /// </summary>
        /// <param name="stageId"> ステージ id </param>
        /// <exception cref="NotImplementedException"> 実装されていないステージ id を指定した場合 </exception>
        public static void CreateStages(int stageId)
        {
            var tileGenerator = TileGenerator.Instance;
            var panelGenerator = PanelGenerator.Instance;
            var bulletGroupGenerator = BulletGroupGenerator.Instance;

            List<IEnumerator> coroutines = new List<IEnumerator>();

            // ステージデータ読み込む
            StageData stageData = GameDataBase.Instance.GetStage(stageId);
            if (stageData != null) {
                tileGenerator.CreateTiles(stageData.TileDatas);
                panelGenerator.CreatePanels(stageData.PanelDatas);
                coroutines = bulletGroupGenerator.CreateBulletGroups(stageData.BulletGroups);

                // 銃弾一括生成
                bulletGroupGenerator.CreateBulletGroups(coroutines);
                return; // NotImplementedExceptionを回避するため
            }

            // TODO 全ステージデータを作成
            switch (stageId) {
                case 6:
                    // 銃弾実体生成
                    // ランダムな引数でNormalCartridgeを生成する
                    coroutines.Add(bulletGroupGenerator.CreateBulletGroup(
                            appearanceTime: 1.0f,
                            interval: 5.0f,
                            loop: true,
                    bulletGenerators: new List<GameObject>() {
                        bulletGroupGenerator.CreateNormalCartridgeGenerator(ratio: 100,
                            cartridgeDirection: ECartridgeDirection.Random, row: ERow.Random),
                                                bulletGroupGenerator.CreateRandomNormalCartridgeGenerator(ratio: 100,
                                                    randomCartridgeDirection: new int[] {200, 10, 100, 0},
                                                    randomRow: new int[] {100, 5, 5, 5, 100}, randomColumn: new int[] {100, 10, 0}),
                    }));
                    /* 特殊タイル -> 数字パネル -> 特殊パネル */
                    tileGenerator.CreateNormalTiles();
                    // 数字パネル作成
                    panelGenerator.CreateNumberPanels(
                    new List<Dictionary<string, int>>() {
                        PanelGenerator.ComvartToDictionary(panelNum: 1, initialTileNum: 4, finalTileNum: 4),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 2, initialTileNum: 5, finalTileNum: 5),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 3, initialTileNum: 6, finalTileNum: 6),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 4, initialTileNum: 7, finalTileNum: 7),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 5, initialTileNum: 8, finalTileNum: 8),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 6, initialTileNum: 9, finalTileNum: 9),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 7, initialTileNum: 10, finalTileNum: 10),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 8, initialTileNum: 14, finalTileNum: 11)
                    }
                    );
                    break;
                case 10:
                    // 銃弾実体生成
                    // ランダムな銃弾をランダムな引数で生成する
                    coroutines.Add(bulletGroupGenerator.CreateBulletGroup(
                            appearanceTime: 1.0f,
                            interval: 5.0f,
                            loop: true,
                    bulletGenerators: new List<GameObject>() {
                        bulletGroupGenerator.CreateRandomNormalCartridgeGenerator(ratio: 100,
                            randomCartridgeDirection: new int[] {10, 10, 10, 10},
                            randomRow: new int[] {10, 10, 10, 10, 10}, randomColumn: new int[] {10, 10, 10}),
                                       bulletGroupGenerator.CreateRandomTurnCartridgeGenerator(ratio: 100,
                                           randomCartridgedirection: new int[] {10, 10, 10, 10},
                                           randomRow: new int[] {10, 10, 10, 10, 10}, randomColumn: new int[] {10, 10, 10},
                                           randomTurnDirections: new int[] {10, 10, 10, 10}, randomTurnRow: new int[] {10, 10, 10},
                                           randomTurnColumn: new int[] {10, 10, 10, 10, 10}),
                                       bulletGroupGenerator.CreateRandomNormalHoleGenerator(ratio: 100,
                                           randomRow: new int[] {10, 10, 10, 10, 10}, randomColumn: new int[] {10, 10, 10}),
                                       bulletGroupGenerator.CreateRandomAimingHoleGenerator(ratio: 100,
                                           randomNumberPanels: new int[] {10, 10, 10, 10, 10, 10, 10, 10})
                    }));
                    /* 特殊タイル -> 数字パネル -> 特殊パネル */
                    tileGenerator.CreateNormalTiles();
                    // 数字パネル作成
                    panelGenerator.CreateNumberPanels(
                    new List<Dictionary<string, int>>() {
                        PanelGenerator.ComvartToDictionary(panelNum: 1, initialTileNum: 4, finalTileNum: 4),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 2, initialTileNum: 5, finalTileNum: 5),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 3, initialTileNum: 6, finalTileNum: 6),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 4, initialTileNum: 7, finalTileNum: 7),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 5, initialTileNum: 8, finalTileNum: 8),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 6, initialTileNum: 9, finalTileNum: 9),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 7, initialTileNum: 10, finalTileNum: 10),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 8, initialTileNum: 14, finalTileNum: 11)
                    }
                    );
                    // 特殊パネル作成
                    panelGenerator.CreateDynamicDummyPanel(initialTileNum: 3);
                    panelGenerator.CreateStaticDummyPanel(initialTileNum: 15);
                    break;
                case 1001:
                    // 銃弾実体生成
                    // 銃弾を生成しない
                    /* 特殊タイル -> 数字パネル -> 特殊パネル */
                    tileGenerator.CreateNormalTiles();
                    // 数字パネル作成
                    panelGenerator.CreateNumberPanels(
                    new List<Dictionary<string, int>>() {
                        PanelGenerator.ComvartToDictionary(panelNum: 1, initialTileNum: 4, finalTileNum: 4),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 2, initialTileNum: 5, finalTileNum: 5),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 3, initialTileNum: 6, finalTileNum: 6),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 4, initialTileNum: 7, finalTileNum: 7),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 5, initialTileNum: 8, finalTileNum: 8),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 6, initialTileNum: 9, finalTileNum: 9),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 7, initialTileNum: 10, finalTileNum: 10),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 8, initialTileNum: 14, finalTileNum: 11)
                    }
                    );
                    // 特殊パネル作成
                    panelGenerator.CreateDynamicDummyPanel(initialTileNum: 3);
                    panelGenerator.CreateStaticDummyPanel(initialTileNum: 15);
                    break;
                case 1002:
                    // ライフ付きパネル
                    // 銃弾実体生成
                    // ランダムな銃弾をランダムな引数で生成する
                    coroutines.Add(bulletGroupGenerator.CreateBulletGroup(
                            appearanceTime: 1.0f,
                            interval: 5.0f,
                            loop: true,
                    bulletGenerators: new List<GameObject>() {
                        bulletGroupGenerator.CreateNormalCartridgeGenerator(ratio: 10,
                            cartridgeDirection: ECartridgeDirection.Random, row: ERow.Random),
                                                bulletGroupGenerator.CreateTurnCartridgeGenerator(ratio: 10,
                                                    cartridgeDirection: ECartridgeDirection.Random, row: ERow.Random)
                    }));
                    /* 特殊タイル -> 数字パネル -> 特殊パネル */
                    tileGenerator.CreateNormalTiles();
                    // 数字パネル作成
                    panelGenerator.CreateLifeNumberPanels(
                    new List<Dictionary<string, int>>() {
                        PanelGenerator.ComvartToDictionary(panelNum: 1, initialTileNum: 4, finalTileNum: 4),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 2, initialTileNum: 5, finalTileNum: 5),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 3, initialTileNum: 6, finalTileNum: 6),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 4, initialTileNum: 7, finalTileNum: 7),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 5, initialTileNum: 8, finalTileNum: 8),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 6, initialTileNum: 9, finalTileNum: 9),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 7, initialTileNum: 10, finalTileNum: 10),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 8, initialTileNum: 14, finalTileNum: 11)
                    }
                    );
                    // 特殊パネル作成
                    panelGenerator.CreateDynamicDummyPanel(initialTileNum: 3);
                    panelGenerator.CreateStaticDummyPanel(initialTileNum: 15);
                    break;
                case 1003:
                    // ライフ付きパネル
                    // 銃弾実体生成
                    // ランダムな銃弾をランダムな引数で生成する
                    coroutines.Add(bulletGroupGenerator.CreateBulletGroup(
                            appearanceTime: 1.0f,
                            interval: 5.0f,
                            loop: true,
                    bulletGenerators: new List<GameObject>() {
                        bulletGroupGenerator.CreateRandomNormalHoleGenerator(10, new int[] {1, 1, 1, 1, 1}, new int[] {1, 1, 1}),
                                                                             bulletGroupGenerator.CreateRandomAimingHoleGenerator(10, new int[] {1, 1, 1, 1, 1, 1, 1, 1})
                    }));
                    /* 特殊タイル -> 数字パネル -> 特殊パネル */
                    tileGenerator.CreateNormalTiles();
                    // 数字パネル作成
                    panelGenerator.CreateLifeNumberPanels(
                    new List<Dictionary<string, int>>() {
                        PanelGenerator.ComvartToDictionary(panelNum: 1, initialTileNum: 4, finalTileNum: 4),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 2, initialTileNum: 5, finalTileNum: 5),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 3, initialTileNum: 6, finalTileNum: 6),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 4, initialTileNum: 7, finalTileNum: 7),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 5, initialTileNum: 8, finalTileNum: 8),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 6, initialTileNum: 9, finalTileNum: 9),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 7, initialTileNum: 10, finalTileNum: 10),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 8, initialTileNum: 14, finalTileNum: 11)
                    }
                    );
                    // 特殊パネル作成
                    panelGenerator.CreateDynamicDummyPanel(initialTileNum: 3);
                    panelGenerator.CreateStaticDummyPanel(initialTileNum: 15);
                    break;
                default:
                    throw new NotImplementedException();
            }

            // 銃弾の一括作成
            bulletGroupGenerator.CreateBulletGroups(coroutines);
        }
    }
}
