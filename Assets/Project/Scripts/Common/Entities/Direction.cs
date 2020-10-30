using UnityEngine;

namespace Treevel.Common.Entities
{
    /// <summary>
    /// ギミックの移動方向
    /// </summary>
    public enum EDirection {
        ToLeft = 1,
        ToRight,
        ToUp,
        ToDown,
    }

    public static class DirectionExtension
    {
        /// <summary>
        /// `EDirection`の方向からベクターに変換する
        /// </summary>
        public static Vector2Int GetVectorInt(this EDirection direction)
        {
            switch (direction) {
                case EDirection.ToLeft:
                    return Vector2Int.left;
                case EDirection.ToRight:
                    return Vector2Int.right;
                case EDirection.ToUp:
                    return Vector2Int.up;
                case EDirection.ToDown:
                    return Vector2Int.down;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}
