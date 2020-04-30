using SpriteGlow;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bottle
{
    public interface IEnterTileHandler
    {
        void OnEnterTile(GameObject tile);
    }

    internal class NormalEnterTileHandler : IEnterTileHandler
    {
        private AbstractBottleController _bottle;

        internal NormalEnterTileHandler(AbstractBottleController bottle)
        {
            _bottle = bottle;
        }

        void IEnterTileHandler.OnEnterTile(GameObject tile)
        {
            if (!(_bottle is IBottleSuccessHandler handler))
                return;

            if (handler.IsSuccess()) {
                // 最終タイルにいるかどうかで，光らせるかを決める
                _bottle.GetComponent<SpriteGlowEffect>().enabled = true;

                handler.DoWhenSuccess();
            } else {
                _bottle.GetComponent<SpriteGlowEffect>().enabled = false;
            }
        }
    }
}
