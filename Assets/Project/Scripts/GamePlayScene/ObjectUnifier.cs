using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ObjectUnifier : MonoBehaviour
    {
        [Range(0, 1), Tooltip("画面お横幅を占める比率")]
        public float WindowWitdthRatio;

        [Tooltip("画像の横縦比")]
        public float ImageRatio;

        private void Awake()
        {
            var originalWidth = GetComponent<SpriteRenderer>().size.x;
            var originalHeight = GetComponent<SpriteRenderer>().size.y;

            var widthEfficient = WindowSize.WIDTH * WindowWitdthRatio;
            var heightEfficient = widthEfficient * ImageRatio ;
            transform.localScale = new Vector3(widthEfficient / originalWidth, heightEfficient / originalHeight, 1.0f);
        }
    }
}
