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

		private StageLevel nowLevel;

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
		private void Draw(StageLevel stageLevel)
		{
			// 画面を綺麗にする
			GameObject[] graphUis = GameObject.FindGameObjectsWithTag(TagName.GRAPH_UI);
			foreach (var graphUi in graphUis)
			{
				Destroy(graphUi);
			}

			// 難易度の更新
			nowLevel = stageLevel;

			switch (nowLevel)
			{
				case StageLevel.Easy:
					level.GetComponent<Text>().text = "Easy";
					DrawPercentage(StageLevel.Easy);
					DrawGraph(StageLevel.Easy);
					break;
				case StageLevel.Normal:
					level.GetComponent<Text>().text = "Normal";
					DrawPercentage(StageLevel.Normal);
					DrawGraph(StageLevel.Normal);
					break;
				case StageLevel.Hard:
					level.GetComponent<Text>().text = "Hard";
					DrawPercentage(StageLevel.Hard);
					DrawGraph(StageLevel.Hard);
					break;
				case StageLevel.VeryHard:
					level.GetComponent<Text>().text = "VeryHard";
					DrawPercentage(StageLevel.VeryHard);
					DrawGraph(StageLevel.VeryHard);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		/* 難易度に合わせた成功割合を描画する */
		private void DrawPercentage(StageLevel stageLevel)
		{
			var stageNum = StageInfo.Num[stageLevel];
			var stageStartId = StageInfo.StageStartId[stageLevel];

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
		private void DrawGraph(StageLevel stageLevel)
		{
			var stageNum = StageInfo.Num[stageLevel];
			var stageStartId = StageInfo.StageStartId[stageLevel];

			// 描画するパネル
			var graphAreaContent = graphArea.GetComponent<RectTransform>();

			/* UIの配置周りの定数宣言 */

			// 棒グラフ描画範囲の左端
			const float leftPosition = 0.1f;

			// 棒グラフ描画範囲の右端
			const float rightPosition = 0.95f;

			// 棒グラフ本体の上端
			const float topPosition = 0.9f;

			// 棒グラフ本体の下端
			const float bottomPosition = 0.15f;

			// 棒グラフの横幅が隙間の何倍か
			const float graphWidthRatio = 1.0f;

			// ステージ番号の下端
			const float bottomStageNumPosition = 0.05f;

			// グラフ間の隙間の横幅 -> stageNum個のグラフと(stageNum + 1)個の隙間
			var blankWidth = (rightPosition - leftPosition) / (stageNum * graphWidthRatio + (stageNum + 1));

			// 棒グラフの横幅
			var graphWidth = blankWidth * graphWidthRatio;

			// 挑戦回数の最大値を求める
			var maxChallengeNum = GetMaxChallengeNum(stageNum, stageStartId);

			// 目盛の最大値を求める
			var maxScale = (float) Math.Ceiling((float) maxChallengeNum / 30) * 30;

			// 目盛を書き換える
			if (maxScale > 0)
			{
				GameObject.Find("Scale4-Value").GetComponent<Text>().text = maxScale.ToString();
				GameObject.Find("Scale3-Value").GetComponent<Text>().text = (maxScale * 2 / 3).ToString();
				GameObject.Find("Scale2-Value").GetComponent<Text>().text = (maxScale / 3).ToString();
			}

			// ステージ番号
			var stageName = 1;
			// 描画する棒グラフの左端を示す
			var left = leftPosition;

			for (var stageId = stageStartId; stageId < stageStartId + stageNum; stageId++)
			{
				// 左端と右端の更新
				left += blankWidth;
				var right = left + graphWidth;
				
				var stageStatus = StageStatus.Get(stageId);

				/* ステージ番号の配置 */
				var stageNumUi = Instantiate(stageNumPrefab);
				stageNumUi.transform.SetParent(graphAreaContent, false);
				stageNumUi.GetComponent<Text>().text = stageName.ToString();
				stageNumUi.GetComponent<RectTransform>().anchorMin = new Vector2(left, bottomStageNumPosition);
				stageNumUi.GetComponent<RectTransform>().anchorMax = new Vector2(right, bottomPosition);

				/* 棒グラフの配置 */
				var graphUi = Instantiate(graphPrefab);
				graphUi.transform.SetParent(graphAreaContent, false);

				// 挑戦回数に応じた棒グラフの上端
				var graphMaxY = bottomPosition;

				if (maxChallengeNum != 0)
				{
					// 挑戦回数を用いて，棒グラフの高さを描画範囲に正規化する
					graphMaxY = (topPosition - bottomPosition) * (stageStatus.challengeNum / maxScale) + bottomPosition;
				}

				graphUi.GetComponent<RectTransform>().anchorMin = new Vector2(left, bottomPosition);
				graphUi.GetComponent<RectTransform>().anchorMax = new Vector2(right, graphMaxY);

				if (stageStatus.passed)
				{
					/* 成功している場合は，色を水色にして，成功した際の挑戦回数も示す */
					graphUi.GetComponent<Image>().color = Color.cyan;

					var successLineUi = Instantiate(successLinePrefab);
					successLineUi.transform.SetParent(graphAreaContent, false);
					var successY = (topPosition - bottomPosition) * (stageStatus.firstSuccessNum / maxScale) +
					               bottomPosition;
					successLineUi.GetComponent<RectTransform>().anchorMin = new Vector2(left, successY);
					successLineUi.GetComponent<RectTransform>().anchorMax = new Vector2(right, successY);
				}
				else
				{
					/* 成功していない場合は，色を赤色にする */
					graphUi.GetComponent<Image>().color = Color.red;
				}

				// 左端の更新
				left = right;
				// ステージ番号の更新
				stageName++;
			}
		}

		/* 挑戦回数の最大値を求める */
		private static int GetMaxChallengeNum(int stageNum, int stageStartId)
		{
			var maxChallengeNum = 0;

			for (var stageId = stageStartId; stageId < stageStartId + stageNum; stageId++)
			{
				var stageStatus = StageStatus.Get(stageId);

				if (stageStatus.challengeNum > maxChallengeNum)
				{
					maxChallengeNum = stageStatus.challengeNum;
				}
			}

			return maxChallengeNum;
		}

		/* 左ボタンクリック時の処理 */
		private void LeftButtonDown()
		{
			if (nowLevel == StageLevel.Easy)
			{
				Draw(StageLevel.VeryHard);
			}
			else
			{
				Draw(nowLevel - 1);
			}
		}

		/* 右ボタンクリック時の処理 */
		private void RightButtonDown()
		{
			if (nowLevel == StageLevel.VeryHard)
			{
				Draw(StageLevel.Easy);
			}
			else
			{
				Draw(nowLevel + 1);
			}
		}
	}
}
