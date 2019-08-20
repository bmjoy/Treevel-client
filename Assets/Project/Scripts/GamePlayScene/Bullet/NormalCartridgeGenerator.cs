using UnityEngine;
using System;
using System.Collections;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;

namespace Project.Scripts.GamePlayScene.Bullet
{
    public class NormalCartridgeGenerator : BulletGenerator
    {
        /// <summary>
        /// NormalCartridgeのPrefab
        /// </summary>
        [SerializeField] private GameObject _normalCartridgePrefab;
        /// <summary>
        /// NormalCartridgeWarningのPrefab
        /// </summary>
        [SerializeField] private GameObject _normalCartridgeWarningPrefab;

        /// <summary>
        /// 銃弾の移動方向
        /// </summary>
        protected ECartridgeDirection cartridgeDirection;

        /// <summary>
        /// 移動する行(または列)の番号
        /// </summary>
        protected int line;

        /// <summary>
        /// 移動方向の重み
        /// </summary>
        /// <returns></returns>
        private int[] randomCartridgeDirection =
            BulletLibrary.GetInitialArray(Enum.GetNames(typeof(ECartridgeDirection)).Length - 1);

        /// <summary>
        /// 移動する行の重み
        /// </summary>
        protected int[] randomRow = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(ERow)).Length - 1);

        /// <summary>
        /// 移動する列の重み
        /// </summary>
        protected int[] randomColumn = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(EColumn)).Length - 1);

        /// <summary>
        /// 特定の行を移動するNormalCartridgeを生成するGeneratorの初期化
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="cartridgeDirection">移動方向 </param>
        /// <param name="row"> 移動する行 </param>
        public void Initialize(int ratio, ECartridgeDirection cartridgeDirection, ERow row)
        {
            this.ratio = ratio;
            this.cartridgeDirection = cartridgeDirection;
            line = (int) row;
        }

        /// <summary>
        /// 特定の列を移動するNormalCartridgeを生成するGeneratorの初期化
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="cartridgeDirection">移動方向 </param>
        /// <param name="column"> 移動する列 </param>
        public void Initialize(int ratio, ECartridgeDirection cartridgeDirection, EColumn column)
        {
            this.ratio = ratio;
            this.cartridgeDirection = cartridgeDirection;
            line = (int) column;
        }

        /// <summary>
        /// ランダムな行を移動するNormalCartridgeを生成するGeneratorの初期化
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="cartridgeDirection"> 銃弾の移動方向 </param>
        /// <param name="row"> 移動する行 </param>
        /// <param name="randomCartridgeDirection"> 移動方向の重み </param>
        /// <param name="randomRow"> 移動する行の重み </param>
        /// <param name="randomColumn"> 移動する列の重み </param>
        public void Initialize(int ratio, ECartridgeDirection cartridgeDirection, ERow row,
            int[] randomCartridgeDirection, int[] randomRow, int[] randomColumn)
        {
            Initialize(ratio, cartridgeDirection, row);
            this.randomCartridgeDirection = randomCartridgeDirection;
            this.randomRow = randomRow;
            this.randomColumn = randomColumn;
        }

        /// <summary>
        /// ランダムな列を移動するNormalCartridgeを生成するGeneratorの初期化
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="cartridgeDirection"> 銃弾の移動方向 </param>
        /// <param name="column"> 移動する列 </param>
        /// <param name="randomCartridgeDirection"> 移動方向の重み </param>
        /// <param name="randomRow"> null </param>
        /// <param name="randomColumn"> 移動する列の重み </param>
        public void Initialize(int ratio, ECartridgeDirection cartridgeDirection, EColumn column,
            int[] randomCartridgeDirection, int[] randomRow, int[] randomColumn)
        {
            Initialize(ratio, cartridgeDirection, column);
            this.randomCartridgeDirection = randomCartridgeDirection;
            this.randomRow = randomRow;
            this.randomColumn = randomColumn;
        }

        public override IEnumerator CreateBullet(int bulletId)
        {
            // 銃弾の移動方向を指定する
            var nextCartridgeDirection = (cartridgeDirection == ECartridgeDirection.Random)
                ? GetCartridgeDirection()
                : cartridgeDirection;

            // 銃弾の移動する行(または列)を指定する
            var nextCartridgeLine = line;
            if (nextCartridgeLine == (int) ERow.Random) {
                switch (nextCartridgeDirection) {
                    case ECartridgeDirection.ToLeft:
                    case ECartridgeDirection.ToRight:
                        nextCartridgeLine = GetRow();
                        break;
                    case ECartridgeDirection.ToUp:
                    case ECartridgeDirection.ToBottom:
                        nextCartridgeLine = GetColumn();
                        break;
                    case ECartridgeDirection.Random:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            // warningの作成
            var warning = Instantiate(_normalCartridgeWarningPrefab);
            warning.GetComponent<Renderer>().sortingOrder = bulletId;
            var warningScript = warning.GetComponent<CartridgeWarningController>();
            var bulletMotionVector =
                warningScript.Initialize(ECartridgeType.Normal, nextCartridgeDirection, nextCartridgeLine);

            // 警告の表示時間だけ待つ
            yield return new WaitForSeconds(BulletWarningController.WARNING_DISPLAYED_TIME);
            // 警告を削除する
            Destroy(warning);

            // ゲームが続いているなら銃弾を作成する
            if (gamePlayDirector.state == GamePlayDirector.EGameState.Playing) {
                var cartridge = Instantiate(_normalCartridgePrefab);
                cartridge.GetComponent<NormalCartridgeController>()
                .Initialize(nextCartridgeDirection, nextCartridgeLine, bulletMotionVector);

                // 同レイヤーのオブジェクトの描画順序の制御
                cartridge.GetComponent<Renderer>().sortingOrder = bulletId;
            }
        }

        /// <summary>
        /// 移動方向を重みに基づき決定する
        /// </summary>
        /// <returns></returns>
        protected ECartridgeDirection GetCartridgeDirection()
        {
            var index = BulletLibrary.SamplingArrayIndex(randomCartridgeDirection) + 1;
            return (ECartridgeDirection) Enum.ToObject(typeof(ECartridgeDirection), index);
        }

        /// <summary>
        /// 移動する行を重みに基づき決定する
        /// </summary>
        /// <returns></returns>
        protected int GetRow()
        {
            var index = BulletLibrary.SamplingArrayIndex(randomRow) + 1;
            return (int) Enum.ToObject(typeof(ERow), index);
        }

        /// <summary>
        /// 移動する列を重みに基づき決定する
        /// </summary>
        /// <returns></returns>
        protected int GetColumn()
        {
            var index = BulletLibrary.SamplingArrayIndex(randomColumn) + 1;
            return (int) Enum.ToObject(typeof(EColumn), index);
        }
    }
}
