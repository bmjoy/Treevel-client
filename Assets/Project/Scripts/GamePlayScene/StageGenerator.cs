using System;
using Project.Scripts.GamePlayScene.Bullet.Generators;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils;
using UnityEngine;

namespace Project.Scripts.GamePlayScene
{
    public static class StageGenerator
    {
        /// <summary>
        /// ステージ作成が完了したかどうかのフラグ
        /// </summary>
        public static bool CreatedFinished
        {
            get;
            private set;
        }

        /// <summary>
        /// ステージを作成する
        /// </summary>
        /// <param name="stageId"> ステージ id </param>
        /// <exception cref="NotImplementedException"> 実装されていないステージ id を指定した場合 </exception>
        public static async void CreateStages(int stageId)
        {
            CreatedFinished = false;

            var tileGenerator = TileGenerator.Instance;
            var bulletGroupGenerator = BulletGroupGenerator.Instance;

            // ステージデータ読み込む
            var stageData = GameDataBase.GetStage(stageId);
            if (stageData != null) {
                // タイル生成
                tileGenerator.CreateTiles(stageData.TileDatas);

                // ボトル生成
                BottleGenerator.CreatePanels(stageData.PanelDatas);

                // 銃弾の初期化
                bulletGroupGenerator.CreateBulletGroups(stageData.BulletGroups);
            } else {
                // 存在しないステージ
                Debug.LogError("Unable to create a stage whose stageId is " + stageId.ToString() + ".");
            }

            CreatedFinished = true;
        }
    }
}
