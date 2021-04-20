using System;
using System.Linq;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Networks;
using Treevel.Common.Networks.Requests;
using Treevel.Common.Utils;
using UnityEngine;

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

        /// <summary>
        /// オブジェクト情報のリセット
        /// </summary>
        public static void Reset()
        {
            foreach (ETreeId treeId in Enum.GetValues(typeof(ETreeId))) {
                var stageNum = treeId.GetStageNum();

                Enumerable.Range(1, stageNum).ToList()
                    .ForEach(stageId => PlayerPrefs.DeleteKey(StageData.EncodeStageIdKey(treeId, stageId)));
            }
        }

        /// <summary>
        /// 成功した時の処理をする
        /// </summary>
        public void Succeed()
        {
            if (!IsCleared) {
                challengeNumAtFirstSuccess = challengeNum;
                firstSuccessDate = DateTime.Now;
            }

            successNum++;
        }

        /// <summary>
        /// 失敗した時の処理をする
        /// </summary>
        public void Fail()
        {
            failureNum++;
        }
    }
}
