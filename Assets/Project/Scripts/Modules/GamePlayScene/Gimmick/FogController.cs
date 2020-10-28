using System.Collections;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Utils;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Gimmick
{
    public class FogController : AbstractGimmickController
    {
        /// <summary>
        /// 目標座標
        /// </summary>
        private Vector2 _targetPos;

        /// <summary>
        /// 持続時間
        /// </summary>
        private float _duration;

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);

            if (gimmickData.isRandom) {
                var row = GimmickLibrary.SamplingArrayIndex(gimmickData.randomRow.ToArray()) + 1;
                var column = GimmickLibrary.SamplingArrayIndex(gimmickData.randomColumn.ToArray()) + 1;
                _targetPos = BoardManager.Instance.GetTilePos(column - 1, row - 1);
            } else {
                _targetPos = BoardManager.Instance.GetTilePos((int)gimmickData.targetColumn - 1, (int)gimmickData.targetRow - 1);
            }
            _duration = gimmickData.duration;
        }

        public override IEnumerator Trigger()
        {
            yield return null;
        }

        protected override void OnEndGame()
        {
        }
    }
}
