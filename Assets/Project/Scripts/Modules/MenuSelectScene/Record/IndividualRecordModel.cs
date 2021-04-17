using System.Linq;
using Treevel.Common.Entities;
using Treevel.Common.Utils;
using UniRx;

namespace Treevel.Modules.MenuSelectScene.Record
{
    public class IndividualRecordModel
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        /// <summary>
        /// 現在の季節
        /// </summary>
        public readonly ReactiveProperty<ESeasonId> currentSeason = new ReactiveProperty<ESeasonId>(ESeasonId.Spring);

        /// <summary>
        /// 現在の木
        /// </summary>
        public readonly ReactiveProperty<ETreeId> currentTree = new ReactiveProperty<ETreeId>();

        /// <summary>
        /// 木に対応するステージの情報
        /// </summary>
        public readonly ReactiveProperty<StageStatus[]> stageStatusArray = new ReactiveProperty<StageStatus[]>();

        public IndividualRecordModel()
        {
            currentSeason.Subscribe(season => {
                currentTree.Value = season.GetFirstTree();
            }).AddTo(_disposable);

            currentTree.Subscribe(tree => {
                FetchStageStatusArray();
            }).AddTo(_disposable);
        }

        public void FetchStageStatusArray()
        {
            stageStatusArray.Value = StageStatusService.INSTANCE.Get(currentTree.Value).ToArray();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
