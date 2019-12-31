using UnityEngine;
using System;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.BulletWarning
{
    public class CartridgeWarningController : BulletWarningController
    {
        protected override void Awake()
        {
            base.Awake();
            transform.localScale =
                new Vector2(CartridgeWarningSize.WIDTH / originalWidth, CartridgeWarningSize.HEIGHT / originalHeight);
        }

        /// <summary>
        /// 座標を設定する
        /// 銃弾の移動方向が副次的に計算される
        /// </summary>
        /// <param name="cartridgeType"> 銃弾の種類 </param>
        /// <param name="direction"> 移動方向 </param>
        /// <param name="line"> 移動する行(または列) </param>
        /// <returns> 銃弾の移動方向 </returns>
        public Vector2 Initialize(ECartridgeType cartridgeType, ECartridgeDirection direction, int line)
        {
            // 表示する警告画像を設定する
            switch (cartridgeType) {
                case ECartridgeType.Normal:
                    break;
                case ECartridgeType.Turn:
                    var sprite = Resources.Load<Sprite>("Textures/BulletWarning/turnWarning");
                    gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
                    break;
                default:
                    throw new NotImplementedException();
            }

            Vector2 bulletMotionVector;
            Vector2 warningPosition;
            // 銃弾の進行方向から警告の座標を求める
            switch (direction) {
                case ECartridgeDirection.ToLeft:
                    warningPosition = new Vector2(WindowSize.WIDTH / 2,
                        TileSize.HEIGHT * (StageSize.ROW / 2 + 1 - line));
                    bulletMotionVector = Vector2.left;
                    break;
                case ECartridgeDirection.ToRight:
                    warningPosition = new Vector2(-WindowSize.WIDTH / 2,
                        TileSize.HEIGHT * (StageSize.ROW / 2 + 1 - line));
                    bulletMotionVector = Vector2.right;
                    break;
                case ECartridgeDirection.ToUp:
                    warningPosition = new Vector2(TileSize.WIDTH * (line - (StageSize.COLUMN / 2 + 1)),
                        -WindowSize.HEIGHT / 2);
                    bulletMotionVector = Vector2.up;
                    break;
                case ECartridgeDirection.ToBottom:
                    warningPosition = new Vector2(TileSize.WIDTH * (line - (StageSize.COLUMN / 2 + 1)),
                        WindowSize.HEIGHT / 2);
                    bulletMotionVector = Vector2.down;
                    break;
                default:
                    throw new NotImplementedException();
            }

            warningPosition = warningPosition + Vector2.Scale(bulletMotionVector,
                    new Vector2(CartridgeWarningSize.POSITION_X, CartridgeWarningSize.POSITION_Y)) / 2;
            transform.position = warningPosition;
            return bulletMotionVector;
        }

        /// <summary>
        /// 座標と警告画像の初期化
        /// </summary>
        /// <param name="position"> 座標 </param>
        /// <param name="imageName"> 警告画像の名前 </param>
        public void Initialize(Vector2 position, string imageName)
        {
            transform.position = position;
            var sprite = Resources.Load<Sprite>("Textures/BulletWarning/" + imageName);
            gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }
}
