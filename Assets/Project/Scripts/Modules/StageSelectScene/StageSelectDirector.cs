﻿using System.Collections.Generic;
using System.Linq;
using SnapScroll;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Networks;
using Treevel.Common.Networks.Objects;
using Treevel.Common.Networks.Requests;
using Treevel.Common.Patterns.Singleton;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene;
using UnityEngine;

namespace Treevel.Modules.StageSelectScene
{
    public class StageSelectDirector : SingletonObject<StageSelectDirector>
    {
        /// <summary>
        /// 概要を表示するポップアップ
        /// </summary>
        [SerializeField] private OverviewPopup _overviewPopup;

        private SnapScrollView _snapScrollView;

        /// <summary>
        /// ロード中の背景
        /// </summary>
        [SerializeField] private GameObject _loadingBackground;

        /// <summary>
        /// ロード中のアニメーション
        /// </summary>
        [SerializeField] private GameObject _loading;

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
        [SerializeField] private GameObject _treeName;

        /// <summary>
        /// 木
        /// </summary>
        private static List<StageTreeController> _trees;

        /// <summary>
        /// 枝
        /// </summary>
        private static List<BranchController> _branches;

        /// <summary>
        /// 左の木に遷移するボタン
        /// </summary>
        [SerializeField] private GameObject _leftButton;

        /// <summary>
        /// 右の木に遷移するボタン
        /// </summary>
        [SerializeField] private GameObject _rightButton;

        private void Awake()
        {
            _trees = GameObject.FindGameObjectsWithTag(Constants.TagName.TREE).Select(tree => tree.GetComponent<StageTreeController>()).ToList<StageTreeController>();
            BranchController.branchStates = PlayerPrefsUtility.GetDictionary<string, bool>(Constants.PlayerPrefsKeys.BRANCH_STATE);
            _branches = GameObject.FindGameObjectsWithTag(Constants.TagName.BRANCH).Select(branch => branch.GetComponent<BranchController>()).ToList<BranchController>();

            // 取得
            _snapScrollView = FindObjectOfType<SnapScrollView>();
            // ページの最大値を設定
            _snapScrollView.MaxPage = LevelInfo.TREE_NUM[levelName] - 1;
            // ページの横幅の設定
            _snapScrollView.PageSize = RuntimeConstants.ScaledCanvasSize.SIZE_DELTA.x;
            // ページ遷移時のイベント登録
            _snapScrollView.OnPageChanged += () => {
                // 木IDを更新
                treeId = (ETreeId)((_snapScrollView.Page + 1) + ((int)treeId / Constants.MAX_TREE_NUM_IN_SEASON));

                // ボタン表示/非表示
                _leftButton.SetActive(_snapScrollView.Page != 0);
                _rightButton.SetActive(_snapScrollView.Page != _snapScrollView.MaxPage);
            };

            // ページの設定
            _snapScrollView.Page = (int)treeId % Constants.MAX_TREE_NUM_IN_SEASON - 1;
            _snapScrollView.RefreshPage(false);

            // TODO: 表示している木の名前を描画する
            DrawTreeName();

            _overviewPopup = _overviewPopup ?? FindObjectOfType<OverviewPopup>();
        }

        /// <summary>
        /// ステージと枝と木の解放状況の更新
        /// </summary>
        private void OnEnable()
        {
            _branches.ForEach(branch => StartCoroutine(branch.UpdateState()));
            _trees.ForEach(tree => tree.UpdateState());
        }

        /// <summary>
        /// ステージと枝と木の状態の保存
        /// </summary>
        private void OnDisable()
        {
            _branches.ForEach(branch => branch.SaveState());
            PlayerPrefsUtility.SetDictionary(Constants.PlayerPrefsKeys.BRANCH_STATE, BranchController.branchStates);
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
            NetworkService.Execute(new GetStageStatsRequest(StageData.EncodeStageIdKey(treeId, stageNumber)), (data) => {
                // ポップアップを初期化する
                _overviewPopup.GetComponent<OverviewPopup>().Initialize(treeId, stageNumber, (StageStats)data);
                // ポップアップを表示する
                _overviewPopup.gameObject.SetActive(true);
            });
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
            AddressableAssetManager.LoadScene(Constants.SceneName.GAME_PLAY_SCENE);
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
            AddressableAssetManager.LoadScene(Constants.SceneName.MENU_SELECT_SCENE);
        }
    }
}