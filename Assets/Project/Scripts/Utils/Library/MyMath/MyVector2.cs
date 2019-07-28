using UnityEngine;
using System;

namespace Project.Scripts.Utils.Library.MyMath
{
    public static class MyVector2
    {
        /// <summary>
        /// 要素の絶対値によるベクトルを返す
        /// </summary>
        /// <param name="v"> 対象ベクトル </param>
        /// <returns> 要素の絶対値によるベクトル </returns>
        public static Vector2 Abs(Vector2 v)
        {
            return new Vector2(Math.Abs(v.x), Math.Abs(v.y));
        }

        /// <summary>
        /// ベクトルの転置を返す
        /// </summary>
        /// <param name="v"> 対象ベクトル </param>
        /// <returns>　転置後のベクトル </returns>
        public static Vector2 Transposition(Vector2 v)
        {
            return new Vector2(v.y, v.x);
        }

        /// <summary>
        /// ベクトルを原点中心で回転させる
        /// </summary>
        /// <param name="v"> 対象ベクトル </param>
        /// <param name="angle"> 回転角 </param>
        /// <returns> 回転後のベクトル </returns>
        public static Vector2 Rotate(Vector2 v, float angle)
        {
            return new Vector2((float)(Math.Cos(angle) * v.x - Math.Sin(angle) * v.y),
                               (float)(Math.Sin(angle) * v.x + Math.Cos(angle) * v.y));
        }
    }
}