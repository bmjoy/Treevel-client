using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Common.Networks;
using Treevel.Common.Networks.Requests;
using UniRx;

namespace Treevel.Modules.MenuSelectScene.Record
{
    public class GeneralRecordModel
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        /// <summary>
        /// ステージの情報
        /// </summary>
        public readonly ReactiveProperty<StageStatus[]> stageStatusArray = new ReactiveProperty<StageStatus[]>();

        /// <summary>
        /// 起動日数
        /// </summary>
        public readonly ReactiveProperty<int> startupDays = new ReactiveProperty<int>();

        /// <summary>
        /// 各ギミックの失敗回数
        /// </summary>
        public readonly ReactiveProperty<Dictionary<EFailureReasonType, int>> failureReasonCount = new ReactiveProperty<Dictionary<EFailureReasonType, int>>();

        public GeneralRecordModel()
        {
            FetchStageStatusArrayAsync().Forget();

            RecordData.Instance.StartupDaysObservable
                .Subscribe(startupDays => this.startupDays.Value = startupDays)
                .AddTo(_disposable);

            RecordData.Instance.failureReasonCount
                .Subscribe(failureReasonCount => this.failureReasonCount.Value = failureReasonCount)
                .AddTo(_disposable);
        }

        public async UniTask FetchStageStatusArrayAsync()
        {
            var tasks = GameDataManager.GetAllStages()
                // FIXME: 呼ばれるたびに 全ステージ数 分リクエストしてしまうので、リクエストを減らす工夫をする
                .Select(stage => NetworkService.Execute(new GetStageStatusRequest(stage.TreeId, stage.StageNumber)));

            stageStatusArray.Value = await UniTask.WhenAll(tasks);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
