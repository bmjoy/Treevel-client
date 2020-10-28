using System;
using UnityEngine;

namespace Treevel.Common.Entities
{
    public enum EFailureReasonType {
        Others,
        Tornado,
        Meteorite,
        AimingMeteorite,
        Thunder,
        SolarBeam,
        GustWind,
        Fog,
    }

    public static class EFailureReasonTypeExtension
    {
        public static Color GetColor(this EFailureReasonType type)
        {
            switch (type) {
                case EFailureReasonType.Others:
                    return Color.gray;
                case EFailureReasonType.Tornado:
                    return Color.green;
                case EFailureReasonType.Meteorite:
                    return Color.red;
                case EFailureReasonType.AimingMeteorite:
                    return Color.red;
                case EFailureReasonType.Thunder:
                    return Color.yellow;
                case EFailureReasonType.SolarBeam:
                    return Color.magenta;
                case EFailureReasonType.GustWind:
                    return Color.white;
                case EFailureReasonType.Fog:
                    return Color.white;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
