using System;
using UnityEngine;
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
        /// 解放状態
        /// </summary>
        public bool released = false;

        /// <summary>
        /// クリア状態
        /// </summary>
        public bool cleared = false;

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
        /// 初成功にかかった挑戦回数
        /// </summary>
        public int firstSuccessNum = 0;

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
            MyPlayerPrefs.SetObject(PlayerPrefsKeys.EncodeStageIdKey(treeId, stageNumber), this);
        }

        /// <summary>
        /// オブジェクトの取得
        /// </summary>
        /// <param name="treeId"></param>
        /// <param name="stageNumber"></param>
        /// <returns></returns>
        public static StageStatus Get(ETreeId treeId, int stageNumber)
        {
            var data = MyPlayerPrefs.GetObject<StageStatus>(PlayerPrefsKeys.EncodeStageIdKey(treeId, stageNumber));
            data._treeId = treeId;
            data._stageNumber = stageNumber;
            return data;
        }

        /// <summary>
        /// オブジェクト情報のリセット
        /// </summary>
        /// <param name="stageId"></param>
        public static void Reset()
        {
            foreach (ETreeId treeId in Enum.GetValues(typeof(ETreeId))) {
                var stageNum = TreeInfo.NUM[treeId];

                for (var stageNumber = 1; stageNumber < stageNum; stageNumber++) {
                    PlayerPrefs.DeleteKey(PlayerPrefsKeys.EncodeStageIdKey(treeId, stageNumber));
                }
            }
        }

        /// <summary>
        /// 解放する
        /// </summary>
        /// <param name="treeId"></param>
        /// <param name="stageNumber"></param>
        public void ReleaseStage(ETreeId treeId, int stageNumber)
        {
            released = true;
            Set(treeId, stageNumber);
        }

        /// <summary>
        /// クリア済みにする
        /// </summary>
        /// <param name="stageId"> ステージ id </param>
        public void ClearStage(ETreeId treeId, int stageNumber)
        {
            if (!cleared) firstSuccessNum = challengeNum;
            cleared = true;
            Set(treeId, stageNumber);
        }

        /// <summary>
        /// 挑戦回数を 1 加算する
        /// </summary>
        /// <param name="stageId"> ステージ id </param>
        public void IncChallengeNum(ETreeId treeId, int stageNumber)
        {
            challengeNum++;
            Set(treeId, stageNumber);
        }

        /// <summary>
        /// 成功回数を 1 加算する
        /// </summary>
        /// <param name="stageId"> ステージ id </param>
        public void IncSuccessNum(ETreeId treeId, int stageNumber)
        {
            successNum++;
            Set(treeId, stageNumber);
        }

        /// <summary>
        /// 失敗回数を 1 加算する
        /// </summary>
        /// <param name="stageId"> ステージ id </param>
        public void IncFailureNum(ETreeId treeId, int stageNumber)
        {
            failureNum++;
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
