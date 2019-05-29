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
			ResetStageStatus(StageLevel.Easy);
			ResetStageStatus(StageLevel.Normal);
			ResetStageStatus(StageLevel.Hard);
			ResetStageStatus(StageLevel.VeryHard);
		}

		private static void ResetStageStatus(StageLevel stageLevel)
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
