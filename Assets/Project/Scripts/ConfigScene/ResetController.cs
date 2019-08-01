using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;


namespace Project.Scripts.ConfigScene
{
	public class ResetController : MonoBehaviour
	{
		private Button resetButton;

		private void Awake()
		{
			resetButton = GetComponent<Button>();
			resetButton.onClick.AddListener(ResetButtonDown);
		}

		private static void ResetButtonDown()
		{
			ResetStageStatus(EStageLevel.Easy);
			ResetStageStatus(EStageLevel.Normal);
			ResetStageStatus(EStageLevel.Hard);
			ResetStageStatus(EStageLevel.VeryHard);
		}

		private static void ResetStageStatus(EStageLevel stageLevel)
		{
			var stageNum = StageInfo.Num[stageLevel];
			var stageStartId = StageInfo.StageStartId[stageLevel];

			for (var stageId = stageStartId; stageId < stageStartId + stageNum; stageId++)
			{
				StageStatus.Reset(stageId);
			}
		}
	}
}
