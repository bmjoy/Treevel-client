using System.Collections;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Utils;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Gimmick
{
    [RequireComponent(typeof(ParticleSystem))]
    public class FogController : GimmickControllerBase
    {
        public const int WIDTH_MAX = 2;
        public const int HEIGHT_MAX = 2;

        /// <summary>
        /// パーティクルシステム
        /// </summary>
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            GamePlayDirector.Instance.GameEnd.Subscribe(_ => Destroy(gameObject)).AddTo(this);
        }

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);

            // 全てのGimmickの中で最も手前に描画する
            GetComponent<Renderer>().sortingOrder = short.MaxValue;

            _particleSystem = GetComponent<ParticleSystem>();
            // サイズ
            var width = gimmickData.width;
            var height = gimmickData.height;
            // 中心位置
            int row;
            int column;
            if (gimmickData.isRandom) {
                row = GimmickLibrary.SamplingArrayIndex(gimmickData.randomRow.ToArray());
                column = GimmickLibrary.SamplingArrayIndex(gimmickData.randomColumn.ToArray());
            } else {
                row = (int)gimmickData.targetRow;
                column = (int)gimmickData.targetColumn;
            }

            var leftTopPos = BoardManager.Instance.GetTilePos(column, row);
            var rightBottomPos = BoardManager.Instance.GetTilePos(column + width - 1, row + height - 1);
            transform.position = (leftTopPos + rightBottomPos) / 2;

            var mainModule = _particleSystem.main;
            // 持続時間
            mainModule.duration = gimmickData.duration;
            // 最大粒子量
            mainModule.maxParticles *= width * height;
            // 1秒あたりの放出粒子量
            var emissionModule = _particleSystem.emission;
            emissionModule.rateOverTimeMultiplier *= width * height;
            // 倍率
            var shapeModule = _particleSystem.shape;
            var scale = shapeModule.scale;
            shapeModule.scale *= new Vector2(width, height);
        }

        public override IEnumerator Trigger()
        {
            _particleSystem.Play();
            yield return null;
        }
    }
}
