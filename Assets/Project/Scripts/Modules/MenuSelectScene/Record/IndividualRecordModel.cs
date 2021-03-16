using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Common.Networks;
using Treevel.Common.Networks.Requests;
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

        public async void FetchStageStatusArray()
        {
            var tasks = GameDataManager.GetStages(currentTree.Value)
                .Select(stage => NetworkService.Execute(new GetStageStatusRequest(stage.TreeId, stage.StageNumber)));

            stageStatusArray.Value = await UniTask.WhenAll(tasks);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
