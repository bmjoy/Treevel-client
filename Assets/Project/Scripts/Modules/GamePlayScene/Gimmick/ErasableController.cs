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

        private void Awake()
        {
            GamePlayDirector.Instance.GameEnd
                .Subscribe(_ => Destroy(gameObject))
                .AddTo(this);
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

        private static async void InstantiateErasableBottle()
        {
            var erasableBottle = await AddressableAssetManager.Instantiate(Constants.Address.ERASABLE_BOTTLE_PREFAB);
            var erasableBottleController = erasableBottle.GetComponent<ErasableBottleController>();

            var puttableTilePosition = new List<(int, int)>();

            for (var col = 0; col < Constants.StageSize.COLUMN; ++col) {
                for (var row = 0; row < Constants.StageSize.ROW; ++row) {
                    if (BoardManager.Instance.IsNormalTile(col, row) && BoardManager.Instance.IsEmptyTile(col, row)) {
                        // 空いている NormalTile を候補とする
                        puttableTilePosition.Add((col, row));
                    }
                }
            }

            // 候補がない場合には生成しない
            if (puttableTilePosition.Count > 0) {
                // 完全にランダムで ErasableBottle を置く場所を決める
                var (column, row) = puttableTilePosition[Random.Range(0, puttableTilePosition.Count)];
                BoardManager.Instance.PutBottle(erasableBottleController, column, row);
            }
        }
    }
}
