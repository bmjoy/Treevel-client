using UnityEngine;
using UnityEngine.Playables;

namespace Treevel.Modules.MenuSelectScene.LevelSelect
{
    /// <summary>
    /// レベルセレクトシーンのロードを制御するためのカスタマイズトラックインスタンス
    /// </summary>
    public class RoadPlayableBehaviour : PlayableBehaviour
    {
        public RoadController roadController;

        private Material _material;

        private ScaleContent _scaleContent;

        /// <summary>
        /// 解放時のテクスチャの占領比率(1.0だと全解放)
        /// </summary>
        private static readonly int _SHADER_PARAM_FILL_AMOUNT = Shader.PropertyToID("_fillAmount");

        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);
            _scaleContent = Object.FindObjectOfType<ScaleContent>();
        }

        // フレーム毎の処理
        // 非ランタイムでも呼ばれるので注意！
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (_material == null && roadController != null) {
                _material = roadController.GetComponent<LineRenderer>().sharedMaterial;
            }
            if (_material == null) {
                return;
            }
            var progress = (float)(playable.GetTime() / playable.GetDuration());

            _material.SetFloat(_SHADER_PARAM_FILL_AMOUNT, Mathf.Lerp(0, 1, progress));
            // 道の色が変化するところを追従する
            _scaleContent.FocusAtScreenPosition(roadController.GetPositionAtRatio(progress));
        }
    }
}
