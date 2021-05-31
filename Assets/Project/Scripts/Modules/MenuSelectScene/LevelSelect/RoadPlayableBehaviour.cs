using UnityEngine;
using UnityEngine.Playables;

namespace Treevel.Modules.MenuSelectScene.LevelSelect
{
    /// <summary>
    /// レベルセレクトシーンのロードを制御するためのカスタマイズトラックインスタンス
    /// </summary>
    public class RoadPlayableBehaviour : PlayableBehaviour
    {
        public RoadController RoadController { get; set; }

        private Material _material;

        /// <summary>
        /// 解放時のテクスチャの占領比率(1.0だと全解放)
        /// </summary>
        private static readonly int _SHADER_PARAM_FILL_AMOUNT = Shader.PropertyToID("_fillAmount");

        // フレーム毎の処理
        // 非ランタイムでも呼ばれるので注意！
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (_material == null && RoadController != null) {
                _material = RoadController.GetComponent<LineRenderer>().sharedMaterial;
            }
            if (_material == null) {
                return;
            }
            var progress = (float)(playable.GetTime() / playable.GetDuration());

            _material.SetFloat(_SHADER_PARAM_FILL_AMOUNT, Mathf.Lerp(0, 1, progress));
        }
    }
}
