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
        public readonly ReactiveProperty<StageRecord[]> stageRecordArray = new ReactiveProperty<StageRecord[]>();

        /// <summary>
        /// 起動日数
        /// </summary>
        public readonly ReactiveProperty<int> startupDays = new ReactiveProperty<int>();

        public GeneralRecordModel()
        {
            FetchStageRecordArray();

            UserRecord.Instance.StartupDaysObservable
                .Subscribe(startupDays => this.startupDays.Value = startupDays)
                .AddTo(_disposable);
        }

        public void FetchStageRecordArray()
        {
            stageRecordArray.Value = StageRecordService.Instance.Get().ToArray();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
