using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.GamePlayScene.Bullet;
using Project.Scripts.GamePlayScene.Panel;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene
{
    public class StageGenerator : MonoBehaviour
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

            switch (stageId) {
                case 1:
                    // 銃弾実体生成
                    // 銃弾を生成しない
                    /* 特殊タイル -> 数字パネル -> 特殊パネル */
                    // 数字パネル作成
                    panelGenerator.PrepareTilesAndCreateNumberPanels(
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
                case 2:
                    // 銃弾実体生成
                    // NormalCartridgeを生成する
                    coroutines.Add(bulletGroupGenerator.CreateBulletGroup(
                            appearanceTime: 3.0f,
                            interval: 5.0f,
                            loop: true,
                    bulletGenerators: new List<GameObject>() {
                        bulletGroupGenerator.CreateNormalCartridgeGenerator(ratio: 100,
                            cartridgeDirection: ECartridgeDirection.ToLeft, row: ERow.First)
                    }));
                    /* 特殊タイル -> 数字パネル -> 特殊パネル */
                    // 特殊タイル作成
                    tileGenerator.CreateWarpTiles(firstTileNum: 2, secondTileNum: 14);
                    // 数字パネル作成
                    panelGenerator.PrepareTilesAndCreateNumberPanels(
                    new List<Dictionary<string, int>>() {
                        PanelGenerator.ComvartToDictionary(panelNum: 1, initialTileNum: 1, finalTileNum: 4),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 2, initialTileNum: 3, finalTileNum: 5),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 3, initialTileNum: 5, finalTileNum: 6),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 4, initialTileNum: 6, finalTileNum: 7),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 5, initialTileNum: 8, finalTileNum: 8),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 6, initialTileNum: 11, finalTileNum: 9),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 7, initialTileNum: 13, finalTileNum: 10),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 8, initialTileNum: 15, finalTileNum: 11)
                    }
                    );
                    break;
                case 3:
                    // 銃弾実体生成
                    // TurnCartridgeを生成する
                    coroutines.Add(bulletGroupGenerator.CreateBulletGroup(
                            appearanceTime: 1.0f,
                            interval: 5.0f,
                            loop: true,
                    bulletGenerators: new List<GameObject>() {
                        bulletGroupGenerator.CreateTurnCartridgeGenerator(ratio: 100,
                            cartridgeDirection: ECartridgeDirection.ToLeft, row: ERow.First,
                            turnDirection: new int[] {(int) ECartridgeDirection.ToBottom},
                            turnLine: new int[] {(int) EColumn.Left})
                    }));
                    /* 特殊タイル -> 数字パネル -> 特殊パネル */
                    // 特殊タイル作成
                    tileGenerator.CreateWarpTiles(firstTileNum: 2, secondTileNum: 14);
                    // 数字パネル作成
                    panelGenerator.PrepareTilesAndCreateNumberPanels(
                    new List<Dictionary<string, int>>() {
                        PanelGenerator.ComvartToDictionary(panelNum: 1, initialTileNum: 1, finalTileNum: 4),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 2, initialTileNum: 3, finalTileNum: 5),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 3, initialTileNum: 5, finalTileNum: 6),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 4, initialTileNum: 6, finalTileNum: 7),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 5, initialTileNum: 8, finalTileNum: 8),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 6, initialTileNum: 11, finalTileNum: 9),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 7, initialTileNum: 13, finalTileNum: 10),
                                                           PanelGenerator.ComvartToDictionary(panelNum: 8, initialTileNum: 15, finalTileNum: 11)
                    }
                    );
                    break;
                case 4:
                    // 銃弾実体生成
                    // NormalHoleを生成する
                    coroutines.Add(bulletGroupGenerator.CreateBulletGroup(
                            appearanceTime: 1.0f,
                            interval: 5.0f,
                            loop: true,
                    bulletGenerators: new List<GameObject>() {
                        bulletGroupGenerator.CreateNormalHoleGenerator(ratio: 100, row: ERow.First,
                            column: EColumn.Left)
                    }));
                    /* 特殊タイル -> 数字パネル -> 特殊パネル */
                    // 数字パネル作成
                    panelGenerator.PrepareTilesAndCreateNumberPanels(
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
                case 5:
                    // 銃弾実体生成
                    // AimingHoleを生成する
                    coroutines.Add(bulletGroupGenerator.CreateBulletGroup(
                            appearanceTime: 1.0f,
                            interval: 5.0f,
                            loop: true,
                    bulletGenerators: new List<GameObject>() {
                        bulletGroupGenerator.CreateAimingHoleGenerator(ratio: 100, aimingPanel: new int[] {1})
                    }));
                    /* 特殊タイル -> 数字パネル -> 特殊パネル */
                    // 数字パネル作成
                    panelGenerator.PrepareTilesAndCreateNumberPanels(
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
                                                bulletGroupGenerator.CreateNormalCartridgeGenerator(ratio: 100,
                                                    cartridgeDirection: ECartridgeDirection.Random, row: ERow.Random,
                                                    randomCartridgeDirection: new int[] {200, 10, 100, 0},
                                                    randomRow: new int[] {100, 5, 5, 5, 100}, randomColumn: new int[] {100, 10, 0}),
                    }));
                    /* 特殊タイル -> 数字パネル -> 特殊パネル */
                    // 数字パネル作成
                    panelGenerator.PrepareTilesAndCreateNumberPanels(
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
                case 7:
                    // 銃弾実体生成
                    // ランダムな引数でTurnCartridgeを生成する
                    coroutines.Add(bulletGroupGenerator.CreateBulletGroup(
                            appearanceTime: 1.0f,
                            interval: 5.0f,
                            loop: true,
                    bulletGenerators: new List<GameObject>() {
                        bulletGroupGenerator.CreateTurnCartridgeGenerator(ratio: 100,
                            cartridgeDirection: ECartridgeDirection.Random, row: ERow.Random),
                                                bulletGroupGenerator.CreateTurnCartridgeGenerator(ratio: 100,
                                                    cartridgeDirection: ECartridgeDirection.Random, row: ERow.Random,
                                                    turnDirection: null, turnLine: null,
                                                    randomCartridgedirection: new int[] {10, 10, 10, 10},
                                                    randomRow: new int[] {1, 2, 3, 4, 5}, randomColumn: new int[] {100, 0, 100},
                                                    randomTurnDirections: new int[] {100, 0, 100, 0}, randomTurnRow: new int[] {1, 0, 0},
                                                    randomTurnColumn: new int[] {10, 10, 10, 10, 10})
                    }));
                    /* 特殊タイル -> 数字パネル -> 特殊パネル */
                    // 数字パネル作成
                    panelGenerator.PrepareTilesAndCreateNumberPanels(
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
                case 8:
                    // 銃弾実体生成
                    // ランダムな引数でNormalHoleを生成する
                    coroutines.Add(bulletGroupGenerator.CreateBulletGroup(
                            appearanceTime: 1.0f,
                            interval: 5.0f,
                            loop: true,
                    bulletGenerators: new List<GameObject>() {
                        bulletGroupGenerator.CreateNormalHoleGenerator(ratio: 100, row: ERow.Random,
                            column: EColumn.Random),
                                    bulletGroupGenerator.CreateNormalHoleGenerator(ratio: 100, row: ERow.Random,
                                        column: EColumn.Random,
                                        randomRow: new int[] {100, 20, 20, 20, 100}, randomColumn: new int[] {30, 100, 30})
                    }));
                    /* 特殊タイル -> 数字パネル -> 特殊パネル */
                    // 数字パネル作成
                    panelGenerator.PrepareTilesAndCreateNumberPanels(
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
                case 9:
                    // 銃弾実体生成
                    // ランダムな引数でAimingHoleを生成する
                    coroutines.Add(bulletGroupGenerator.CreateBulletGroup(
                            appearanceTime: 1.0f,
                            interval: 5.0f,
                            loop: true,
                    bulletGenerators: new List<GameObject>() {
                        bulletGroupGenerator.CreateAimingHoleGenerator(ratio: 100, aimingPanel: null),
                                                                       bulletGroupGenerator.CreateAimingHoleGenerator(ratio: 100, aimingPanel: null,
                                                                               randomNumberPanel: new int[] {10, 0, 10, 0, 10, 0, 10, 10})
                    }));
                    /* 特殊タイル -> 数字パネル -> 特殊パネル */
                    // 数字パネル作成
                    panelGenerator.PrepareTilesAndCreateNumberPanels(
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
                        bulletGroupGenerator.CreateNormalCartridgeGenerator(ratio: 10,
                            cartridgeDirection: ECartridgeDirection.Random, row: ERow.Random),
                                                bulletGroupGenerator.CreateTurnCartridgeGenerator(ratio: 10,
                                                    cartridgeDirection: ECartridgeDirection.Random, row: ERow.Random),
                                                bulletGroupGenerator.CreateNormalHoleGenerator(ratio: 10, row: ERow.Random,
                                                    column: EColumn.Random),
                                                bulletGroupGenerator.CreateAimingHoleGenerator(ratio: 10)
                    }));
                    /* 特殊タイル -> 数字パネル -> 特殊パネル */
                    // 数字パネル作成
                    panelGenerator.PrepareTilesAndCreateNumberPanels(
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
                    // 数字パネル作成
                    panelGenerator.PrepareTilesAndCreateNumberPanels(
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
