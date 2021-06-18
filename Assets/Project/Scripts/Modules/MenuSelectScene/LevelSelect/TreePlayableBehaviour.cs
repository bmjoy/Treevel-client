using UnityEngine;
using UnityEngine.Playables;

namespace Treevel.Modules.MenuSelectScene.LevelSelect
{
    /// <summary>
    /// レベルセレクトシーンのロードを制御するためのカスタマイズトラックインスタンス
    /// </summary>
    public class TreePlayableBehaviour : PlayableBehaviour
    {
        public LevelTreeController releasedTree;

        private Material _material;

        private ParticleSystem _particle;

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

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
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
