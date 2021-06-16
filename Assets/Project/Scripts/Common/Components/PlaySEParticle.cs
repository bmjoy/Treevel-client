using Treevel.Common.Managers;
using UnityEngine;

namespace Treevel.Common.Components
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PlaySEParticle : MonoBehaviour
    {
        private int _particleNum;

        private ParticleSystem _particleSystem;

        [SerializeField] private ESEKey _soundKey;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            var count = _particleSystem.particleCount;
            if (count > _particleNum && _particleNum == 0) {
                // particleが生成された
                SoundManager.Instance.PlaySE(_soundKey);
            }

            _particleNum = count;
        }
    }
}
