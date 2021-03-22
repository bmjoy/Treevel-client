using Cysharp.Threading.Tasks.Triggers;
using Treevel.Common.Components;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    public class GoalTileController : NormalTileController
    {
        /// <summary>
        /// タイルの色
        /// </summary>
        public EGoalColor GoalColor { get; private set; }

        /// <summary>
        /// 傷オブジェクト
        /// </summary>
        [SerializeField] private SpriteRenderer _wound;

        /// <summary>
        /// タイル画像に重ねる色画像
        /// </summary>
        [SerializeField] private SpriteRenderer _mainColorLayer;

        public override void Initialize(TileData tileData)
        {
            Initialize(tileData.number);
            GoalColor = tileData.goalColor;
            // colorがNoneではないことを保証する
            if (GoalColor == EGoalColor.None) UIManager.Instance.ShowErrorMessage(EErrorCode.InvalidTileColor);
            _wound.sprite = AddressableAssetManager.GetAsset<Sprite>(tileData.goalColor.GetTileAddress());
            _wound.color = GoalColor.GetMainColor();
            _mainColorLayer.color = GoalColor.GetMainColor(_mainColorLayer.color.a);
            bottleHandler = new GoalTileBottleHandler(this);
            #if UNITY_EDITOR
            name = Constants.TileName.GOAL_TILE + tileData.number;
            #endif
        }

        private sealed class GoalTileBottleHandler : DefaultBottleHandler
        {
            private readonly EGoalColor _goalColor;
            private static string _ANIMATOR_PARAM_BOOL_IS_SUCCEEDED = "IsSucceeded";
            private static string _ANIMATOR_PARAM_BOOL_IS_GAME_STARTED = "IsGameStarted";
            private readonly Animator _animator;

            public GoalTileBottleHandler(GoalTileController goalTile)
            {
                _goalColor = goalTile.GoalColor;
                _animator = goalTile.GetComponent<Animator>();
            }

            public override void OnGameStart(GameObject bottle)
            {
                if (bottle != null) OnBottleEnter(bottle, null);
                _animator.SetBool(_ANIMATOR_PARAM_BOOL_IS_GAME_STARTED, true);
            }

            public override void OnBottleEnter(GameObject bottle, Vector2Int? direction)
            {
                if (bottle == null) return;
                var bottleController = bottle.GetComponent<GoalBottleController>();
                if (bottleController == null) return;
                // 成功状態の演出
                if (bottleController.GoalColor == _goalColor) _animator.SetBool(_ANIMATOR_PARAM_BOOL_IS_SUCCEEDED, true);
            }

            public override void OnBottleExit(GameObject bottle)
            {
                // Not 成功状態の演出
                _animator.SetBool(_ANIMATOR_PARAM_BOOL_IS_SUCCEEDED, false);
            }
        }
    }
}
