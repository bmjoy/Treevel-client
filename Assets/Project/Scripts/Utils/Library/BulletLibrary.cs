using UnityEngine;
using System;
using System.Linq;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.Utils.Library
{
    public static class BulletLibrary
    {
        /// <summary>
        /// 座標から行(row)と列(column)を返す
        /// </summary>
        /// <param name="position"> 座標 </param>
        public static(int, int) GetRowAndColumn(Vector2 position)
        {
            // 最上タイルのy座標
            const float topTilePositionY = WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f);
            int row = (int)Math.Round((topTilePositionY - position.y) / TileSize.HEIGHT, MidpointRounding.AwayFromZero) + 1;
            int column = (int)Math.Round((position.x / TileSize.WIDTH) + 1, MidpointRounding.AwayFromZero) + 1;
            return (row, column);
        }

        /// <summary>
        /// 要素数を指定し、BulletGeneratorPrameter.INITIAL_RATIOで初期化された配列を返す
        /// </summary>
        /// <param name="arrayLength"> 配列の要素数 </param>
        /// <returns> 初期化された配列 </returns>
        public static int[] GetInitialArray(int arrayLength)
        {
            var returnArray = new int[arrayLength];

            for (var index = 0; index < arrayLength; index++) {
                returnArray[index] = BulletGeneratorParameter.INITIAL_RATIO;
            }

            return returnArray;
        }

        /// <summary>
        /// 確率分布を表した配列に基づき、配列の、あるインデックスを返す
        /// (配列の最初であるならば0を返す)
        /// </summary>
        /// <param name="probabilityArray"> 確率分布を表した配列 </param>
        /// <returns>　サンプリングされたインデックス </returns>
        public static int SamplingArrayIndex(int[] probabilityArray)
        {
            var sumOfProbability = probabilityArray.Sum();
            // 1以上重みの総和以下の値をランダムに取得する
            var randomValue = new System.Random().Next(sumOfProbability) + 1;
            var index = 0;

            // 重み配列の最初の要素から順に、ランダムな値から値を引く
            while (randomValue > 0) {
                randomValue -= probabilityArray[index];
                index += 1;
            }

            return index - 1;
        }
    }
}
