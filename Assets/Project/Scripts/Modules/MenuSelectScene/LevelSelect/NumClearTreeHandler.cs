using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Networks;
using Treevel.Common.Networks.Requests;

namespace Treevel.Modules.MenuSelectScene.LevelSelect
{
    /// <summary>
    /// クリアしているステージの割合がクリア条件
    /// </summary>
    public class NumClearTreeHandler : IClearTreeHandler
    {
        /// <summary>
        /// 木のID
        /// </summary>
        private readonly ETreeId _treeId;

        /// <summary>
        /// クリアに必要なステージ数
        /// </summary>
        private readonly int _clearNumThreshold;

        /// <summary>
        /// 木に存在するステージ数
        /// </summary>
        private readonly int _stageNum;

        /// <summary>
        /// ステージ情報
        /// </summary>
        private StageStatus[] _stageStatuses;

        /// <summary>
        /// クリアに必要なステージ数を設定するコンストラクタ
        /// </summary>
        /// <param name="treeId"> 木のID(ステージ数を取得) </param>
        /// <param name="clearThreshold"> クリアに必要なステージ数 </param>
        public NumClearTreeHandler(ETreeId treeId, int clearThreshold)
        {
            _treeId = treeId;
            _clearNumThreshold = clearThreshold;
            _stageNum = _treeId.GetStageNum();
            if (clearThreshold < 1) {
                throw new Exception($"clearThreshold(={clearThreshold}) must not be less than 1");
            }

            if (clearThreshold > _stageNum) {
                throw new Exception($"clearThreshold(={clearThreshold}) must not be larger than the number of stages");
            }

            // コンストラクタは async にできないので、関数に分離
            InitializeAsync().Forget();
        }

        private async UniTask InitializeAsync()
        {
            // FIXME: GetTreeState の時に値が入っている保証がない
            var tasks = Enumerable.Range(1, _stageNum)
                // FIXME: 呼ばれるたびに ステージ数 分リクエストしてしまうので、リクエストを減らす工夫をする
                .Select(s => NetworkService.Execute(new GetStageStatusRequest(_treeId, s)));
            _stageStatuses = await UniTask.WhenAll(tasks);
        }

        /// <summary>
        /// ステージのクリア数に応じた木の状態を取得する
        /// </summary>
        /// <returns> 木の状態 </returns>
        public ETreeState GetTreeState()
        {
            var clearStageNum = _stageStatuses
                .Count(status => status.state == EStageState.Cleared);

            // クリア数に応じた木の状態を返す
            if (clearStageNum == _stageNum) {
                return ETreeState.AllCleared;
            } else if (clearStageNum >= _clearNumThreshold) {
                return ETreeState.Cleared;
            } else {
                return ETreeState.Released;
            }
        }
    }
}
