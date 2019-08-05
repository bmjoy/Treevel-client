using System;
using System.Collections.Generic;
using Project.Scripts.GamePlayScene;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using SnapScroll;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene
{
    public class StageSelectDirector : MonoBehaviour
    {
        /// <summary>
        /// ステージボタンの Prefab
        /// </summary>
        public GameObject stageButtonPrefab;

        /// <summary>
        /// ゲーム画面のためのダミー背景
        /// </summary>
        public GameObject dummyBackgroundPrefab;

        /// <summary>
        /// 難易度ごとに固有の難易度名
        /// </summary>
        private static readonly Dictionary<EStageLevel, string> LevelName = new Dictionary<EStageLevel, string>()
        {
            {EStageLevel.Easy, "簡単"},
            {EStageLevel.Normal, "普通"},
            {EStageLevel.Hard, "ムズイ"},
            {EStageLevel.VeryHard, "激ムズ"}
        };

        /// <summary>
        /// 難易度ごとに固有のボタン色
        /// </summary>
        private static readonly Dictionary<EStageLevel, Color> Color = new Dictionary<EStageLevel, Color>()
        {
            {EStageLevel.Easy, UnityEngine.Color.magenta},
            {EStageLevel.Normal, UnityEngine.Color.green},
            {EStageLevel.Hard, UnityEngine.Color.yellow},
            {EStageLevel.VeryHard, UnityEngine.Color.cyan}
        };

        private SnapScrollView snapScrollView;

        private void Awake()
        {
            // 取得
            snapScrollView = FindObjectOfType<SnapScrollView>();
            // ページの最大値を設定
            snapScrollView.MaxPage = Enum.GetNames(typeof(EStageLevel)).Length - 1;
            // ページの横幅の設定
            snapScrollView.PageSize = Screen.width;

            // TODO: 非同期で呼び出す
            // 各ステージの選択ボタンなどを描画する
            Draw();
        }

        /// <summary>
        /// 全難易度の画面を描画する
        /// </summary>
        private void Draw()
        {
            foreach (EStageLevel stageLevel in Enum.GetValues(typeof(EStageLevel))) {
                UpdateText(stageLevel);
                MakeButtons(stageLevel);
            }
        }

        /// <summary>
        /// テキストを更新する
        /// </summary>
        /// <param name="stageLevel"> 変更するテキストの難易度 </param>
        private static void UpdateText(EStageLevel stageLevel) {
            var text = GameObject.Find("Canvas/SnapScrollView/Viewport/Content/" + stageLevel + "/Level").GetComponent<Text>();
            text.text = LevelName[stageLevel];
        }

        /// <summary>
        /// ボタンを配置する
        /// </summary>
        /// <param name="stageLevel"> 配置するボタンの難易度 </param>
        private void MakeButtons(EStageLevel stageLevel)
        {
            var content = GameObject.Find("Canvas/SnapScrollView/Viewport/Content/" + stageLevel + "/Scroll View/Viewport/Content/Buttons").GetComponent<RectTransform>();

            // TODO: 今後，難易度ごとにボタン配置を変える必要がある
            for (var i = 0; i < StageInfo.Num[stageLevel]; i++) {
                // ステージを一意に定めるID
                var stageId = StageInfo.StageStartId[stageLevel] + i;
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
                button.GetComponent<Image>().color = Color[stageLevel];
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

        /// <summary>
        /// ステージ選択画面からゲーム選択画面へ移動する
        /// </summary>
        /// <param name="clickedButton"> タッチされたボタン </param>
        protected void StageButtonDown(GameObject clickedButton)
        {
            // タップされたステージidを取得（暫定的にボタンの名前）
            var stageId = int.Parse(clickedButton.name);
            // 挑戦回数をインクリメント
            var ss = StageStatus.Get(stageId);
            ss.IncChallengeNum(stageId);
            // ステージ番号を渡す
            GamePlayDirector.stageId = stageId;
            // 背景画像をCanvasの下に置く
            var canvas = FindObjectOfType<Canvas>().gameObject.transform;
            var background = Instantiate(dummyBackgroundPrefab);
            background.transform.SetParent(canvas, false);

            // シーン遷移
            SceneManager.LoadScene(SceneName.GAME_PLAY_SCENE);
        }
    }
}
