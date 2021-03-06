using System;
using Cysharp.Threading.Tasks;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using UnityEngine;

namespace Treevel.Modules.MenuSelectScene.LevelSelect
{
    public abstract class TreeControllerBase : MonoBehaviour
    {
        /// <summary>
        /// 現在の木の状態
        /// </summary>
        [NonSerialized] public ETreeState state = ETreeState.Unreleased;

        /// <summary>
        /// クリア状態を判定するクラス
        /// </summary>
        protected IClearTreeHandler clearHandler;

        /// <summary>
        /// 木のId
        /// </summary>
        [SerializeField] public ETreeId treeId;

        public virtual void Awake()
        {
            // クリア条件を実装するクラスを指定する
            clearHandler = GameDataManager.GetTreeData(treeId).GetClearTreeHandler();
        }

        /// <summary>
        /// 木の状態の更新
        /// </summary>
        public abstract void UpdateState();

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
    }
}
