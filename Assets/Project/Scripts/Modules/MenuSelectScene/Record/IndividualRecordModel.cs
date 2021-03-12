using System.Collections.Generic;
using System.Linq;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
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
        public readonly ReactiveProperty<List<StageStatus>> stageStatusList = new ReactiveProperty<List<StageStatus>>();

        public IndividualRecordModel()
        {
            currentSeason.Subscribe(season => {
                currentTree.Value = season.GetFirstTree();
            }).AddTo(_disposable);

            currentTree.Subscribe(tree => {
                FetchStageStatusList();
            }).AddTo(_disposable);
        }

        public void FetchStageStatusList()
        {
            stageStatusList.Value = GameDataManager.GetStages(currentTree.Value)
                .Select(stage => StageStatus.Get(stage.TreeId, stage.StageNumber))
                .ToList();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
