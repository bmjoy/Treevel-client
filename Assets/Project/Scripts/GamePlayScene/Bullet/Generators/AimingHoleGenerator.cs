using System.Linq;
using System.Collections;
using JetBrains.Annotations;
using Project.Scripts.GamePlayScene.Bullet.Controllers;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Project.Scripts.GamePlayScene.Bullet.Generators
{
    public class AimingHoleGenerator : NormalHoleGenerator
    {
        /// <summary>
        /// 撃ち抜くNumberPanelの番号配列
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
        private int[] _randomNumberPanels = BulletLibrary.GetInitialArray(StageSize.NUMBER_BOTTLE_NUM); // TODO: ステージデータからボトルの数を計算

        /// <summary>
        /// 特定のNumberPanelを撃ち抜くAimingHoleのGeneratorの初期化
        /// </summary>
        /// <param name="ratio"> Generatorの出現確率 </param>
        /// <param name="aimingPanels"> 撃ち抜くNumberPanelの配列 </param>
        public void Initialize(int ratio, int[] aimingPanels)
        {
            this.ratio = ratio;
            _aimingPanels = aimingPanels;
        }

        /// <inheritdoc />
        public override void Initialize(GameDatas.BulletData data)
        {
            this.ratio = data.ratio;
            if (data.aimingPanels.Count > 0) _aimingPanels = data.aimingPanels.ToArray();
            if (data.randomNumberPanels.Count > 0) _randomNumberPanels = data.randomNumberPanels.ToArray();
        }

        /// <summary>
        /// ランダムなNumberPanelを撃ち抜くAimingHoleのGeneratorの初期化
        /// </summary>
        /// <param name="ratio"> Generatorの出現確率 </param>
        /// <param name="randomNumberPanels"> 撃ちぬくNumberPanelの確率 </param>
        public void InitializeRandom(int ratio, int[] randomNumberPanels)
        {
            this.ratio = ratio;
            _randomNumberPanels = randomNumberPanels;
        }

        public override IEnumerator CreateBullet(int bulletId)
        {
            // どのNumberPanelを撃つか指定する
            int[] nextAimingPanel = _aimingPanels ?? new int[] {GetNumberPanel()};

            // 警告の作成
            AsyncOperationHandle<GameObject> warningOp;
            yield return warningOp = _holeWarningPrefab.InstantiateAsync();
            var warning = warningOp.Result;
            warning.GetComponent<Renderer>().sortingOrder = bulletId;
            var warningScript = warning.GetComponent<AimingHoleWarningController>();
            warningScript.Initialize(nextAimingPanel, ref _aimingHoleCount);
            // 警告の表示時間だけ待つ
            for (int index = 0; index < BulletWarningParameter.WARNING_DISPLAYED_FRAMES; index++) yield return new WaitForFixedUpdate();

            // 警告の位置を一時保存
            Vector2 warningPos = warning.transform.position;

            // 警告を削除する
            Destroy(warning);

            if (gamePlayDirector.State != GamePlayDirector.EGameState.Playing) yield break;

            // ゲームが続いているなら銃弾を作成する
            AsyncOperationHandle<GameObject> holeOp;
            yield return holeOp = _holePrefab.InstantiateAsync();
            var hole = holeOp.Result;
            var holeScript = hole.GetComponent<AimingHoleController>();
            holeScript.Initialize(warningPos);
            // 同レイヤーのオブジェクトの描画順序の制御
            hole.GetComponent<Renderer>().sortingOrder = bulletId;
        }

        /// <summary>
        /// 撃ちぬくNumberPanelを重みに基づき決定する
        /// </summary>
        /// <returns></returns>
        private int GetNumberPanel()
        {
            var index = BulletLibrary.SamplingArrayIndex(_randomNumberPanels);
            return CalcPanelIdByRandomArrayIndex(index);
        }

        /// <summary>
        /// 乱数配列のインデックスをボトルのIdに変換する
        /// </summary>
        /// <param name="index">_randomNumberPanelsから取ったインデックス</param>
        /// <returns>ボトルのID</returns>
        private int CalcPanelIdByRandomArrayIndex(int index)
        {
            var panels = BottleLibrary.OrderedNumberPanels;
            var panelAtIndex = panels.ElementAt(index);
            return panelAtIndex.Id;
        }
    }
}
