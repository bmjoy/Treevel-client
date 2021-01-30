using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Gimmick
{
    public class ErasableController : AbstractGimmickController
    {
        /// <summary>
        /// ErasableBottle を生成する間隔（秒数）
        /// </summary>
        private const int _GENERATING_INTERVAL = 3;

        /// <summary>
        /// normalTile の位置リスト
        /// </summary>
        private List<(int, int)> _normalTilePositions = new List<(int, int)>();

        private void Awake()
        {
            GamePlayDirector.Instance.GameEnd
                .Subscribe(_ => Destroy(gameObject))
                .AddTo(this);

            // 現状、normalTile の位置が後から変わることはないので、初めに取得しておく
            for (var col = 0; col < Constants.StageSize.COLUMN; ++col) {
                for (var row = 0; row < Constants.StageSize.ROW; ++row) {
                    if (BoardManager.Instance.IsNormalTile(col, row)) _normalTilePositions.Add((col, row));
                }
            }
        }

        public override IEnumerator Trigger()
        {
            while (gameObject != null) {
                // 生成を一定時間待つ
                yield return new WaitForSeconds(_GENERATING_INTERVAL);

                // ErasableBottle を生成する
                InstantiateErasableBottle();
            }
        }

        private async void InstantiateErasableBottle()
        {
            var puttableTilePositions = new List<(int, int)>();

            foreach (var normalTilePosition in _normalTilePositions) {
                var (column, row) = normalTilePosition;

                if (BoardManager.Instance.IsEmptyTile(column, row)) {
                    // 空いている NormalTile を候補とする
                    puttableTilePositions.Add((column, row));
                }
            }

            // 候補がない場合には生成しない
            if (puttableTilePositions.Count > 0) {
                var erasableBottle = await AddressableAssetManager.Instantiate(Constants.Address.ERASABLE_BOTTLE_PREFAB);
                var erasableBottleController = erasableBottle.GetComponent<ErasableBottleController>();

                // 完全にランダムで ErasableBottle を置く場所を決める
                var (column, row) = puttableTilePositions[Random.Range(0, puttableTilePositions.Count)];
                BoardManager.Instance.PutBottle(erasableBottleController, column, row);
            }
        }
    }
}
