using System.Collections.Generic;
using System.Linq;
using Treevel.Common.Entities;
using Treevel.Common.Utils;
using Treevel.Modules.MenuSelectScene.LevelSelect;
using Treevel.Modules.MenuSelectScene.Settings;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.StageSelectScene
{
    public class BranchController : LineController
    {
        /// <summary>
        /// ステージが属する木
        /// </summary>
        [SerializeField] private ETreeId _treeId;

        private StageController _endObjectController;

        public static Dictionary<string, bool> branchStates;

        protected override void Awake()
        {
            base.Awake();
            _endObjectController = endObject.GetComponent<StageController>();
            FindObjectOfType<ResetController>().OnResetData.Subscribe(_ => Reset()).AddTo(this);
        }

        protected override void SetSaveKey()
        {
            saveKey = $"{_treeId}{Constants.PlayerPrefsKeys.KEY_CONNECT_CHAR}{startObject.GetComponent<StageController>().stageNumber}{Constants.PlayerPrefsKeys.KEY_CONNECT_CHAR}{endObject.GetComponent<StageController>().stageNumber}";
        }

        private void Reset()
        {
            PlayerPrefs.DeleteKey(Constants.PlayerPrefsKeys.BRANCH_STATE);
        }

        /// <summary>
        /// 枝の状態の更新
        /// </summary>
        public override void UpdateState()
        {
            if (branchStates.ContainsKey(saveKey))
                released = branchStates[saveKey];
            else
                released = false;

            if (!released) {
                if (constraintObjects.Length == 0) {
                    // 初期状態で解放されている枝
                    released = true;
                } else {
                    released = constraintObjects.All(stage => stage.GetComponent<StageController>().state >= EStageState.Cleared);
                }
                if (released) {
                    // 終点のステージの状態の更新
                    _endObjectController.ReleaseStage();
                    _endObjectController.ReflectTreeState();
                }
            }

            if (!released) {
                // 非解放時
                lineRenderer.startColor = new Color(0.2f, 0.2f, 0.7f);
                lineRenderer.endColor = new Color(0.2f, 0.2f, 0.7f);
            }
        }

        /// <summary>
        /// 枝の状態の保存
        /// </summary>
        public override void SaveState()
        {
            branchStates[saveKey] = released;
        }
    }
}
