using System;

namespace Treevel.Common.Entities
{
    [Serializable]
    public class StageRecord
    {
        /// 木の Id
        /// </summary>
        public ETreeId treeId;

        /// <summary>
        /// ステージ番号
        /// </summary>
        public int stageNumber;

        /// <summary>
        /// クリアしたかどうか
        /// </summary>
        public bool IsCleared => successNum > 0;

        /// <summary>
        /// 挑戦回数
        /// </summary>
        public int challengeNum;

        /// <summary>
        /// 初成功時の挑戦回数
        /// </summary>
        public int challengeNumAtFirstSuccess;

        /// <summary>
        /// 初成功時の日付
        /// </summary>
        public DateTime firstSuccessDate;

        /// <summary>
        /// 成功回数
        /// </summary>
        public int successNum;

        /// <summary>
        /// 失敗回数
        /// </summary>
        public int failureNum;

        /// <summary>
        /// 各失敗原因における失敗回数
        /// </summary>
        public FailureReasonNum failureReasonNum = new FailureReasonNum();

        /// <summary>
        /// フリック回数
        /// </summary>
        public int flickNum;

        /// <summary>
        /// チュートリアルを見たかどうか
        /// </summary>
        public bool tutorialChecked;

        public StageRecord(ETreeId treeId, int stageNumber)
        {
            this.treeId = treeId;
            this.stageNumber = stageNumber;
        }

        public StageRecord(ETreeId treeId,
                           int stageNumber,
                           int challengeNum,
                           int challengeNumAtFirstSuccess,
                           DateTime firstSuccessDate,
                           int successNum,
                           int failureNum,
                           FailureReasonNum failureReasonNum,
                           int flickNum,
                           bool tutorialChecked)
        {
            this.treeId = treeId;
            this.stageNumber = stageNumber;
            this.challengeNum = challengeNum;
            this.challengeNumAtFirstSuccess = challengeNumAtFirstSuccess;
            this.firstSuccessDate = firstSuccessDate;
            this.successNum = successNum;
            this.failureNum = failureNum;
            this.failureReasonNum = failureReasonNum;
            this.flickNum = flickNum;
            this.tutorialChecked = tutorialChecked;
        }

        /// <summary>
        /// 成功した時の処理をし、インスタンスを作り直して返す
        /// </summary>
        public StageRecord Succeed()
        {
            return new StageRecord(treeId,
                                   stageNumber,
                                   challengeNum,
                                   IsCleared ? challengeNumAtFirstSuccess : challengeNum,
                                   IsCleared ? firstSuccessDate : DateTime.Now,
                                   successNum + 1,
                                   failureNum,
                                   failureReasonNum,
                                   flickNum,
                                   tutorialChecked);
        }

        /// <summary>
        /// 失敗した時の処理をし、インスタンスを作り直して返す
        /// </summary>
        public StageRecord Fail()
        {
            return new StageRecord(treeId,
                                   stageNumber,
                                   challengeNum,
                                   challengeNumAtFirstSuccess,
                                   firstSuccessDate,
                                   successNum,
                                   failureNum + 1,
                                   failureReasonNum,
                                   flickNum,
                                   tutorialChecked);
        }
    }

    [Serializable]
    public class FailureReasonNum
    {
        public int others;

        public int tornado;

        public int meteorite;

        public int aimingMeteorite;

        public int thunder;

        public int solarBeam;

        public int powder;

        /// <summary>
        /// 特定の失敗原因に対する失敗回数を 1 加算する
        /// </summary>
        /// <param name="failureReasonType"> 失敗原因 </param>
        public void Increment(EFailureReasonType failureReasonType)
        {
            switch (failureReasonType) {
                case EFailureReasonType.Others:
                    others++;
                    break;
                case EFailureReasonType.Tornado:
                    tornado++;
                    break;
                case EFailureReasonType.Meteorite:
                    meteorite++;
                    break;
                case EFailureReasonType.AimingMeteorite:
                    aimingMeteorite++;
                    break;
                case EFailureReasonType.Thunder:
                    thunder++;
                    break;
                case EFailureReasonType.SolarBeam:
                    solarBeam++;
                    break;
                case EFailureReasonType.Powder:
                    powder++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(failureReasonType), failureReasonType, "対応する処理を実装してください");
            }
        }

        /// <summary>
        /// 特定の失敗原因に対する失敗回数を取得する
        /// </summary>
        /// <param name="failureReasonType"> 失敗原因 </param>
        /// <returns> 失敗回数 </returns>
        public int Get(EFailureReasonType failureReasonType)
        {
            return failureReasonType switch {
                EFailureReasonType.Others => others,
                EFailureReasonType.Tornado => tornado,
                EFailureReasonType.Meteorite => meteorite,
                EFailureReasonType.AimingMeteorite => aimingMeteorite,
                EFailureReasonType.Thunder => thunder,
                EFailureReasonType.SolarBeam => solarBeam,
                EFailureReasonType.Powder => powder,
                _ => throw new ArgumentOutOfRangeException(nameof(failureReasonType), failureReasonType, "対応する処理を実装してください"),
            };
        }
    }
}
