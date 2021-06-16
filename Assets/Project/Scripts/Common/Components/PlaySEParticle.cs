using Treevel.Common.Managers;
using UnityEngine;

namespace Treevel.Common.Components
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PlaySEParticle : MonoBehaviour
    {
        /// <summary>
        /// 1フレーム前のParticleの個数
        /// </summary>
        private int _preParticleCount;

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
