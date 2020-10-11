using Treevel.Common.Entities;
using Treevel.Common.Utils;
using Treevel.Modules.MenuSelectScene.LevelSelect;
using Treevel.Modules.StageSelectScene;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.MenuSelectScene.Settings
{
    public class ResetController : MonoBehaviour
    {
        /// <summary>
        /// ステージリセットボタン
        /// </summary>
        private Button _resetButton;

        private void Awake()
        {
            _resetButton = GetComponent<Button>();
            _resetButton.onClick.AddListener(ResetButtonDown);
        }

        /// <summary>
        /// ステージリセットボタンを押した場合の処理
        /// </summary>
        private static void ResetButtonDown()
        {
            // 全ステージをリセット
            StageStatus.Reset();

            // 記録情報をリセット
            RecordData.Instance.Reset();

            // 道の解放条件をリセット
            LevelSelectDirector.Reset();
            // 枝の解放条件をリセット(同一シーンに存在しないため、シーン開始時にリセットする)
            BranchController.Reset();

            // キャンバスの設定をリセット
            UserSettings.LevelSelectCanvasScale = Default.LEVEL_SELECT_CANVAS_SCALE;
            UserSettings.LevelSelectScrollPosition = Default.LEVEL_SELECT_SCROLL_POSITION;
        }
    }
}
