using System;

namespace Treevel.Common.Entities
{
    public enum EBottleType
    {
        Dynamic,
        Static,
        Normal,
        AttackableDummy,
    }

    public static class BottleTypeExtension
    {
        public static bool IsAttackable(this EBottleType type)
        {
            switch (type) {
                case EBottleType.Dynamic:
                case EBottleType.Static:
                    return false;
                case EBottleType.Normal:
                case EBottleType.AttackableDummy:
                    return true;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
