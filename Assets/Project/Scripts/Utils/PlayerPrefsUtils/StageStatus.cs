using System;
using UnityEngine;

namespace Project.Scripts.Utils.PlayerPrefsUtils
{
	[Serializable]
	public class StageStatus
	{
		// クリア有無
		public bool passed = false;

		// 挑戦回数
		public int challengeNum = 0;

		// 成功回数
		public int successNum = 0;

		// 失敗回数
		public int failureNum = 0;

		// 初成功にかかった挑戦回数
		public int firstSuccessNum = -1;

		private static void Set(int stageId, StageStatus stageStatus)
		{
			MyPlayerPrefs.SetObject(PlayerPrefsKeys.STAGE_STATUS_KEY + stageId, stageStatus);
		}

		public static StageStatus Get(int stageId)
		{
			return MyPlayerPrefs.GetObject<StageStatus>(PlayerPrefsKeys.STAGE_STATUS_KEY + stageId);
		}

		public static void Reset(int stageId)
		{
			PlayerPrefs.DeleteKey(PlayerPrefsKeys.STAGE_STATUS_KEY + stageId);
		}

		public void ClearStage(int stageId)
		{
			if (!passed) firstSuccessNum = challengeNum;
			passed = true;
			Set(stageId, this);
		}

		public void IncChallengeNum(int stageId)
		{
			challengeNum++;
			Set(stageId, this);
		}

		public void IncSuccessNum(int stageId)
		{
			successNum++;
			Set(stageId, this);
		}

		public void IncFailureNum(int stageId)
		{
			failureNum++;
			Set(stageId, this);
		}
	}
}
