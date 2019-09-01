using UnityEngine;
using System;
using System.Collections;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;

namespace Project.Scripts.GamePlayScene.Bullet
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
        /// 特定の行、特定の列に出現するNormalHoleを生成するGeneratorの初期化
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="row"> 出現する行 </param>
        /// <param name="column"> 出現する列 </param>
        public void Initialize(int ratio, ERow row, EColumn column)
        {
            this.ratio = ratio;
            this._row = (int) row;
            this._column = (int) column;
        }

        /// <summary>
        /// ランダムな行、ランダムな列に出現するNormalHoleを生成するGeneratorの初期化
        /// </summary>
        /// <param name="ratio">　Generatorの出現確率 </param>
        /// <param name="randomRow"> 出現する行の重み </param>
        /// <param name="randomColumn"> 出現する列の重み </param>
        public void Initialize(int ratio, int[] randomRow, int[] randomColumn)
        {
            this.ratio = ratio;
            this._randomRow = randomRow;
            this._randomColumn = randomColumn;
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
            for(int index=0; index<BulletWarningParameter.WARNING_DISPLAYED_FRAMES; index++) yield return new WaitForFixedUpdate();
            // 警告を削除する
            Destroy(warning);

            // ゲームが続いているなら銃弾を作成する
            if (gamePlayDirector.state == GamePlayDirector.EGameState.Playing) {
                var hole = Instantiate(_normalHolePrefab);
                var holeScript = hole.GetComponent<NormalHoleController>();
                holeScript.Initialize(nextHoleRow, nextHoleColumn, warning.transform.position);
                // 同レイヤーのオブジェクトの描画順序の制御
                hole.GetComponent<Renderer>().sortingOrder = bulletId;
                StartCoroutine(holeScript.CollisionCheck());
            }
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
