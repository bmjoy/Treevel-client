using Project.Scripts.GamePlayScene;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using Project.Scripts.Utils.PlayerPrefsUtils;
using SnapScroll;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Scripts.StageSelectScene
{
    public class StageSelectDirector : SingletonObject<StageSelectDirector>
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
        /// 木のID
        /// </summary>
        public static ETreeId treeId;

        /// <summary>
        /// 表示している木の名前
        /// </summary>
        private GameObject _treeName;

        /// <summary>
        /// ステージ
        /// </summary>
        private static List<StageController> stages;

        /// <summary>
        /// 枝
        /// </summary>
        private static List<BranchController> branches;

        // TODO: SnapScrollが働いたときの更新処理
        //       - 隣の木に移動するButtonが押せるかどうか(端では押せない)を更新する
        //       - 木の名前を表示するテキストを更新する
        //       - 選択している木のidを更新する

        private void Awake()
        {
            stages = GameObject.FindGameObjectsWithTag(TagName.STAGE).Select(stage => stage.GetComponent<StageController>()).ToList<StageController>();
            branches = GameObject.FindGameObjectsWithTag(TagName.BRANCH).Select(branch => branch.GetComponent<BranchController>()).ToList<BranchController>();

            // ステージの状態の更新
            foreach (var stage in stages) {
                stage.UpdateReleased();
            }
            // 枝の状態の更新
            foreach (var branch in branches) {
                branch.UpdateReleased();
            }

            // 取得
            _snapScrollView = FindObjectOfType<SnapScrollView>();
            // ページの最大値を設定
            _snapScrollView.MaxPage = LevelInfo.TREE_NUM[levelName] - 1;
            // ページの横幅の設定
            _snapScrollView.PageSize = ScaledCanvasSize.SIZE_DELTA.x;

            // UIの設定
            _treeName = GameObject.Find(_TREENAME);

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
        /// 表示している木の名前を描画する
        /// </summary>
        private void DrawTreeName()
        {
            // TODO: 現在表示している木の名前に更新する
        }

        public void ShowOverPopup(ETreeId treeId, int stageNumber)
        {
            // ポップアップを初期化する
            _overviewPopup.GetComponent<OverviewPopup>().SetStageId(treeId, stageNumber);
            // ポップアップを表示する
            _overviewPopup.gameObject.SetActive(true);
        }

        /// <summary>
        /// ステージ選択画面からゲーム選択画面へ移動する
        /// </summary>
        public void GoToGame(ETreeId treeId, int stageNumber)
        {
            // 挑戦回数をインクリメント
            var ss = StageStatus.Get(treeId, stageNumber);
            ss.IncChallengeNum(treeId, stageNumber);
            // ステージ情報を渡す
            GamePlayDirector.levelName = levelName;
            GamePlayDirector.treeId = treeId;
            GamePlayDirector.stageNumber = stageNumber;
            // ポップアップを非表示にする
            _overviewPopup.gameObject.SetActive(false);
            // ロード中の背景を表示する
            _loadingBackground.SetActive(true);
            // ロード中のアニメーションを開始する
            _loading.SetActive(true);

            AddressableAssetManager.LoadStageDependencies(treeId, stageNumber);

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
