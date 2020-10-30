using System.Collections;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Utils;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Gimmick
{
    [RequireComponent(typeof(ParticleSystem))]
    public class FogController : AbstractGimmickController
    {
        /// <summary>
        /// パーティクルシステム
        /// </summary>
        private ParticleSystem _particleSystem;
        private ParticleSystem.MainModule _mainModule;

        /// <summary>
        /// 目標座標
        /// </summary>
        private Vector2 _targetPos;

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);

            _particleSystem = GetComponent<ParticleSystem>();
            _mainModule = _particleSystem.main;

            // 全てのGimmickの中で最も手前に描画する
            GetComponent<Renderer>().sortingOrder = short.MaxValue;

            if (gimmickData.isRandom) {
                var row = GimmickLibrary.SamplingArrayIndex(gimmickData.randomRow.ToArray()) + 1;
                var column = GimmickLibrary.SamplingArrayIndex(gimmickData.randomColumn.ToArray()) + 1;
                _targetPos = BoardManager.Instance.GetTilePos(column - 1, row - 1);
            } else {
                _targetPos = BoardManager.Instance.GetTilePos((int)gimmickData.targetColumn - 1, (int)gimmickData.targetRow - 1);
            }
            transform.position = _targetPos;
            _mainModule.duration = gimmickData.duration;
        }

        public override IEnumerator Trigger()
        {
            _particleSystem.Play();
            yield return null;
        }

        protected override void OnEndGame()
        {
            Destroy(gameObject);
        }
    }
}
