using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Networks;
using Treevel.Common.Networks.Requests;

namespace Treevel.Common.Utils
{
    public sealed class StageStatusService
    {
        // For Singleton
        private StageStatusService() { }

        /// <summary>
        /// インスタンス
        /// </summary>
        public static readonly StageStatusService Instance = new StageStatusService();

        /// <summary>
        /// オンメモリに StageStatus を保持する
        /// </summary>
        private readonly Dictionary<string, StageStatus> _cachedStageStatusDic = new Dictionary<string, StageStatus>();

        /// <summary>
        /// 全ステージの StageStatus を読み込んでおく
        /// </summary>
        public async UniTask PreloadAllStageStatusesAsync()
        {
            var stageStatuses = await NetworkService.Execute(new GetAllStageStatusListRequest());
            var stageStatusList = stageStatuses as List<StageStatus> ?? stageStatuses.ToList();

            foreach (var stageData in GameDataManager.GetAllStages()) {
                StageStatus stageStatus;
                try {
                    stageStatus = stageStatusList.First(stageStatus => stageStatus.treeId == stageData.TreeId
                                                                       && stageStatus.stageNumber == stageData.StageNumber);
                } catch {
                    stageStatus = PlayerPrefsUtility.GetObjectOrDefault(StageData.EncodeStageIdKey(stageData.TreeId, stageData.StageNumber),
                                                                        new StageStatus(stageData.TreeId, stageData.StageNumber));
                }

                _cachedStageStatusDic[StageData.EncodeStageIdKey(stageData.TreeId, stageData.StageNumber)] = stageStatus;
            }
        }

        /// <summary>
        /// 特定の StageStatus を取得する
        /// </summary>
        /// <param name="treeId"> 木の Id </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        /// <returns> StageStatus </returns>
        public StageStatus Get(ETreeId treeId, int stageNumber)
        {
            return _cachedStageStatusDic[StageData.EncodeStageIdKey(treeId, stageNumber)];
        }

        /// <summary>
        /// 特定の木の StageStatus を取得する
        /// </summary>
        /// <param name="treeId"> 木の Id </param>
        public IEnumerable<StageStatus> Get(ETreeId treeId)
        {
            return _cachedStageStatusDic.Values.ToList()
                .Where(stageStatus => stageStatus.treeId == treeId);
        }

        /// <summary>
        /// 全ての StageStatus を取得する
        /// </summary>
        public IEnumerable<StageStatus> Get()
        {
            return _cachedStageStatusDic.Values.ToList();
        }

        /// <summary>
        /// StageStatus を保存する
        /// </summary>
        /// <param name="treeId"> 木の Id </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        /// <param name="stageStatus"> 保存する StageStatus </param>
        public void Set(ETreeId treeId, int stageNumber, StageStatus stageStatus)
        {
            var key = StageData.EncodeStageIdKey(treeId, stageNumber);

            NetworkService.Execute(new UpdateStageStatusRequest(key, stageStatus))
                .ContinueWith(isSuccess => {
                    if (isSuccess) {
                        // データの保存に成功したら、PlayerPrefs とオンメモリにも保存する
                        PlayerPrefsUtility.SetObject(key, stageStatus);
                        _cachedStageStatusDic[key] = stageStatus;
                    }
                });
        }
    }
}
