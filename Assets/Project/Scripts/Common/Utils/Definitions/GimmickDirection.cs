using UnityEngine;

namespace Project.Scripts.Utils.Definitions
{
    /// <summary>
    /// ギミックの移動方向
    /// </summary>
    public enum EGimmickDirection {
        ToLeft = 1,
        ToRight,
        ToUp,
        ToBottom,
        Random = -1
    }

    public static class GimmickDirectionExtension
    {
        /// <summary>
        /// `EGimmickDirection`の方向からベクターに変換する
        /// </summary>
        public static Vector2Int GetDirectionVector(this EGimmickDirection direction)
        {
            switch (direction) {
                case EGimmickDirection.ToLeft:
                    return Vector2Int.left;
                case EGimmickDirection.ToRight:
                    return Vector2Int.right;
                case EGimmickDirection.ToUp:
                    return Vector2Int.up;
                case EGimmickDirection.ToBottom:
                    return Vector2Int.down;
                case EGimmickDirection.Random:
                default:
                    throw new System.NotImplementedException();
            }
        }

    }
}
