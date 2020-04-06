using System;
using System.Collections;
using Project.Scripts.GamePlayScene;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using SnapScroll;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene
{
    public class StageSelectDirector : MonoBehaviour
    {
        /// <summary>
        /// 概要を表示するポップアップ
        /// </summary>
        [SerializeField] private OverviewPopup _overviewPopup;

        private SnapScrollView _snapScrollView;

        private const string _LOADING_BACKGROUND = "LoadingBackground";
        private const string _LOADING = "Loading";
        private const string _TREENAME = "TreeName";
        private const string _TREECANVAS = "TreeCanvas";

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
        public static ETreeId treeId;

        /// <summary>
        /// 表示している木の名前
        /// </summary>
        private GameObject _treeName;

        // TODO: SnapScrollが働いたときの更新処理
        //       - 隣の木に移動するButtonが押せるかどうか(端では押せない)を更新する
        //       - 木の名前を表示するテキストを更新する
        //       - 選択している木のidを更新する

        private void Awake()
        {
            // 取得
            _snapScrollView = FindObjectOfType<SnapScrollView>();
            // ページの最大値を設定
            _snapScrollView.MaxPage = LevelInfo.TREE_NUM[levelName] - 1;
            // ページの横幅の設定
            StartCoroutine(SetPageSize());

            // UIの設定
            _treeName = GameObject.Find(_TREENAME);

            // TODO: 非同期で呼び出す
            // 各ステージの選択ボタンなどを描画する
            Draw();

            // TODO: 表示している木の名前を描画する
            DrawTreeName();

            // ロード中背景を非表示にする
            _loadingBackground = GameObject.Find(_LOADING_BACKGROUND);
            _loadingBackground.SetActive(false);
            // ロードアニメーションを非表示にする
            _loading = GameObject.Find(_LOADING);
            _loading.SetActive(false);
            _overviewPopup = _overviewPopup ?? FindObjectOfType<OverviewPopup>();
        }

        /// <summary>
        /// リサイズ後のキャンバスサイズを取得してSnapScrollViewのPageSizeを変更する
        /// </summary>
        /// <returns></returns>
        private IEnumerator SetPageSize()
        {
            // リサイズが終わるまで待つ
            yield return new WaitForEndOfFrame();
            _snapScrollView.PageSize = GameObject.Find(_TREECANVAS).GetComponent<RectTransform> ().sizeDelta.x;
        }

        /// <summary>
        /// 全難易度の画面を描画する
        /// </summary>
        private void Draw()
        {
            MakeButtons();
        }

        /// <summary>
        /// 表示している木の名前を描画する
        /// </summary>
        private void DrawTreeName()
        {
            // TODO: 現在表示している木の名前に更新する
        }

        /// <summary>
        /// ボタンを配置する
        /// </summary>
        private void MakeButtons()
        {
            // 指定したレベルの全ての木の描画
            for (var i = 0; i < LevelInfo.TREE_NUM[levelName]; i++) {
                // TODO: 選択した木を開くようにScrollViewを変更する
                for (var j = 0; j < TreeInfo.NUM[treeId]; j++) {
                    // ButtonのGameObjectを取得する
                    var button = GameObject.Find($"{_TREECANVAS}/Trees/SnapScrollView/Viewport/Content/Tree{i+1}/Stage{j+1}");
                    // クリック時のリスナー
                    button.GetComponent<Button>().onClick.AddListener(() => StageButtonDown(button));
                    // TODO: ステージを選択できるか、ステージをクリアしたかどうかでButtonの表示を変更する
                }
                // TODO: ButtonとButtonの間に線を描画する
            }
        }

        /// <summary>
        /// タッチされたステージの概要を表示
        /// </summary>
        /// <param name="clickedButton"> タッチされたボタン </param>
        private void StageButtonDown(GameObject clickedButton)
        {
            // タップされたステージidを取得（暫定的にButtonのテキスト）
            Debug.Log(clickedButton);
            var stageId = int.Parse(clickedButton.GetComponentInChildren<Text>().text);

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
            // ポップアップを非表示にする
            _overviewPopup.gameObject.SetActive(false);
            // ロード中の背景を表示する
            _loadingBackground.SetActive(true);
            // ロード中のアニメーションを開始する
            _loading.SetActive(true);

            AddressableAssetManager.LoadStageDependencies(stageId);

            // シーン遷移
            AddressableAssetManager.LoadScene(SceneName.GAME_PLAY_SCENE);
        }

        /// <summary>
        /// 1つidの小さい木を表示する
        /// </summary>
        public void LeftButtonDown()
        {
            // TODO: 実装
        }

        /// <summary>
        /// 1つidの大きい木を表示する
        /// </summary>
        public void RightButtonDown()
        {
            // TODO: 実装
        }

        /// <summary>
        /// LevelSelectSceneに戻る
        /// </summary>
        public void BackButtonDown()
        {
            AddressableAssetManager.LoadScene(SceneName.MENU_SELECT_SCENE);
        }
    }
}
