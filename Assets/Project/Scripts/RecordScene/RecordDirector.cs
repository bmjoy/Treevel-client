using System;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.RecordScene
{
	public class RecordDirector : MonoBehaviour
	{
		public GameObject graphPrefab;

		public GameObject stageNumPrefab;

		private GameObject level;

		private GameObject percentage;

		private GameObject graphArea;

		private GameObject leftButton;

		private GameObject rightButton;

		private int nowLevel;

		private void Awake()
		{
			level = GameObject.Find("Level");
			percentage = GameObject.Find("Percentage");
			graphArea = GameObject.Find("GraphArea");
			leftButton = GameObject.Find("LeftButton");
			rightButton = GameObject.Find("RightButton");

			leftButton.GetComponent<Button>().onClick.AddListener(LeftButtonDown);
			rightButton.GetComponent<Button>().onClick.AddListener(RightButtonDown);

			// "Easy"を表示
			Draw(0);
		}

		/* 難易度に合わせた画面を描画する */
		private void Draw(int levelNum)
		{
			// 画面を綺麗にする
			GameObject[] graphUis = GameObject.FindGameObjectsWithTag(TagName.GRAPH_UI);
			foreach (var graphUi in graphUis)
			{
				Destroy(graphUi);
			}

			switch (levelNum)
			{
				case 0:
					nowLevel = 0;
					level.GetComponent<Text>().text = "Easy";
					DrawPercentage(StageNum.EASY, StageStartId.EASY);
					DrawGraph(StageNum.EASY, StageStartId.EASY);
					break;
				case 1:
					nowLevel = 1;
					level.GetComponent<Text>().text = "Normal";
					DrawPercentage(StageNum.NORMAL, StageStartId.NORMAL);
					DrawGraph(StageNum.NORMAL, StageStartId.NORMAL);
					break;
				case 2:
					nowLevel = 2;
					level.GetComponent<Text>().text = "Hard";
					DrawPercentage(StageNum.HARD, StageStartId.HARD);
					DrawGraph(StageNum.HARD, StageStartId.HARD);
					break;
				case 3:
					nowLevel = 3;
					level.GetComponent<Text>().text = "VeryHard";
					DrawPercentage(StageNum.VERY_HARD, StageStartId.VERY_HARD);
					DrawGraph(StageNum.VERY_HARD, StageStartId.VERY_HARD);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		/* 難易度に合わせた成功割合を描画する */
		private void DrawPercentage(int stageNum, int stageStartId)
		{
			var clearStageNum = 0;

			for (var stageId = stageStartId; stageId < stageStartId + stageNum ; stageId++)
			{
				var stageStatus = StageStatus.Get(stageId);

				if (stageStatus.passed)
				{
					clearStageNum++;
				}
			}

			var percentageNum = (clearStageNum / (float) stageNum) * 100;

			percentage.GetComponent<Text>().text = percentageNum + "%";
		}

		/* 難易度に合わせた棒グラフを描画する */
		private void DrawGraph(int stageNum, int stageStartId)
		{
			// 描画する範囲
			var graphAreaContent = GameObject.Find("GraphArea").GetComponent<RectTransform>();;
			// 隙間の大きさ
			var blank = 0.85f / (2 * stageNum + 1);
			// グラフ描画の左端
			var leftPosition = 0.1f;
			// ステージ番号
			var stageName = 1;

			// 挑戦回数の最大値を求める
			var maxChallengeNum = 0;
			for (var stageId = stageStartId; stageId < stageStartId + stageNum ; stageId++)
			{
				var stageStatus = StageStatus.Get(stageId);
				if (stageStatus.challengeNum > maxChallengeNum)
				{
					maxChallengeNum = stageStatus.challengeNum;
				}
			}

			// 目盛の数値を書き換える
			var maxScale = (float) Math.Ceiling(maxChallengeNum / 3.0f) * 3.0f;
			GameObject.Find("Scale2-Value").GetComponent<Text>().text = (maxScale / 3).ToString();
			GameObject.Find("Scale3-Value").GetComponent<Text>().text = (maxScale * 2 / 3).ToString();
			GameObject.Find("Scale4-Value").GetComponent<Text>().text = maxScale.ToString();

			for (var stageId = stageStartId; stageId < stageStartId + stageNum ; stageId++)
			{
				leftPosition += blank;

				var graphUi = Instantiate(graphPrefab);
				graphUi.transform.SetParent(graphAreaContent, false);
				graphUi.GetComponent<RectTransform>().anchorMin = new Vector2(leftPosition, 0.15f);

				var stageStatus = StageStatus.Get(stageId);
				var maxY = 0.15f;
				if(maxChallengeNum != 0)
				{
					maxY = 0.75f * stageStatus.challengeNum / maxScale + 0.15f;
				}
				graphUi.GetComponent<RectTransform>().anchorMax = new Vector2(leftPosition + blank, maxY);

				var stageNumUi = Instantiate(stageNumPrefab);
				stageNumUi.transform.SetParent(graphAreaContent, false);
				stageNumUi.GetComponent<Text>().text = stageName.ToString();
				stageNumUi.GetComponent<RectTransform>().anchorMin = new Vector2(leftPosition, 0.05f);
				stageNumUi.GetComponent<RectTransform>().anchorMax = new Vector2(leftPosition + blank, 0.15f);

				leftPosition += blank;
				stageName++;
			}
		}

		/* 左ボタンクリック時の処理 */
		private void LeftButtonDown()
		{
			if (nowLevel == 0)
			{
				Draw(3);
			}
			else
			{
				Draw(nowLevel - 1);
			}
		}

		/* 右ボタンクリック時の処理 */
		private void RightButtonDown()
		{
			if (nowLevel == 3)
			{
				Draw(0);
			}
			else
			{
				Draw(nowLevel + 1);
			}
		}
	}
}
