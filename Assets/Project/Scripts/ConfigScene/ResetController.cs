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
			ResetStageStatus(StageNum.EASY, StageStartId.EASY);
			ResetStageStatus(StageNum.NORMAL, StageStartId.NORMAL);
			ResetStageStatus(StageNum.HARD, StageStartId.HARD);
			ResetStageStatus(StageNum.VERY_HARD, StageStartId.VERY_HARD);
		}

		private static void ResetStageStatus(int stageNum, int stageStartId)
		{
			for (var stageId = stageStartId; stageId < stageStartId + stageNum; stageId++)
			{
				StageStatus.Reset(stageId);
			}
		}
	}
}
