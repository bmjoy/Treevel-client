using Treevel.Common.Components;
using Treevel.Common.Entities.GameDatas;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Tile
{
    [RequireComponent(typeof(GameSpriteRendererUnifier))]
    public abstract class TileControllerBase : GameObjectControllerBase
    {
        /// <summary>
        /// タイルの番号
        /// </summary>
        public int TileNumber { get; private set; }

        protected IBottleHandler bottleHandler = new DefaultBottleHandler();

        protected virtual void Awake() { }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="tileNum"> タイルの番号 </param>
        public virtual void Initialize(TileData tileData)
        {
            Initialize(tileData.number);
        }

        public virtual void Initialize(int tileNum)
        {
            TileNumber = tileNum;
            GetComponent<SpriteRendererUnifier>().Unify();
            GamePlayDirector.Instance.StagePrepared.Subscribe(_ => {
                // 表示
                GetComponent<SpriteRenderer>().enabled = true;
            }).AddTo(compositeDisposableOnGameEnd, this);
        }

        /// <summary>
        /// タイルにボトルが入ってきた場合の処理
        /// </summary>
        /// <param name="bottle"> タイルに入ってきたボトル </param>
        /// <param name="direction"> 入ってきた方向（単一方向の単位ベクトル） </param>
        public void OnBottleEnter(GameObject bottle, Vector2Int? direction)
        {
            bottleHandler.OnBottleEnter(bottle, direction);
        }

        public void OnBottleExit(GameObject bottle)
        {
            bottleHandler.OnBottleExit(bottle);
        }

        public void OnGameStart(GameObject bottle)
        {
            bottleHandler.OnGameStart(bottle);
        }

        protected interface IBottleHandler
        {
            void OnGameStart(GameObject bottle);
            void OnBottleEnter(GameObject bottle, Vector2Int? direction);
            void OnBottleExit(GameObject bottle);
        }

        // 何もしないボトルハンドラー
        protected class DefaultBottleHandler : IBottleHandler
        {
            public virtual void OnGameStart(GameObject bottle) { }

            public virtual void OnBottleEnter(GameObject bottle, Vector2Int? direction) { }

            public virtual void OnBottleExit(GameObject bottle) { }
        }
    }
}
