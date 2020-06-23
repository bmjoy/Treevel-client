using SpriteGlow;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bottle
{
    public interface IBottleEnterTileHandler
    {
        void OnEnterTile(GameObject tile);
    }

    internal class NormalBottleEnterTileHandler : IBottleEnterTileHandler
    {
        private readonly AbstractBottleController _bottle;
        private readonly IBottleSuccessHandler _successHandler;

        internal NormalBottleEnterTileHandler(AbstractBottleController bottle, IBottleSuccessHandler successHandler)
        {
            if (successHandler == null)
                throw new System.NullReferenceException("SuccessHandler can not be null");

            _bottle = bottle;
            _successHandler = successHandler;
        }

        public void OnEnterTile(GameObject tile)
        {
            if (_successHandler.IsSuccess()) {
                // 最終タイルにいるかどうかで，光らせるかを決める
                _bottle.GetComponent<SpriteGlowEffect>().enabled = true;

                _successHandler.DoWhenSuccess();
            } else {
                _bottle.GetComponent<SpriteGlowEffect>().enabled = false;
            }
        }
    }
}
