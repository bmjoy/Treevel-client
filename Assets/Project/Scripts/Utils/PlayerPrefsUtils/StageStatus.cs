using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.Utils.PlayerPrefsUtils
{
    [Serializable]
    public class StageStatus
    {
        private ETreeId _treeId;

        /// <summary>
        /// ステージID
        /// </summary>
        private int _stageNumber;

        /// <summary>
        /// ステージの状態
        /// </summary>
        public EStageState state = EStageState.Unreleased;

        /// <summary>
        /// 挑戦回数
        /// </summary>
        public int challengeNum = 0;

        /// <summary>
        /// 成功回数
        /// </summary>
        public int successNum = 0;

        /// <summary>
        /// 失敗回数
        /// </summary>
        public int failureNum = 0;

        /// <summary>
        /// 各失敗原因に対する失敗回数
        /// </summary>
        public Dictionary<FailureReasonType, int> failureReasonsNum = new Dictionary<FailureReasonType, int>();

        /// <summary>
        /// 初成功にかかった挑戦回数
        /// </summary>
        public int firstSuccessNum = 0;

        /// <summary>
        /// フリック回数
        /// </summary>
        public int flickNum = 0;

        /// <summary>
        /// チュートリアルを見たかどうか
        /// </summary>
        public bool tutorialChecked = false;

        /// <summary>
        /// オブジェクトの保存
        /// </summary>
        /// <param name="treeId"></param>
        /// <param name="stageNumber"></param>
        private void Set(ETreeId treeId, int stageNumber)
        {
            MyPlayerPrefs.SetObject(StageData.EncodeStageIdKey(treeId, stageNumber), this);
        }

        /// <summary>
        /// オブジェクトの取得
        /// </summary>
        /// <param name="treeId"></param>
        /// <param name="stageNumber"></param>
        /// <returns></returns>
        public static StageStatus Get(ETreeId treeId, int stageNumber)
        {
            var data = MyPlayerPrefs.GetObject<StageStatus>(StageData.EncodeStageIdKey(treeId, stageNumber));
            data._treeId = treeId;
            data._stageNumber = stageNumber;
            return data;
        }

        /// <summary>
        /// オブジェクト情報のリセット
        /// </summary>
        public static void Reset()
        {
            foreach (ETreeId treeId in Enum.GetValues(typeof(ETreeId))) {
                var stageNum = TreeInfo.NUM[treeId];

                Enumerable.Range(1, stageNum).ToList().ForEach(stageId => PlayerPrefs.DeleteKey(StageData.EncodeStageIdKey(treeId, stageId)));
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
            Set(treeId, stageNumber);
        }

        /// <summary>
        /// クリア済みにする
        /// </summary>
        /// <param name="treeId"> 木のID </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        public void ClearStage(ETreeId treeId, int stageNumber)
        {
            if (state != EStageState.Cleared) firstSuccessNum = challengeNum;
            state = EStageState.Cleared;
            Set(treeId, stageNumber);
        }

        /// <summary>
        /// 挑戦回数を 1 加算する
        /// </summary>
        /// <param name="treeId"> 木のID </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        public void IncChallengeNum(ETreeId treeId, int stageNumber)
        {
            challengeNum++;
            Set(treeId, stageNumber);
        }

        /// <summary>
        /// 成功回数を 1 加算する
        /// </summary>
        /// <param name="treeId"> 木のID </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        public void IncSuccessNum(ETreeId treeId, int stageNumber)
        {
            successNum++;
            Set(treeId, stageNumber);
        }

        /// <summary>
        /// 失敗回数を 1 加算する
        /// </summary>
        /// <param name="treeId"> 木のID </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        public void IncFailureNum(ETreeId treeId, int stageNumber)
        {
            failureNum++;
            Set(treeId, stageNumber);
        }

        /// <summary>
        /// フリック回数の加算
        /// </summary>
        /// <param name="treeId"> 木の ID </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        /// <param name="flickNum"> 加算するフリック回数 </param>
        public void AddFlickNum(ETreeId treeId, int stageNumber, int flickNum)
        {
            this.flickNum += flickNum;
            Set(treeId, stageNumber);
        }

        public void Update(bool success)
        {
            if (success) {
                // クリア済みにする
                ClearStage(_treeId, _stageNumber);

                IncSuccessNum(_treeId, _stageNumber);
            } else {
                IncFailureNum(_treeId, _stageNumber);
            }
        }

        public void Dump()
        {
            #if UNITY_EDITOR
            Debug.Log($"Stage: {_stageNumber}");
            Debug.Log($"  挑戦回数：{challengeNum}");
            Debug.Log($"  成功回数：{successNum}");
            Debug.Log($"  失敗回数：{failureNum}");
            #endif
        }

        public void SetTutorialChecked(bool isChecked)
        {
            tutorialChecked = isChecked;
            Set(this._treeId, this._stageNumber);
        }
    }
}
