using UnityEngine;
﻿using Project.Scripts.Utils.Definitions;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene
{
	public class NormalStageSelectDirector : StageSelectDirector
	{
		protected override void MakeButtons()
		{
			var content = GameObject.Find("Canvas/Scroll View/Viewport/Content/Buttons").GetComponent<RectTransform>();
			for (var i = 0; i < StageNum.NORMAL; i++)
			{
				// ステージを一意に定めるID
				var stageId = StageStartId.NORMAL + i;
				// ボタンインスタンスを生成
				var button = Instantiate(stageButtonPrefab);
				// 名前
				button.name = stageId.ToString();
				// 親ディレクトリ
				button.transform.SetParent(content, false);
				// 表示テキスト
				button.GetComponentInChildren<Text>().text = "ステージ" + stageId + "へ";
				// クリック時のリスナー
				button.GetComponent<Button>().onClick.AddListener(() => StageButtonDown(button));
				// Buttonの色
				button.GetComponent<Image>().color = new Color(1.0f, 0.75f, 0.75f);
				// Buttonの位置
				var rectTransform = button.GetComponent<RectTransform>();
				// 下部のマージン : 0.05f
				// ボタン間の間隔 : 0.10f
				var buttonPositionY = 0.05f + i * 0.10f;
				// ボタンの縦幅 : 0.04f (上に0.02f, 下に0.02fをアンカー中央から伸ばす)
				rectTransform.anchorMax = new Vector2(0.90f, buttonPositionY + 0.02f);
				rectTransform.anchorMin = new Vector2(0.10f, buttonPositionY - 0.02f);
				rectTransform.anchoredPosition = new Vector2(0.50f, buttonPositionY);
			}
		}
	}
}
