using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.GamePlayScene.Bullet.Generators;
using Project.Scripts.GamePlayScene.Panel;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils;
using UnityEngine;

namespace Project.Scripts.GamePlayScene
{
    public static class StageGenerator
    {
        /// <summary>
        /// ステージを作成する
        /// </summary>
        /// <param name="stageId"> ステージ id </param>
        /// <exception cref="NotImplementedException"> 実装されていないステージ id を指定した場合 </exception>
        public static async void CreateStages(int stageId)
        {
            var tileGenerator = TileGenerator.Instance;
            var bulletGroupGenerator = BulletGroupGenerator.Instance;

            // ステージデータ読み込む
            var stageData = GameDataBase.GetStage(stageId);
            if (stageData != null) {
                // タイル生成
                tileGenerator.CreateTiles(stageData.TileDatas);

                // パネル生成
                PanelGenerator.CreatePanels(stageData.PanelDatas);

                var coroutines = await bulletGroupGenerator.CreateBulletGroups(stageData.BulletGroups);

                // 銃弾一括生成
                bulletGroupGenerator.CreateBulletGroups(coroutines);
            } else {
                // 存在しないステージ
                Debug.LogError("Unable to create a stage whose stageId is " + stageId.ToString() + ".");
            }
        }
    }
}
