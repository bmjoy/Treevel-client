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
        /// 木
        /// </summary>
        private static List<StageTreeController> _trees;

        /// <summary>
        /// ステージ
        /// </summary>
        private static List<StageController> _stages;

        /// <summary>
        /// 枝
        /// </summary>
        private static List<BranchController> _branches;

        /// <summary>
        /// 左の木に遷移するボタン
        /// </summary>
        private GameObject _leftButton;

        /// <summary>
        /// 右の木に遷移するボタン
        /// </summary>
        private GameObject _rightButton;

        /// <summary>
        /// シーン起動時にリセットするかどうか
        /// </summary>
        public static bool resetFlag = true;

        private void Awake()
        {
            _trees = GameObject.FindGameObjectsWithTag(TagName.TREE).Select(tree => tree.GetComponent<StageTreeController>()).ToList<StageTreeController>();
            _stages = GameObject.FindGameObjectsWithTag(TagName.STAGE).Select(stage => stage.GetComponent<StageController>()).ToList<StageController>();
            _branches = GameObject.FindGameObjectsWithTag(TagName.BRANCH).Select(branch => branch.GetComponent<BranchController>()).ToList<BranchController>();
            if(resetFlag) Reset();

            _leftButton = GameObject.Find("LeftButton");
            _rightButton = GameObject.Find("RightButton");

            // 取得
            _snapScrollView = FindObjectOfType<SnapScrollView>();
            // ページの最大値を設定
            _snapScrollView.MaxPage = LevelInfo.TREE_NUM[levelName] - 1;
            // ページの横幅の設定
            _snapScrollView.PageSize = ScaledCanvasSize.SIZE_DELTA.x;
            // ページ遷移時のイベント登録
            _snapScrollView.OnPageChanged += () => {
                // 木IDを更新
                treeId = (ETreeId)((_snapScrollView.Page + 1) + ((int)treeId / TreeInfo.MAX_TREE_NUM_IN_SEASON));

                // ボタン表示/非表示
                _leftButton.SetActive(_snapScrollView.Page != 0);
                _rightButton.SetActive(_snapScrollView.Page != _snapScrollView.MaxPage);
            };

            // ページの設定
            _snapScrollView.Page = (int)treeId % TreeInfo.MAX_TREE_NUM_IN_SEASON - 1;
            _snapScrollView.RefreshPage(false);

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
        /// ステージと枝と木の解放状況の更新
        /// </summary>
        private void OnEnable()
        {
            _stages.ForEach(stage => stage.UpdateState());
            _branches.ForEach(branch => branch.UpdateState());
            _trees.ForEach(tree => tree.UpdateState());
        }

        /// <summary>
        /// ステージと枝と木の状態の保存
        /// </summary>
        private void OnDisable()
        {
            _branches.ForEach(branch => branch.SaveState());
        }

        /// <summary>
        /// リセットフラグを立てる
        /// </summary>
        public static void PrepareReset()
        {
            resetFlag = true;
        }        

        /// <summary>
        /// 枝の状態のリセット
        /// </summary>
        private void Reset()
        {
            _branches.ForEach(branch => branch.Reset());
            resetFlag = false;
        }        

        /// <summary>
        /// ページ移動
        /// </summary>
        /// <param name="displacement"> ページ数の増減量 </param>
        private void MovePage(int displacement)
        {
            _snapScrollView.Page += displacement;
            _snapScrollView.Page = Mathf.Clamp(_snapScrollView.Page, 0, _snapScrollView.MaxPage);
            _snapScrollView.RefreshPage();
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
            // ページ数-1
            MovePage(-1);
        }

        /// <summary>
        /// 1つidの大きい木を表示する
        /// </summary>
        public void RightButtonDown()
        {
            // ページ数+1
            MovePage(1);
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
