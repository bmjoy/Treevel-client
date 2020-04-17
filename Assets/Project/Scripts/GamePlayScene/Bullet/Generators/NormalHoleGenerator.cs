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
    public class NormalHoleGenerator : BulletGenerator
    {
        /// <summary>
        /// NormalHoleのPrefab
        /// </summary>
        [SerializeField] protected AssetReferenceGameObject _holePrefab;

        /// <summary>
        /// NormalHoleWarningのPrefab
        /// </summary>
        [SerializeField] protected AssetReferenceGameObject _holeWarningPrefab;

        /// <summary>
        ///  出現する行
        /// </summary>
        private int _row = (int) ERow.Random;

        /// <summary>
        /// 出現する列
        /// </summary>
        private int _column = (int) EColumn.Random;

        /// <summary>
        /// 出現する行の重み
        /// </summary>
        /// <returns></returns>
        private int[] _randomRow = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(ERow)).Length - 1);

        /// <summary>
        /// 出現する列の重み
        /// </summary>
        /// <returns></returns>
        private int[] _randomColumn = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(EColumn)).Length - 1);

        /// <inheritdoc />
        public override void Initialize(GameDatas.BulletData data)
        {
            this.ratio = data.ratio;
            _row = (int)data.row;
            _column = (int)data.column;
            if (data.randomRow.Count > 0) _randomRow = data.randomRow.ToArray();
            if (data.randomColumn.Count > 0) _randomColumn = data.randomColumn.ToArray();
        }

        public override IEnumerator CreateBullet(int bulletId)
        {
            // 出現する行および列を指定する
            var nextHoleRow = (_row == (int) ERow.Random) ? GetRow() : _row;
            var nextHoleColumn = (_column == (int) EColumn.Random) ? GetColumn() : _column;

            // 警告の作成
            AsyncOperationHandle<GameObject> warningOp;
            yield return warningOp = _holeWarningPrefab.InstantiateAsync();
            var warning = warningOp.Result;
            warning.GetComponent<Renderer>().sortingOrder = bulletId;
            var warningScript = warning.GetComponent<NormalHoleWarningController>();
            warningScript.Initialize(nextHoleRow, nextHoleColumn);
            // 警告の表示時間だけ待つ
            for (var index = 0; index < BulletWarningParameter.WARNING_DISPLAYED_FRAMES; index++) yield return new WaitForFixedUpdate();

            // 警告の位置を一時保存
            Vector2 warningPos = warning.transform.position;

            // 警告を削除する
            Destroy(warning);

            if (gamePlayDirector.state != GamePlayDirector.EGameState.Playing) yield break;

            // ゲームが続いているなら銃弾を作成する
            AsyncOperationHandle<GameObject> holeOp;
            yield return holeOp = _holePrefab.InstantiateAsync();
            var hole = holeOp.Result;
            var holeScript = hole.GetComponent<NormalHoleController>();
            holeScript.Initialize(nextHoleRow, nextHoleColumn, warningPos);
            // 同レイヤーのオブジェクトの描画順序の制御
            hole.GetComponent<Renderer>().sortingOrder = bulletId;
        }

        /// <summary>
        /// 出現する行を重みに基づき決定する
        /// </summary>
        /// <returns></returns>
        private int GetRow()
        {
            var index = BulletLibrary.SamplingArrayIndex(_randomRow) + 1;
            return (int) Enum.ToObject(typeof(ERow), index);
        }

        /// <summary>
        /// 出現する列を重みに基づき決定する
        /// </summary>
        /// <returns></returns>
        private int GetColumn()
        {
            var index = BulletLibrary.SamplingArrayIndex(_randomColumn) + 1;
            return (int) Enum.ToObject(typeof(EColumn), index);
        }
    }
}
