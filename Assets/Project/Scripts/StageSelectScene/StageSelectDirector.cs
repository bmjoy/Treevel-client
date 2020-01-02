using System;
using System.Collections;
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
        /// 概要を表示するポップアップ
        /// </summary>
        [SerializeField] private OverviewPopup _overviewPopup;

        private SnapScrollView _snapScrollView;

        private const string LOADING_BACKGROUND = "LoadingBackground";
        private const string LOADING = "Loading";

        /// <summary>
        /// ロード中の背景
        /// </summary>
        private GameObject _loadingBackground;
        /// <summary>
        /// ロード中のアニメーション
        /// </summary>
        private GameObject _loading;

        /// <summary>
        /// 木のレベル
        /// </summary>
        public static ELevelName levelName;

        /// <summary>
        /// 木のId
        /// </summary>
        public static ETreeName treeId;

        private void Awake()
        {
            // 取得
            _snapScrollView = FindObjectOfType<SnapScrollView>();
            // ページの最大値を設定
            _snapScrollView.MaxPage = Enum.GetNames(typeof(ELevelName)).Length - 1;
            // ページの横幅の設定
            _snapScrollView.PageSize = Screen.width;
            // ロード中背景を非表示にする
            _loadingBackground = GameObject.Find(LOADING_BACKGROUND);
            _loadingBackground.SetActive(false);
            // ロードアニメーションを非表示にする
            _loading = GameObject.Find(LOADING);
            _loading.SetActive(false);
            _overviewPopup = _overviewPopup ?? FindObjectOfType<OverviewPopup>();

            // TODO: 非同期で呼び出す
            // 各ステージの選択ボタンなどを描画する
            Draw();
        }

        /// <summary>
        /// 全難易度の画面を描画する
        /// </summary>
        private void Draw()
        {
            MakeButtons();
        }

        /// <summary>
        /// ボタンを配置する
        /// </summary>
        private void MakeButtons()
        {
            var content = GameObject.Find("Canvas/SnapScrollView/Viewport/Content/" + "Easy" + "/ScrollView/Viewport/Content/Buttons").GetComponent<RectTransform>();

            // TODO: 今後，難易度ごとにボタン配置を変える必要がある
            for (var i = 0; i < TreeInfo.Num[treeId]; i++) {
                // ステージを一意に定めるID
                // TODO: stageIdは(levelName, treeId)から決める(競合しないように未実装にしてある)
                var stageId = LevelInfo.StageStartId[levelName] + i;
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
                button.GetComponent<Image>().color = LevelInfo.LevelColor[levelName];
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
        /// タッチされたステージの概要を表示
        /// </summary>
        /// <param name="clickedButton"> タッチされたボタン </param>
        private void StageButtonDown(GameObject clickedButton)
        {
            // タップされたステージidを取得（暫定的にボタンの名前）
            var stageId = int.Parse(clickedButton.name);

            if (PlayerPrefs.GetInt(PlayerPrefsKeys.DO_NOT_SHOW, 0) == 0) {
                // ポップアップを初期化する
                _overviewPopup.GetComponent<OverviewPopup>().SetStageId(stageId);
                // ポップアップを表示する
                _overviewPopup.gameObject.SetActive(true);
            } else {
                GoToGame(stageId);
            }
        }

        /// <summary>
        /// ステージ選択画面からゲーム選択画面へ移動する
        /// </summary>
        /// <param name="stageId"> ステージID </param>
        public void GoToGame(int stageId)
        {
            // 挑戦回数をインクリメント
            var ss = StageStatus.Get(stageId);
            ss.IncChallengeNum(stageId);
            // ステージ情報を渡す
            GamePlayDirector.levelName = levelName;
            GamePlayDirector.treeId = treeId;
            GamePlayDirector.stageId = stageId;
            // ロード中の背景を表示する
            _loadingBackground.SetActive(true);
            // ロード中のアニメーションを開始する
            _loading.SetActive(true);
            // シーン遷移
            StartCoroutine(LoadGamePlayScene());
        }

        /// <summary>
        /// GamePlaySceneに遷移する
        /// ロード中にアニメーションを動かすために非同期にロードする
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadGamePlayScene()
        {
            var async = SceneManager.LoadSceneAsync(SceneName.GAME_PLAY_SCENE);
            while (!async.isDone) {
                yield return null;
            }
        }
    }
}
