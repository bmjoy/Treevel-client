using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Networks;
using Treevel.Common.Networks.Requests;
using Treevel.Common.Utils;
using Treevel.Modules.MenuSelectScene.LevelSelect;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.StageSelectScene
{
    public class StageTreeController : TreeControllerBase
    {
        [SerializeField] private Image _treeImage;

        /// <summary>
        /// 画像に乗せるマテリアル(デフォルトでは何もなし)
        /// </summary>
        [SerializeField] private Material _material;

        /// <summary>
        /// 木全体を覆うオブジェクト(デフォルトでは表示されていない)
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
            _constraintTreeClearHandlers = _constraintTrees.Select(id => id.GetClearTreeHandler()).ToList();
        }

        public override async UniTask UpdateStateAsync()
        {
            // 現在状態をPlayerPrefsから得る
            state = (ETreeState)Enum.ToObject(typeof(ETreeState),
                                              PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.TREE + treeId,
                                                                 Default.TREE_STATE));

            // 非解放状態の時、自身を制約する木の解放状態に応じて自身の解放状態を更新する
            if (state == ETreeState.Unreleased) {
                var released = false;
                if (!_constraintTrees.Any()) {
                    // 初期状態で解放されている道
                    released = true;
                } else {
                    released = _constraintTreeClearHandlers.All(
                        handler => handler.GetTreeState() >= ETreeState.Cleared);
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
                    state = clearHandler.GetTreeState();
                    break;
                }
                case ETreeState.Cleared: {
                    // 全クリアかどうかをチェックする
                    var stageNum = treeId.GetStageNum();
                    var stageRecords = StageRecordService.Instance.Get(treeId);
                    var clearStageNum = stageRecords.Count(stageRecord => stageRecord.IsCleared);
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
            _mask.SetActive(true);
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
