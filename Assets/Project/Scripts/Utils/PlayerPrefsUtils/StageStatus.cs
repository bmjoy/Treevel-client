using System;
using UnityEngine;

namespace Project.Scripts.Utils.PlayerPrefsUtils
{
    [Serializable]
    public class StageStatus
    {
        /// <summary>
        /// クリア有無
        /// </summary>
        public bool passed = false;

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
        /// オブジェクトの保存
        /// </summary>
        /// <param name="stageId"> ステージ id </param>
        private void Set(int stageId)
        {
            MyPlayerPrefs.SetObject(PlayerPrefsKeys.STAGE_STATUS_KEY + stageId, this);
        }

        /// <summary>
        /// オブジェクトの取得
        /// </summary>
        /// <param name="stageId"> ステージ id </param>
        /// <returns></returns>
        public static StageStatus Get(int stageId)
        {
            return MyPlayerPrefs.GetObject<StageStatus>(PlayerPrefsKeys.STAGE_STATUS_KEY + stageId);
        }

        /// <summary>
        /// オブジェクト情報のリセット
        /// </summary>
        /// <param name="stageId"></param>
        public static void Reset(int stageId)
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.STAGE_STATUS_KEY + stageId);
        }

        /// <summary>
        /// クリア済みにする
        /// </summary>
        /// <param name="stageId"> ステージ id </param>
        public void ClearStage(int stageId)
        {
            if (!passed) firstSuccessNum = challengeNum;
            passed = true;
            Set(stageId);
        }

        /// <summary>
        /// 挑戦回数を 1 加算する
        /// </summary>
        /// <param name="stageId"> ステージ id </param>
        public void IncChallengeNum(int stageId)
        {
            challengeNum++;
            Set(stageId);
        }

        /// <summary>
        /// 成功回数を 1 加算する
        /// </summary>
        /// <param name="stageId"> ステージ id </param>
        public void IncSuccessNum(int stageId)
        {
            successNum++;
            Set(stageId);
        }

        /// <summary>
        /// 失敗回数を 1 加算する
        /// </summary>
        /// <param name="stageId"> ステージ id </param>
        public void IncFailureNum(int stageId)
        {
            failureNum++;
            Set(stageId);
        }
    }
}
