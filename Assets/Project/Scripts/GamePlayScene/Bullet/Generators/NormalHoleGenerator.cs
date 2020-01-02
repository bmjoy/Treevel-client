using System;
using System.Collections;
using Project.Scripts.GamePlayScene.Bullet.Controllers;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet.Generators
{
    public class NormalHoleGenerator : BulletGenerator
    {
        /// <summary>
        /// NormalHoleのPrefab
        /// </summary>
        [SerializeField] private GameObject _normalHolePrefab;

        /// <summary>
        /// NormalHoleWarningのPrefab
        /// </summary>
        [SerializeField] private GameObject _normalHoleWarningPrefab;

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

        /// <summary>
        /// RandomNormalHole、NormalHole共通初期化メソッド
        ///
        /// 使用例：
        /// <code>
        /// // NormalHoleの場合
        /// Initialize(ratio, row, column /*random 以外*/);
        /// // RandomNormalHoleの場合
        /// Initialize(ratio, ERow.Random, EColumn.Random, randomRow, randomColumn);
        /// </code>
        /// </summary>
        /// <param name="ratio">Generatorの出現割合</param>
        /// <param name="row">出現する行</param>
        /// <param name="column">出現する列</param>
        /// <param name="randomRow">出現する行の重み</param>
        /// <param name="randomColumn">出現する列の重み</param>
        public void Initialize(int ratio,
            ERow row,
            EColumn column,
            int[] randomRow = null,
            int[] randomColumn = null)
        {
            this.ratio = ratio;
            _row = (int) row;
            _column = (int) column;
            _randomRow = randomRow;
            _randomColumn = randomColumn;
        }

        public override IEnumerator CreateBullet(int bulletId)
        {
            // 出現する行および列を指定する
            var nextHoleRow = (_row == (int) ERow.Random) ? GetRow() : _row;
            var nextHoleColumn = (_column == (int) EColumn.Random) ? GetColumn() : _column;

            // 警告の作成
            var warning = Instantiate(_normalHoleWarningPrefab);
            warning.GetComponent<Renderer>().sortingOrder = bulletId;
            var warningScript = warning.GetComponent<NormalHoleWarningController>();
            warningScript.Initialize(nextHoleRow, nextHoleColumn);
            // 警告の表示時間だけ待つ
            for (var index = 0; index < BulletWarningParameter.WARNING_DISPLAYED_FRAMES; index++) yield return new WaitForFixedUpdate();
            // 警告を削除する
            Destroy(warning);

            if (gamePlayDirector.state != GamePlayDirector.EGameState.Playing) yield break;

            // ゲームが続いているなら銃弾を作成する
            var hole = Instantiate(_normalHolePrefab);
            var holeScript = hole.GetComponent<NormalHoleController>();
            holeScript.Initialize(nextHoleRow, nextHoleColumn, warning.transform.position);
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
