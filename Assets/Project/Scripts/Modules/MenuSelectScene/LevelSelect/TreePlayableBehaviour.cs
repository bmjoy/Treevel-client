using UnityEngine;
using UnityEngine.Playables;

namespace Treevel.Modules.MenuSelectScene.LevelSelect
{
    /// <summary>
    /// レベルセレクトシーンのロードを制御するためのカスタマイズトラックインスタンス
    /// </summary>
    public class TreePlayableBehaviour : PlayableBehaviour
    {
        /// <summary>
        /// 解放対象となる木のインスタンス
        /// </summary>
        public LevelTreeController releasedTree;

        /// <summary>
        /// 解放演出で使うパーティクルシステムコンポーネント
        /// </summary>
        private ParticleSystem _particle;

        /// <summary>
        /// 解放ステートが反映されたか
        /// </summary>
        private bool _isReflectedTreeState;

        private ScaleContent _scaleContent;

        /// <summary>
        /// ScriptPlayable<TreePlayableBehaviour>.Create(graph); 中に呼ばれる。コンストラクタみたいなもの
        /// </summary>
        public override void OnPlayableCreate(Playable playable)
        {
            _scaleContent = Object.FindObjectOfType<ScaleContent>();
        }

        /// <summary>
        /// TreePlayableAsset.CreatePlayable()を抜けた後に呼ばれる、
        /// Referenceが解決した後なのでreleasedTree関連する処理はここで書く（OnPlayableCreateではエラーになる)
        /// </summary>
        public override void OnGraphStart(Playable playable)
        {
            if (!releasedTree) {
                return;
            }
            var fieldObject = releasedTree.transform.Find("Field");
            if (!fieldObject) {
                return;
            }

            _particle = fieldObject.transform.GetChild(0).GetComponent<ParticleSystem>();
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (_isReflectedTreeState) return;

            var progress = (float)(playable.GetTime() / playable.GetDuration());
            if (progress > 0.5f) {
                releasedTree.ReflectTreeState();
                _isReflectedTreeState = true;
            }
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            // 道の解放の最後で木が見切れてしまうため一回視点を修正する
            _scaleContent.FocusAtScreenPosition(releasedTree.GetTreeWorldPosition());
            _particle.gameObject.SetActive(true);
            _particle.Play();
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            _particle.Stop();
            _particle.gameObject.SetActive(false);
        }
    }
}
