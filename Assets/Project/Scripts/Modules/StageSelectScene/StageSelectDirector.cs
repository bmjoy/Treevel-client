using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using SnapScroll;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using Treevel.Common.Networks.Objects;
using Treevel.Common.Patterns.Singleton;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene;
using UnityEngine;

namespace Treevel.Modules.StageSelectScene
{
    public class StageSelectDirector : SingletonObjectBase<StageSelectDirector>
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
        public static ESeasonId seasonId;

        /// <summary>
        /// 木のID
        /// </summary>
        public static ETreeId treeId;

        /// <summary>
        /// 木
        /// </summary>
        private static List<StageTreeController> _trees;

        /// <summary>
        /// 枝
        /// </summary>
        private static List<BranchController> _branches;

        /// <summary>
        /// バナー広告のビュー
        /// </summary>
        private BannerView _banner;

        private void Awake()
        {
            _trees = GameObject.FindGameObjectsWithTag(Constants.TagName.TREE)
                .Select(tree => tree.GetComponent<StageTreeController>()).ToList();
            BranchController.animationPlayedBranches = PlayerPrefsUtility.GetList<string>(Constants.PlayerPrefsKeys.BRANCH_STATE);
            _branches = GameObject.FindGameObjectsWithTag(Constants.TagName.BRANCH)
                .Select(branch => branch.GetComponent<BranchController>()).ToList();

            // 取得
            _snapScrollView = FindObjectOfType<SnapScrollView>();
            // ページの最大値を設定
            _snapScrollView.MaxPage = seasonId.GetTreeNum() - 1;
            // ページの横幅の設定
            _snapScrollView.PageSize = RuntimeConstants.SCALED_CANVAS_SIZE.x;

            // ページの設定
            _snapScrollView.Page = (int)treeId % Constants.MAX_TREE_NUM_IN_SEASON - 1;
            _snapScrollView.RefreshPage(false);

            _overviewPopup = _overviewPopup ?? FindObjectOfType<OverviewPopup>();

            // バナー広告を表示
            _banner = AdvertiseService.ShowBanner(Constants.MobileAds.BANNER_UNIT_ID_STAGE_SELECT, AdPosition.Bottom);
        }

        private void OnDestroy()
        {
            // バナー広告を削除
            _banner.Destroy();
        }

        /// <summary>
        /// ステージと枝と木の解放状況の更新
        /// </summary>
        private void OnEnable()
        {
            SoundManager.Instance.PlayBGM(seasonId.GetStageSelectBGM());

            _branches.ForEach(branch => branch.UpdateState());
            _trees.ForEach(tree => tree.UpdateState());
        }

        /// <summary>
        /// ステージと枝と木の状態の保存
        /// </summary>
        private void OnDisable()
        {
            PlayerPrefsUtility.SetList(Constants.PlayerPrefsKeys.BRANCH_STATE, BranchController.animationPlayedBranches);
        }

        public void ShowOverPopup(ETreeId treeId, int stageNumber)
        {
            // TODO: implement PlayFab version
            // NetworkService.Execute(new GetStageStatsRequest(StageData.EncodeStageIdKey(treeId, stageNumber)))
            //     .ToObservable()
            //     .Subscribe(data => {
            //         // ポップアップを初期化する
            //         _overviewPopup.GetComponent<OverviewPopup>().Initialize(treeId, stageNumber, (StageStats)data);
            //         // ポップアップを表示する
            //         _overviewPopup.gameObject.SetActive(true);
            //     })
            //     .AddTo(this);
            // ポップアップを初期化する
            _overviewPopup.GetComponent<OverviewPopup>().Initialize(treeId, stageNumber, new StageStats());
            // ポップアップを表示する
            _overviewPopup.gameObject.SetActive(true);
        }

        /// <summary>
        /// ステージ選択画面からゲーム選択画面へ移動する
        /// </summary>
        public async UniTask GoToGameAsync(ETreeId treeId, int stageNumber)
        {
            // ステージ情報を渡す
            GamePlayDirector.seasonId = seasonId;
            GamePlayDirector.treeId = treeId;
            GamePlayDirector.stageNumber = stageNumber;
            // ポップアップを非表示にする
            _overviewPopup.gameObject.SetActive(false);
            // ロード中の背景を表示する
            _loadingBackground.SetActive(true);
            // ロード中のアニメーションを開始する
            _loading.SetActive(true);

            await AddressableAssetManager.LoadStageDependenciesAsync(treeId, stageNumber);

            // シーン遷移
            AddressableAssetManager.LoadScene(Constants.SceneName.GAME_PLAY_SCENE);
        }

        /// <summary>
        /// LevelSelectSceneに戻る
        /// </summary>
        public void BackButtonDown()
        {
            SoundManager.Instance.PlaySE(ESEKey.UI_Button_Click_General);
            SoundManager.Instance.PlayBGM(EBGMKey.MenuSelect);
            AddressableAssetManager.LoadScene(Constants.SceneName.MENU_SELECT_SCENE);
        }
    }
}
