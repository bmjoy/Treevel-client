using System;
using UnityEngine;

namespace Project.Scripts.Utils.PlayerPrefsUtils
{
    [Serializable]
    public class StageStatus
    {
        /// <summary>
        /// ステージID
        /// </summary>
        [NonSerialized] private int _id;

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
        /// チュートリアルを見たかどうか
        /// </summary>
        public bool tutorialChecked = false;

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
            var data = MyPlayerPrefs.GetObject<StageStatus>(PlayerPrefsKeys.STAGE_STATUS_KEY + stageId);
            data._id = stageId;
            return data;
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

        public void Update(bool success)
        {
            if (success) {
                // クリア済みにする
                ClearStage(_id);

                IncSuccessNum(_id);
            } else {
                IncFailureNum(_id);
            }
        }

        public void Dump()
        {
            #if UNITY_EDITOR
            Debug.Log($"Stage: {_id}");
            Debug.Log($"  挑戦回数：{challengeNum}");
            Debug.Log($"  成功回数：{successNum}");
            Debug.Log($"  失敗回数：{failureNum}");
            #endif
        }

        public void SetTutorialChecked(bool isChecked)
        {
            tutorialChecked = isChecked;
            Set(this._id);
        }
    }
}
