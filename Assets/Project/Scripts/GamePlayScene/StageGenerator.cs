using System;
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
                // パネルの作成はタイルに依存するため、タイルの生成が終わるまで待つ
                await tileGenerator.CreateTiles(stageData.TileDatas);

                // パネルの初期化
                PanelGenerator.CreatePanels(stageData.PanelDatas);

                // 銃弾の初期化
                bulletGroupGenerator.CreateBulletGroups(stageData.BulletGroups);
            } else {
                // 存在しないステージ
                Debug.LogError("Unable to create a stage whose stageId is " + stageId.ToString() + ".");
            }
        }
    }
}
