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
            }
            else
            {
                // 存在しないステージ
                Debug.LogError("Unable to create a stage whose stageId is " + stageId.ToString() + ".");
            }
        }
    }
}
