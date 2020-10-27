using System.Linq;
using Treevel.Common.Entities;
using UnityEngine;

namespace Treevel.Common.Utils
{
    public static class GimmickLibrary
    {
        /// <summary>
        /// 要素数を指定し、valueで初期化された配列を返す
        /// </summary>
        /// <param name="arrayLength"> 配列の要素数 </param>
        /// <returns> 初期化された配列 </returns>
        public static int[] GetInitialArray(int arrayLength, int value = 100)
        {
            return Enumerable.Repeat(arrayLength, value).ToArray();
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

        /// <summary>
        /// ギミックの移動方向は横向きか
        /// </summary>
        public static bool IsHorizontal(EDirection direction)
        {
            return direction == EDirection.ToRight || direction == EDirection.ToLeft;
        }

        /// <summary>
        /// ギミックの移動方向は縦向きか
        /// </summary>
        public static bool IsVertical(EDirection direction)
        {
            return direction == EDirection.ToUp || direction == EDirection.ToBottom;
        }
    }
}
