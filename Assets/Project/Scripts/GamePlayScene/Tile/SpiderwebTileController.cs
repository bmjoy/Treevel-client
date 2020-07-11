using System.Threading.Tasks;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Tile
{
    public class SpiderwebTileController : AbstractTileController
    {
        /// <summary>
        /// 蜘蛛の巣によってボトルが止まる秒数
        /// </summary>
        private const int _BIND_TIME = 2;

        protected override void Awake()
        {
            base.Awake();
            bottleHandler = new SpiderwebTileBottleHandler();
        }

        public override void Initialize(int tileNum)
        {
            base.Initialize(tileNum);

            #if UNITY_EDITOR
            name = TileName.SPIDERWEB_TILE;
            #endif
        }

        private sealed class SpiderwebTileBottleHandler : DefaultBottleHandler
        {
            public override void OnBottleEnter(GameObject bottle, Vector2Int? direction)
            {
                if (bottle.GetComponent<DynamicBottleController>() == null) return;

                StopBottle(bottle);
            }

            private static async void StopBottle(GameObject bottle)
            {
                Debug.Log("蜘蛛の巣タイルで拘束");
                bottle.GetComponent<DynamicBottleController>().IsMovable = false;

                // 引数は milliseconds
                await Task.Delay(_BIND_TIME * 1000);

                bottle.GetComponent<DynamicBottleController>().IsMovable = true;
                Debug.Log("蜘蛛の巣タイルから解放");
            }
        }
    }
}
