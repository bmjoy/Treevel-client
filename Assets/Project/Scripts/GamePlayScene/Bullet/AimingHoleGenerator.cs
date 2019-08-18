using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;

namespace Project.Scripts.GamePlayScene.Bullet
{
    public class AimingHoleGenerator : NormalHoleGenerator
    {
        [SerializeField] private GameObject _aimingHolePrefab;
        [SerializeField] private GameObject _aimingHoleWarningPrefab;

        // 銃弾を出現させるNumberPanelの番号の入った配列
        [CanBeNull] private int[] _aimingPanel = null;

        // aimingPanel配列の何番目か
        private int _aimingHoleCount = 0;

        // aimingHoleが出現するNumberPanelの出現率の重み
        private int[] _randomNumberPanel = BulletLibrary.GetInitialArray(StageSize.NUMBER_PANEL_NUM);

        public void Initialize(int ratio, int[] aimingPanel)
        {
            this.ratio = ratio;
            this._aimingPanel = aimingPanel;
        }

        public void Initialize(int ratio, int[] aimingPanel, int[] randomNumberPanel)
        {
            this.ratio = ratio;
            this._aimingPanel = aimingPanel;
            this._randomNumberPanel = randomNumberPanel;
        }

        public override IEnumerator CreateBullet(int bulletId)
        {
            // どのNumberPanelを撃つか指定する
            int[] nextAimingPanel = _aimingPanel ?? new int[] {GetNumberPanel()};

            // 警告の作成
            var warning = Instantiate(_aimingHoleWarningPrefab);
            warning.GetComponent<Renderer>().sortingOrder = bulletId;
            var warningScript = warning.GetComponent<AimingHoleWarningController>();
            warningScript.Initialize(nextAimingPanel, ref _aimingHoleCount);
            // 警告の表示時間だけ待つ
            yield return new WaitForSeconds(BulletWarningController.WARNING_DISPLAYED_TIME);
            // 警告を削除する
            Destroy(warning);

            // ゲームが続いているなら銃弾を作成する
            if (gamePlayDirector.state == GamePlayDirector.EGameState.Playing) {
                var hole = Instantiate(_aimingHolePrefab);
                var holeScript = hole.GetComponent<AimingHoleController>();
                holeScript.Initialize(warning.transform.position);
                // 同レイヤーのオブジェクトの描画順序の制御
                hole.GetComponent<Renderer>().sortingOrder = bulletId;
                StartCoroutine(holeScript.Delete());
            }
        }

        /* 撃つNumberPanelの番号を重みに基づき決定する */
        private int GetNumberPanel()
        {
            var index = BulletLibrary.SamplingArrayIndex(_randomNumberPanel) + 1;
            return index;
        }
    }
}
