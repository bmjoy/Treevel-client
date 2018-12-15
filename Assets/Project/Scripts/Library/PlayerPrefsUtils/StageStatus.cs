using System;
using UnityEngine;

namespace Project.Scripts.Library.PlayerPrefsUtils
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

		public static void Set(int stageNum, StageStatus stageStatus)
		{
			MyPlayerPrefs.SetObject(PlayerPrefsKeys.STAGE_STATUS_KEY + stageNum, stageStatus);
		}

		public static StageStatus Get(int stageNum)
		{
			return MyPlayerPrefs.GetObject<StageStatus>(PlayerPrefsKeys.STAGE_STATUS_KEY + stageNum);
		}

		public static void Reset(int stageNum)
		{
			PlayerPrefs.DeleteKey(PlayerPrefsKeys.STAGE_STATUS_KEY + stageNum);
		}
	}
}
