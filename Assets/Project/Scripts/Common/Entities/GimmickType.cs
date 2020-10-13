using System;

namespace Treevel.Common.Entities
{
    /// <summary>
    /// ギミックの種類
    /// </summary>
    public enum EGimmickType {
        Tornado,
        Meteorite,
        AimingMeteorite,
        Thunder,
        SolarBeam,
        GustWind
    }

    public static class GimmickTypeExtension
    {
        public static EFailureReasonType GetFailureReason(this EGimmickType type)
        {
            switch (type) {
                case EGimmickType.Tornado:
                    return EFailureReasonType.Tornado;
                case EGimmickType.Meteorite:
                    return EFailureReasonType.Meteorite;
                case EGimmickType.AimingMeteorite:
                    return EFailureReasonType.AimingMeteorite;
                case EGimmickType.Thunder:
                    return EFailureReasonType.Thunder;
                case EGimmickType.SolarBeam:
                    return EFailureReasonType.SolarBeam;
                case EGimmickType.GustWind:
                    return EFailureReasonType.GustWind;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
