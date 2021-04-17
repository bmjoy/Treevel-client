using System.Collections.Generic;
using System.Linq;
using Treevel.Common.Entities;
using Treevel.Common.Utils;
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
            FetchStageStatusArray();

            RecordData.Instance.StartupDaysObservable
                .Subscribe(startupDays => this.startupDays.Value = startupDays)
                .AddTo(_disposable);

            RecordData.Instance.failureReasonCount
                .Subscribe(failureReasonCount => this.failureReasonCount.Value = failureReasonCount)
                .AddTo(_disposable);
        }

        public void FetchStageStatusArray()
        {
            stageStatusArray.Value = StageStatusService.INSTANCE.Get().ToArray();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
