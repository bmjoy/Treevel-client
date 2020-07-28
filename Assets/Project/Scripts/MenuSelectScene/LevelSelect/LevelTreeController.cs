using System;
using System.Linq;
using Project.Scripts.StageSelectScene;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene.LevelSelect
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class LevelTreeController : TreeController
    {
        /// <summary>
        /// 木のレベル
        /// </summary>
        [SerializeField] private ELevelName _levelName;

        [SerializeField] private Material _material;

        [SerializeField] private Button _button;

        public override void UpdateState()
        {
            // 現在状態をDBから得る
            state = (ETreeState) Enum.ToObject(typeof(ETreeState), PlayerPrefs.GetInt(PlayerPrefsKeys.TREE + treeId.ToString(), Default.TREE_STATE));
            // 状態の更新
            switch (state) {
                case ETreeState.Unreleased:
                    break;
                case ETreeState.Released:
                    // Implementorに任せる
                    state = _clearHandler.GetTreeState();
                    break;
                case ETreeState.Cleared:
                    // 全クリアかどうかをチェックする
                    var stageNum = TreeInfo.NUM[treeId];
                    var clearStageNum = Enumerable.Range(1, stageNum).Count(s => StageStatus.Get(treeId, s).state == EStageState.Cleared);
                    state = clearStageNum == stageNum ? ETreeState.AllCleared : state;
                    break;
                case ETreeState.AllCleared:
                    break;
                default:
                    throw new NotImplementedException();
            }

            // 状態の反映
            ReflectTreeState();
        }

        public void Reset()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.TREE + treeId.ToString());
        }

        protected override void ReflectUnreleasedState()
        {
            // グレースケール
            GetComponent<Image>().material = _material;
            _button.enabled = false;
        }

        protected override void ReflectReleasedState()
        {
            GetComponent<Image>().material = null;
            _button.enabled = true;
        }

        protected override void ReflectClearedState()
        {
            GetComponent<Image>().material = null;
            _button.enabled = true;
            // アニメーション
            Debug.Log($"{treeId} is cleared.");
        }

        protected override void ReflectAllClearedState()
        {
            GetComponent<Image>().material = null;
            _button.enabled = true;
            // アニメーション
            Debug.Log($"{treeId} is all cleared.");
        }

        /// <summary>
        /// 木が押されたとき
        /// </summary>
        public void TreeButtonDown()
        {
            StageSelectDirector.levelName = _levelName;
            StageSelectDirector.treeId = treeId;
            TreeLibrary.LoadStageSelectScene(_levelName);
        }

        /// <summary>
        /// 木の状態の保存
        /// </summary>
        public void SaveState()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.TREE + treeId.ToString(), Convert.ToInt32(state));
        }
    }
}
