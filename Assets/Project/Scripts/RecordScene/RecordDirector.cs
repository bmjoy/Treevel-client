using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.RecordScene
{
	public class RecordDirector : MonoBehaviour
	{
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
			switch (levelNum)
			{
				case 0:
					nowLevel = 0;
					level.GetComponent<Text>().text = "Easy";
					leftButton.SetActive(false);
					rightButton.SetActive(true);
					break;
				case 1:
					nowLevel = 1;
					level.GetComponent<Text>().text = "Normal";
					leftButton.SetActive(true);
					rightButton.SetActive(true);
					break;
				case 2:
					nowLevel = 2;
					level.GetComponent<Text>().text = "Hard";
					leftButton.SetActive(true);
					rightButton.SetActive(true);
					break;
				case 3:
					nowLevel = 3;
					level.GetComponent<Text>().text = "VeryHard";
					leftButton.SetActive(true);
					rightButton.SetActive(false);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		/* 左ボタンクリック時の処理 */
		private void LeftButtonDown()
		{
			Draw(nowLevel - 1);
		}

		/* 右ボタンクリック時の処理 */
		private void RightButtonDown()
		{
			Draw(nowLevel + 1);
		}
	}
}
