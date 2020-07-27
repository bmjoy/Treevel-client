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
    public abstract class TreeController : MonoBehaviour
    {
        /// <summary>
        /// 現在の木の状態
        /// </summary>
        [NonSerialized] public ETreeState state = ETreeState.Unreleased;

        /// <summary>
        /// クリア状態を判定するクラス
        /// </summary>
        [SerializeField] private IClearTreeHandler _clearHandler;

        /// <summary>
        /// 木のId
        /// </summary>
        [SerializeField] public ETreeId treeId;

        public void Awake()
        {
            // クリア条件を実装するクラスを指定する
            _clearHandler = TreeInfo.CLEAR_HANDLER[treeId];
        }

        /// <summary>
        /// 木の状態の更新
        /// </summary>
        public void UpdateState()
        {
            // 現在状態をDBから得る
            state = (ETreeState) Enum.ToObject(typeof(ETreeState), PlayerPrefs.GetInt(PlayerPrefsKeys.TREE + treeId.ToString(), Default.TREE_STATE));
            // 状態の更新
            switch (state) {
                case ETreeState.Unreleased: {
                        break;
                    }
                case ETreeState.Released: {
                        // Implementorに任せる
                        state = _clearHandler.GetTreeState();
                        break;
                    }
                case ETreeState.Cleared: {
                        // 全クリアかどうかをチェックする
                        var stageNum = TreeInfo.NUM[treeId];
                        var clearStageNum = Enumerable.Range(1, stageNum).Count(s => StageStatus.Get(treeId, s).cleared);
                        state = clearStageNum == stageNum ? ETreeState.AllCleared : state;
                        break;
                    }
                case ETreeState.AllCleared: {
                        break;
                    }
                default: {
                        throw new NotImplementedException();
                    }
            }

            // 状態の反映
            ReflectTreeState();
        }

        /// <summary>
        /// 木の状態を見た目や動作に反映
        /// </summary>
        public void ReflectTreeState()
        {
            switch (state) {
                case ETreeState.Unreleased:
                    ReflectUnreleasedState();
                    break;
                case ETreeState.Released:
                    ReflectReleasedState();
                    break;
                case ETreeState.Cleared:
                    ReflectClearedState();
                    break;
                case ETreeState.AllCleared:
                    ReflectAllClearedState();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 非解放状態の処理
        /// </summary>
        protected abstract void ReflectUnreleasedState();
        /// <summary>
        /// 解放状態の処理
        /// </summary>
        protected abstract void ReflectReleasedState();
        /// <summary>
        /// クリア状態の処理
        /// </summary>
        protected abstract void ReflectClearedState();
        /// <summary>
        /// 全ステージクリア状態の処理
        /// </summary>
        protected abstract void ReflectAllClearedState();

        /// <summary>
        /// 木の状態の保存
        /// </summary>
        public void SaveState()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.TREE + treeId.ToString(), Convert.ToInt32(state));
        }
    }
}
