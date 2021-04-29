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
        public readonly ReactiveProperty<UserRecord> userRecord = new ReactiveProperty<UserRecord>();

        public GeneralRecordModel()
        {
            FetchStageRecordArray();
            FetchUserRecord();
        }

        public void FetchStageRecordArray()
        {
            stageRecordArray.Value = StageRecordService.Instance.Get().ToArray();
        }

        public void FetchUserRecord()
        {
            userRecord.Value = UserRecordService.Instance.Get();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
