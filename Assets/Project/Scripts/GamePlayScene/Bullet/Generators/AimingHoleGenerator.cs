using System.Collections;
using JetBrains.Annotations;
using Project.Scripts.GamePlayScene.Bullet.Controllers;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet.Generators
{
    public class AimingHoleGenerator : NormalHoleGenerator
    {
        /// <summary>
        /// AimingHoleのPrefab
        /// </summary>
        [SerializeField] private GameObject _aimingHolePrefab;

        /// <summary>
        /// AimingHoleWarningのPrefab
        /// </summary>
        [SerializeField] private GameObject _aimingHoleWarningPrefab;

        /// <summary>
        /// 撃ち抜くNumberanelの番号配列
        /// </summary>
        [CanBeNull] private int[] _aimingPanels = null;

        /// <summary>
        /// 次に参照するaimingPanelsのindex
        /// </summary>
        private int _aimingHoleCount = 1;

        /// <summary>
        /// AimingHoleが撃ち抜くNumberPanelの確率
        /// </summary>
        /// <returns></returns>
        private int[] _randomNumberPanels = BulletLibrary.GetInitialArray(StageSize.NUMBER_PANEL_NUM);

        /// <summary>
        /// 特定のNumberPanelを撃ち抜くAimingHoleのGeneratorの初期化
        /// </summary>
        /// <param name="ratio"> Generatorの出現確率 </param>
        /// <param name="aimingPanels"> 撃ち抜くNumberPanelの配列 </param>
        public void Initialize(int ratio, int[] aimingPanels)
        {
            this.ratio = ratio;
            this._aimingPanels = aimingPanels;
        }

        /// <summary>
        /// ランダムなNumberPanelを撃ち抜くAimingHoleのGeneratorの初期化
        /// </summary>
        /// <param name="ratio"> Generatorの出現確率 </param>
        /// <param name="randomNumberPanel"> 撃ちぬくNumberPanelの確率 </param>
        public void InitializeRandom(int ratio, int[] randomNumberPanels)
        {
            this.ratio = ratio;
            this._randomNumberPanels = randomNumberPanels;
        }

        public override IEnumerator CreateBullet(int bulletId)
        {
            // どのNumberPanelを撃つか指定する
            int[] nextAimingPanel = _aimingPanels ?? new int[] {GetNumberPanel()};

            // 警告の作成
            var warning = Instantiate(_aimingHoleWarningPrefab);
            warning.GetComponent<Renderer>().sortingOrder = bulletId;
            var warningScript = warning.GetComponent<AimingHoleWarningController>();
            warningScript.Initialize(nextAimingPanel, ref _aimingHoleCount);
            // 警告の表示時間だけ待つ
            for (int index = 0; index < BulletWarningParameter.WARNING_DISPLAYED_FRAMES; index++) yield return new WaitForFixedUpdate();
            // 警告を削除する
            Destroy(warning);

            // ゲームが続いているなら銃弾を作成する
            if (gamePlayDirector.state == GamePlayDirector.EGameState.Playing) {
                var hole = Instantiate(_aimingHolePrefab);
                var holeScript = hole.GetComponent<AimingHoleController>();
                holeScript.Initialize(warning.transform.position);
                // 同レイヤーのオブジェクトの描画順序の制御
                hole.GetComponent<Renderer>().sortingOrder = bulletId;
            }
        }

        /// <summary>
        /// 撃ちぬくNumberPanelを重みに基づき決定する
        /// </summary>
        /// <returns></returns>
        private int GetNumberPanel()
        {
            var index = BulletLibrary.SamplingArrayIndex(_randomNumberPanels) + 1;
            return index;
        }
    }
}
