using SpriteGlow;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bottle
{
    public interface IBottleEnterTileHandler
    {
        /// <summary>
        /// ボトルがタイルに移動したときの挙動
        /// </summary>
        /// <param name="tile"> 移動先のタイル </param>
        void OnEnterTile(GameObject tile);
    }
}
