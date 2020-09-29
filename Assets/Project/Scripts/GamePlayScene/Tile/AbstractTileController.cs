using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
    public abstract class AbstractTileController : MonoBehaviour
    {
        /// <summary>
        /// タイルの番号
        /// </summary>
        public int TileNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// 初期配置された際に `OnBottleEnter` を実行するかどうか
        /// </summary>
        public abstract bool RunOnBottleEnterAtInit { get; }

        protected IBottleHandler bottleHandler = new DefaultBottleHandler();

        protected virtual void Awake()
        {

        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="tileNum"> タイルの番号 </param>
        public virtual void Initialize(int tileNum)
        {
            TileNumber = tileNum;
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

        protected interface IBottleHandler
        {
            void OnBottleEnter(GameObject bottle, Vector2Int? direction);
            void OnBottleExit(GameObject bottle);
        }

        // 何もしないボトルハンドラー
        protected class DefaultBottleHandler : IBottleHandler
        {
            public virtual void OnBottleEnter(GameObject bottle, Vector2Int? direction) {}

            public virtual void OnBottleExit(GameObject bottle) {}
        }
    }
}
