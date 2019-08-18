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
        [SerializeField] private GameObject _normalHolePrefab;
        [SerializeField] private GameObject _normalHoleWarningPrefab;

        // 出現する行
        private int _row;

        // 出現する列
        private int _column;

        // Holeが出現する行をランダムに決めるときの各行の重み
        private int[] _randomRow = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(ERow)).Length - 1);

        // Holeが出現する列をランダムに決めるときの各列の重み
        private int[] _randomColumn = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(EColumn)).Length - 1);

        public void Initialize(int ratio, ERow row, EColumn column)
        {
            this.ratio = ratio;
            this._row = (int) row;
            this._column = (int) column;
        }

        public void Initialize(int ratio, ERow row, EColumn column, int[] randomRow, int[] randomColumn)
        {
            this.ratio = ratio;
            this._row = (int) row;
            this._column = (int) column;
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
            yield return new WaitForSeconds(BulletWarningController.WARNING_DISPLAYED_TIME);
            // 警告を削除する
            Destroy(warning);

            // ゲームが続いているなら銃弾を作成する
            if (gamePlayDirector.state == GamePlayDirector.EGameState.Playing) {
                var hole = Instantiate(_normalHolePrefab);
                var holeScript = hole.GetComponent<NormalHoleController>();
                holeScript.Initialize(nextHoleRow, nextHoleColumn, warning.transform.position);
                // 同レイヤーのオブジェクトの描画順序の制御
                hole.GetComponent<Renderer>().sortingOrder = bulletId;
                StartCoroutine(holeScript.Delete());
            }
        }

        /* Holeの出現する行を重みに基づき決定する */
        private int GetRow()
        {
            var index = BulletLibrary.SamplingArrayIndex(_randomRow) + 1;
            return (int) Enum.ToObject(typeof(ERow), index);
        }

        /* Holeの出現する列を重みに基づき決定する */
        private int GetColumn()
        {
            var index = BulletLibrary.SamplingArrayIndex(_randomColumn) + 1;
            return (int) Enum.ToObject(typeof(EColumn), index);
        }
    }
}
