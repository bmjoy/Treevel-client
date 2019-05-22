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

		public GameObject successLinePrefab;

		private GameObject level;

		private GameObject percentage;

		private GameObject graphArea;

		private GameObject leftButton;

		private GameObject rightButton;

		private int nowLevel;

		// グラフの横幅が隙間の何倍か
		private const float RATIO_GRAPH_SPACE = 1.0f;

		private void Awake()
		{
			level = GameObject.Find("Level");
			percentage = GameObject.Find("Percentage");
			graphArea = GameObject.Find("GraphArea");
			leftButton = GameObject.Find("LeftButton");
			rightButton = GameObject.Find("RightButton");

			leftButton.GetComponent<Button>().onClick.AddListener(LeftButtonDown);
			rightButton.GetComponent<Button>().onClick.AddListener(RightButtonDown);

			// 最初は "Easy" を表示
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
			var successStageNum = 0;

			for (var stageId = stageStartId; stageId < stageStartId + stageNum; stageId++)
			{
				var stageStatus = StageStatus.Get(stageId);

				if (stageStatus.passed)
				{
					successStageNum++;
				}
			}

			var percentageNum = (successStageNum / (float) stageNum) * 100;

			percentage.GetComponent<Text>().text = percentageNum + "%";
		}

		/* 難易度に合わせた棒グラフを描画する */
		private void DrawGraph(int stageNum, int stageStartId)
		{
			// 挑戦回数の最大値を求める
			var maxChallengeNum = 0;
			for (var stageId = stageStartId; stageId < stageStartId + stageNum; stageId++)
			{
				var stageStatus = StageStatus.Get(stageId);

				if (stageStatus.challengeNum > maxChallengeNum)
				{
					maxChallengeNum = stageStatus.challengeNum;
				}
			}

			// 目盛の数値を書き換える
			var maxScale = (float) Math.Ceiling(maxChallengeNum / 3.0f) * 3.0f;
			if (maxScale != 0)
			{
				GameObject.Find("Scale2-Value").GetComponent<Text>().text = (maxScale / 3).ToString();
				GameObject.Find("Scale3-Value").GetComponent<Text>().text = (maxScale * 2 / 3).ToString();
				GameObject.Find("Scale4-Value").GetComponent<Text>().text = maxScale.ToString();
			}

			// 描画する範囲
			var graphAreaContent = graphArea.GetComponent<RectTransform>();
			;
			// 隙間の大きさ
			var blankWidth = 0.85f / ((1 + RATIO_GRAPH_SPACE) * stageNum + 1);
			// グラフの横幅
			var graphWidth = blankWidth * RATIO_GRAPH_SPACE;
			// グラフ描画の左端
			var leftPosition = 0.1f;
			// グラフ描画の左端
			var rightPosition = 0.85f;
			// グラフ本体の上端
			const float topGraphPosition = 0.9f;
			// グラフ本体の下端
			const float bottomGraphPosition = 0.15f;
			// ステージ番号の下端
			const float bottomStageNumPosition = 0.05f;
			// ステージ番号
			var stageName = 1;

			for (var stageId = stageStartId; stageId < stageStartId + stageNum; stageId++)
			{
				var stageStatus = StageStatus.Get(stageId);

				leftPosition += blankWidth;

				/* ステージ番号の配置 */
				var stageNumUi = Instantiate(stageNumPrefab);
				stageNumUi.transform.SetParent(graphAreaContent, false);
				stageNumUi.GetComponent<Text>().text = stageName.ToString();
				stageNumUi.GetComponent<RectTransform>().anchorMin = new Vector2(leftPosition, bottomStageNumPosition);
				stageNumUi.GetComponent<RectTransform>().anchorMax =
					new Vector2(leftPosition + graphWidth, bottomGraphPosition);

				/* グラフ本体の配置 */
				var graphUi = Instantiate(graphPrefab);
				graphUi.transform.SetParent(graphAreaContent, false);

				// 挑戦回数に応じたグラフの高さ
				var maxY = bottomGraphPosition;

				if (maxChallengeNum != 0)
				{
					// 挑戦回数に関して，グラフ本体の描画範囲に正規化する
					maxY = (topGraphPosition - bottomGraphPosition) * stageStatus.challengeNum / maxScale +
					       bottomGraphPosition;
				}

				graphUi.GetComponent<RectTransform>().anchorMin = new Vector2(leftPosition, bottomGraphPosition);
				graphUi.GetComponent<RectTransform>().anchorMax = new Vector2(leftPosition + graphWidth, maxY);

				if (stageStatus.passed)
				{
					/* 成功している場合は，色を水色にして，成功した際の挑戦回数を表示する */
					graphUi.GetComponent<Image>().color = Color.cyan;

					var successLineUi = Instantiate(successLinePrefab);
					var successY = (topGraphPosition - bottomGraphPosition) * stageStatus.firstSuccessNum / maxScale +
					               bottomGraphPosition;
					successLineUi.transform.SetParent(graphAreaContent, false);
					successLineUi.GetComponent<RectTransform>().anchorMin = new Vector2(leftPosition, successY);
					successLineUi.GetComponent<RectTransform>().anchorMax =
						new Vector2(leftPosition + graphWidth, successY);
				}
				else
				{
					graphUi.GetComponent<Image>().color = Color.red;
				}

				leftPosition += graphWidth;

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
