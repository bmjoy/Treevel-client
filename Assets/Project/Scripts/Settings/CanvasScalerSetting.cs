using UnityEngine;

namespace Project.Scripts.Settings
{
    public class CanvasScalerSetting : ScriptableObject
    {
        [SerializeField] private Vector2 referenceResolution;

        [SerializeField] private float matchWidthOrHeight;

        public Vector2 ReferenceResolution => referenceResolution;

        public float MatchWidthOrHeight => matchWidthOrHeight;
    }
}
