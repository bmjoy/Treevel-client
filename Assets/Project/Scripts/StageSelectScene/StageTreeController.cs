using System.Collections.Generic;
using System;
using System.Linq;
using Project.Scripts.MenuSelectScene.LevelSelect;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene
{
    public class StageTreeController : TreeController
    {
        [SerializeField] private Image _treeImage;

        /// <summary>
        /// 画像に乗せるマテリアル(デフォルトでは何もなし)
        /// </summary>
        [SerializeField] private Material _material;

        /// <summary>
        /// 木全体を覆うオブジェクト(デフォルトでは表示されている)
        /// </summary>
        [SerializeField] private GameObject _mask;

        /// <summary>
        /// 解放の制約となる木
        /// </summary>
        [SerializeField] private List<ETreeId> _constraintTrees;

        /// <summary>
        /// 解放の制約となる木のクリア条件
        /// </summary>
        private List<IClearTreeHandler> _constraintTreeClearHandlers;

        public override void Awake()
        {
            base.Awake();
            _constraintTreeClearHandlers = _constraintTrees.Select(id => TreeInfo.CLEAR_HANDLER[id]).ToList<IClearTreeHandler>();
        }

        public override void UpdateState()
        {
            // 現在状態をDBから得る
            state = (ETreeState) Enum.ToObject(typeof(ETreeState), PlayerPrefs.GetInt(PlayerPrefsKeys.TREE + treeId.ToString(), Default.TREE_STATE));

            // 非解放状態の時、自身を制約する木の解放状態に応じて自身の解放状態を更新する
            if (state == ETreeState.Unreleased) {
                var released = false;
                if (_constraintTrees.Count() == 0) {
                    // 初期状態で解放されている道
                    released = true;
                } else {
                    released = _constraintTreeClearHandlers.All(handler => handler.GetTreeState() >= ETreeState.Cleared);
                }
                state = released ? ETreeState.Released : ETreeState.Unreleased;
            }

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
                        var clearStageNum = Enumerable.Range(1, stageNum).Count(s => StageStatus.Get(treeId, s).state == EStageState.Cleared);
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

        protected override void ReflectUnreleasedState()
        {
            // グレースケール
            _treeImage.material = _material;
        }

        protected override void ReflectReleasedState()
        {
            _mask.SetActive(false);
        }

        protected override void ReflectClearedState()
        {
            _mask.SetActive(false);
            // アニメーション
            Debug.Log($"{treeId} is cleared.");
        }

        protected override void ReflectAllClearedState()
        {
            _mask.SetActive(false);
            // アニメーション
            Debug.Log($"{treeId} is all cleared.");
        }
    }
}
