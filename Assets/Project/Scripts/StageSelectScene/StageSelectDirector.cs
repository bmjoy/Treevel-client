using System;
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
        [SerializeField] protected GameObject stageButtonPrefab;

        /// <summary>
        /// ゲーム画面のためのダミー背景
        /// </summary>
        [SerializeField] protected GameObject dummyBackgroundPrefab;

        private SnapScrollView _snapScrollView;

        private void Awake()
        {
            // 取得
            _snapScrollView = FindObjectOfType<SnapScrollView>();
            // ページの最大値を設定
            _snapScrollView.MaxPage = Enum.GetNames(typeof(EStageLevel)).Length - 1;
            // ページの横幅の設定
            _snapScrollView.PageSize = Screen.width;

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
                MakeButtons(stageLevel);
            }
        }

        /// <summary>
        /// ボタンを配置する
        /// </summary>
        /// <param name="stageLevel"> 配置するボタンの難易度 </param>
        private void MakeButtons(EStageLevel stageLevel)
        {
            var content = GameObject.Find("Canvas/SnapScrollView/Viewport/Content/" + stageLevel + "/ScrollView/Viewport/Content/Buttons").GetComponent<RectTransform>();

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
                button.GetComponent<Image>().color = StageInfo.LevelColor[stageLevel];
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
        private void StageButtonDown(GameObject clickedButton)
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
