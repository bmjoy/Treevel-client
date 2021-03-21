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
        public EStageState state;

        /// <summary>
        /// 挑戦回数
        /// </summary>
        public int challengeNum;

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
            state = EStageState.Released;
        }

        /// <summary>
        /// 成功した時の処理をする
        /// </summary>
        public void Succeed()
        {
            state = EStageState.Cleared;
        }

        /// <summary>
        /// 失敗した時の処理をする
        /// </summary>
        public void Fail()
        {
            failureNum++;
        }

        /// <summary>
        /// 挑戦回数を 1 加算する
        /// </summary>
        public void IncChallengeNum()
        {
            challengeNum++;
        }

        /// <summary>
        /// フリック回数を加算する
        /// </summary>
        /// <param name="flickNum"> 加算するフリック回数 </param>
        public void AddFlickNum(int flickNum)
        {
            this.flickNum += flickNum;
        }

        /// <summary>
        /// チュートリアルをチェック済みにする
        /// </summary>
        public void CheckTutorial()
        {
            tutorialChecked = true;
        }

        /// <summary>
        /// 自身を外部に保存する
        /// </summary>
        /// <param name="treeId"> 木の id </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        public void Save(ETreeId treeId, int stageNumber)
        {
            NetworkService.Execute(new UpdateStageStatusRequest(treeId, stageNumber, this));
        }
    }
}
