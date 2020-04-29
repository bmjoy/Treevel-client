using System;
using System.Collections;
using Project.Scripts.GamePlayScene.Bullet.Controllers;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Project.Scripts.GamePlayScene.Bullet.Generators
{
    public class NormalCartridgeGenerator : BulletGenerator
    {
        /// <summary>
        /// NormalCartridgeのPrefab
        /// </summary>
        [SerializeField] protected AssetReferenceGameObject _cartridgePrefab;

        /// <summary>
        /// NormalCartridgeWarningのPrefab
        /// </summary>
        [SerializeField] protected AssetReferenceGameObject _cartridgeWarningPrefab;

        /// <summary>
        /// 銃弾の移動方向
        /// </summary>
        protected ECartridgeDirection cartridgeDirection = ECartridgeDirection.Random;

        /// <summary>
        /// 移動する行(または列)の番号
        /// </summary>
        protected int line = (int) ERow.Random;

        /// <summary>
        /// 移動方向の重み
        /// </summary>
        /// <returns></returns>
        private int[] _randomCartridgeDirection =
            BulletLibrary.GetInitialArray(Enum.GetNames(typeof(ECartridgeDirection)).Length - 1);

        /// <summary>
        /// 移動する行の重み
        /// </summary>
        private int[] _randomRow = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(ERow)).Length - 1);

        /// <summary>
        /// 移動する列の重み
        /// </summary>
        private int[] _randomColumn = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(EColumn)).Length - 1);

        /// <inheritdoc />
        public override void Initialize(GameDatas.BulletData data)
        {
            this.ratio = data.ratio;
            this.cartridgeDirection = data.direction;
            this.line = data.line;
            if (data.randomCartridgeDirection.Count > 0) _randomCartridgeDirection = data.randomCartridgeDirection.ToArray();
            if (data.randomRow.Count > 0) _randomRow = data.randomRow.ToArray();
            if (data.randomColumn.Count > 0) _randomColumn = data.randomColumn.ToArray();
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
            AsyncOperationHandle<GameObject> warningOp;
            yield return warningOp = _cartridgeWarningPrefab.InstantiateAsync();
            var warning = warningOp.Result;

            warning.GetComponent<Renderer>().sortingOrder = bulletId;
            var warningScript = warning.GetComponent<CartridgeWarningController>();
            var bulletMotionVector =
                warningScript.Initialize(ECartridgeType.Normal, nextCartridgeDirection, nextCartridgeLine);

            // 警告の表示時間だけ待つ
            for (int index = 0; index < BulletWarningParameter.WARNING_DISPLAYED_FRAMES; index++) yield return new WaitForFixedUpdate();
            // 警告を削除する
            Destroy(warning);

            if (gamePlayDirector.State != GamePlayDirector.EGameState.Playing) yield break;

            // ゲームが続いているなら銃弾を作成する
            AsyncOperationHandle<GameObject> cartridgeOp;
            yield return cartridgeOp = _cartridgePrefab.InstantiateAsync();
            var cartridge = cartridgeOp.Result;

            cartridge.GetComponent<NormalCartridgeController>()
            .Initialize(nextCartridgeDirection, nextCartridgeLine, bulletMotionVector);

            // 同レイヤーのオブジェクトの描画順序の制御
            cartridge.GetComponent<Renderer>().sortingOrder = bulletId;
        }

        /// <summary>
        /// 移動方向を重みに基づき決定する
        /// </summary>
        /// <returns></returns>
        protected ECartridgeDirection GetCartridgeDirection()
        {
            var index = BulletLibrary.SamplingArrayIndex(_randomCartridgeDirection) + 1;
            return (ECartridgeDirection) Enum.ToObject(typeof(ECartridgeDirection), index);
        }

        /// <summary>
        /// 移動する行を重みに基づき決定する
        /// </summary>
        /// <returns></returns>
        protected int GetRow()
        {
            var index = BulletLibrary.SamplingArrayIndex(_randomRow) + 1;
            return (int) Enum.ToObject(typeof(ERow), index);
        }

        /// <summary>
        /// 移動する列を重みに基づき決定する
        /// </summary>
        /// <returns></returns>
        protected int GetColumn()
        {
            var index = BulletLibrary.SamplingArrayIndex(_randomColumn) + 1;
            return (int) Enum.ToObject(typeof(EColumn), index);
        }
    }
}
