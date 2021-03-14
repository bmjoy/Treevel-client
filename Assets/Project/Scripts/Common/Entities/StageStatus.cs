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
    public class StageStatus
    {
        /// <summary>
        /// ステージの状態
        /// </summary>
        public EStageState State { get; private set; } = EStageState.Unreleased;

        /// <summary>
        /// 挑戦回数
        /// </summary>
        public int ChallengeNum { get; private set; }

        /// <summary>
        /// 失敗回数
        /// </summary>
        public int FailureNum { get; private set; }

        /// <summary>
        /// フリック回数
        /// </summary>
        public int FlickNum { get; private set; }

        /// <summary>
        /// チュートリアルを見たかどうか
        /// </summary>
        public bool TutorialChecked { get; private set; }

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
        /// 解放する
        /// </summary>
        /// <param name="treeId"> 木のID </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        public void ReleaseStage(ETreeId treeId, int stageNumber)
        {
            State = EStageState.Released;
            NetworkService.Execute(new UpdateStageStatusRequest(treeId, stageNumber, this));
        }

        /// <summary>
        /// クリア済みにする
        /// </summary>
        /// <param name="treeId"> 木のID </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        public void ClearStage(ETreeId treeId, int stageNumber)
        {
            State = EStageState.Cleared;
            NetworkService.Execute(new UpdateStageStatusRequest(treeId, stageNumber, this));
        }

        /// <summary>
        /// 挑戦回数を 1 加算する
        /// </summary>
        /// <param name="treeId"> 木のID </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        public void IncChallengeNum(ETreeId treeId, int stageNumber)
        {
            ChallengeNum++;
            NetworkService.Execute(new UpdateStageStatusRequest(treeId, stageNumber, this));
        }

        /// <summary>
        /// 失敗回数を 1 加算する
        /// </summary>
        /// <param name="treeId"> 木のID </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        public void IncFailureNum(ETreeId treeId, int stageNumber)
        {
            FailureNum++;
            NetworkService.Execute(new UpdateStageStatusRequest(treeId, stageNumber, this));
        }

        /// <summary>
        /// フリック回数の加算
        /// </summary>
        /// <param name="treeId"> 木の ID </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        /// <param name="flickNum"> 加算するフリック回数 </param>
        public void AddFlickNum(ETreeId treeId, int stageNumber, int flickNum)
        {
            FlickNum += flickNum;
            NetworkService.Execute(new UpdateStageStatusRequest(treeId, stageNumber, this));
        }

        public void Update(ETreeId treeId, int stageNumber, bool success)
        {
            if (success) {
                ClearStage(treeId, stageNumber);
            } else {
                IncFailureNum(treeId, stageNumber);
            }
        }

        public void SetTutorialChecked(ETreeId treeId, int stageNumber, bool isChecked)
        {
            TutorialChecked = isChecked;
            NetworkService.Execute(new UpdateStageStatusRequest(treeId, stageNumber, this));
        }
    }
}
