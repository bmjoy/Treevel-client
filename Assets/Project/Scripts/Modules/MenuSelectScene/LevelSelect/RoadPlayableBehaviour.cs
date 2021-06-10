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

        /// <summary>
        /// 演出中の拡大率
        /// </summary>
        private const float _SCALE_IN_ANIMATION = 1.5f;

        /// <summary>
        /// 演出前の拡大率
        /// </summary>
        private float _originalScale;

        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);
            _scaleContent = Object.FindObjectOfType<ScaleContent>();
            // 拡大させる
            _originalScale = _scaleContent.GetCurrentScale();
            _scaleContent.ScaleContents(_SCALE_IN_ANIMATION);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            base.OnPlayableDestroy(playable);
            // 拡大率を戻す
            _scaleContent.ScaleContents(_originalScale);
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
