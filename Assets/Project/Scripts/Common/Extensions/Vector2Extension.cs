using System;
using UnityEngine;

namespace Treevel.Common.Extensions
{
    public static class Vector2Extension
    {
        /// <summary>
        /// 要素の絶対値によるベクトルを返す
        /// </summary>
        /// <param name="v"> 対象ベクトル </param>
        /// <returns> 要素の絶対値によるベクトル </returns>
        public static Vector2 Abs(this Vector2 v)
        {
            return new Vector2(Math.Abs(v.x), Math.Abs(v.y));
        }

        /// <summary>
        /// ベクトルの転置を返す
        /// </summary>
        /// <param name="v"> 対象ベクトル </param>
        /// <returns>　転置後のベクトル </returns>
        public static Vector2 Transposition(this Vector2 v)
        {
            return new Vector2(v.y, v.x);
        }

        /// <summary>
        /// ベクトルを原点中心で回転させる
        /// </summary>
        /// <param name="v"> 対象ベクトル </param>
        /// <param name="angle"> 回転角 </param>
        /// <returns> 回転後のベクトル </returns>
        public static Vector2 Rotate(this Vector2 v, float angle)
        {
            return new Vector2((float)(Math.Cos(angle) * v.x - Math.Sin(angle) * v.y),
                    (float)(Math.Sin(angle) * v.x + Math.Cos(angle) * v.y));
        }

        /// <summary>
        /// 任意のベクトルを上下左右の四方向に正規化する
        /// </summary>
        /// <param name="v"> 対象ベクトル </param>
        /// <returns>四方向の単位ベクトル</returns>
        public static Vector2 NormalizeDirection(this Vector2 v)
        {
            if (Math.Abs(v.x) > Math.Abs(v.y)) {
                // x方向が強い
                return v.x >= 0 ? Vector2.right : Vector2.left;
            } else {
                // y方向が強い
                return v.y >= 0 ? Vector2.up : Vector2.down;
            }
        }
    }
}
