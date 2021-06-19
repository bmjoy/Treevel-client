using Treevel.Common.Managers;
using UnityEngine;

namespace Treevel.Common.Components
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PlaySEParticle : MonoBehaviour
    {
        /// <summary>
        /// 1フレーム前のParticleの個数
        /// 初期値に0以外を代入することはないので明示的に0を入れる
        /// </summary>
        private int _preParticleCount = 0;

        private ParticleSystem _particleSystem;

        /// <summary>
        /// 再生するSEのキー
        /// </summary>
        [SerializeField] private ESEKey _soundKey;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            var particleCount = _particleSystem.particleCount;
            if (particleCount > _preParticleCount && _preParticleCount == 0) {
                // particleが生成された
                SoundManager.Instance.PlaySE(_soundKey);
            }

            _preParticleCount = particleCount;
        }
    }
}
