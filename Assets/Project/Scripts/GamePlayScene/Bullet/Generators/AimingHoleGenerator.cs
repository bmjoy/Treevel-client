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
        /// 撃ち抜くNumberBottleの番号配列
        /// </summary>
        [CanBeNull] private int[] _aimingBottles = null;

        /// <summary>
        /// 次に参照するaimingBottlesのindex
        /// </summary>
        private int _aimingHoleCount = 1;

        /// <summary>
        /// AimingHoleが撃ち抜くNumberBottleの確率
        /// </summary>
        /// <returns></returns>
        private int[] _randomNumberBottles = BulletLibrary.GetInitialArray(StageSize.NUMBER_BOTTLE_NUM); // TODO: ステージデータからボトルの数を計算

        /// <summary>
        /// 特定のNumberBottleを撃ち抜くAimingHoleのGeneratorの初期化
        /// </summary>
        /// <param name="ratio"> Generatorの出現確率 </param>
        /// <param name="aimingBottles"> 撃ち抜くNumberBottleの配列 </param>
        public void Initialize(int ratio, int[] aimingBottles)
        {
            this.ratio = ratio;
            _aimingBottles = aimingBottles;
        }

        /// <inheritdoc />
        public override void Initialize(GameDatas.BulletData data)
        {
            this.ratio = data.ratio;
            if (data.aimingBottles.Count > 0) _aimingBottles = data.aimingBottles.ToArray();
            if (data.randomNumberBottles.Count > 0) _randomNumberBottles = data.randomNumberBottles.ToArray();
        }

        /// <summary>
        /// ランダムなNumberBottleを撃ち抜くAimingHoleのGeneratorの初期化
        /// </summary>
        /// <param name="ratio"> Generatorの出現確率 </param>
        /// <param name="randomNumberBottles"> 撃ちぬくNumberBottleの確率 </param>
        public void InitializeRandom(int ratio, int[] randomNumberBottles)
        {
            this.ratio = ratio;
            _randomNumberBottles = randomNumberBottles;
        }

        public override IEnumerator CreateBullet(int bulletId)
        {
            // どのNumberBottleを撃つか指定する
            int[] nextAimingBottle = _aimingBottles ?? new int[] {GetNumberBottle()};

            // 警告の作成
            AsyncOperationHandle<GameObject> warningOp;
            yield return warningOp = _holeWarningPrefab.InstantiateAsync();
            var warning = warningOp.Result;
            warning.GetComponent<Renderer>().sortingOrder = bulletId;
            var warningScript = warning.GetComponent<AimingHoleWarningController>();
            warningScript.Initialize(nextAimingBottle, ref _aimingHoleCount);
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
        /// 撃ちぬくNumberBottleを重みに基づき決定する
        /// </summary>
        /// <returns></returns>
        private int GetNumberBottle()
        {
            var index = BulletLibrary.SamplingArrayIndex(_randomNumberBottles);
            return CalcBottleIdByRandomArrayIndex(index);
        }

        /// <summary>
        /// 乱数配列のインデックスをボトルのIdに変換する
        /// </summary>
        /// <param name="index">_randomNumberBottlesから取ったインデックス</param>
        /// <returns>ボトルのID</returns>
        private int CalcBottleIdByRandomArrayIndex(int index)
        {
            var bottles = BottleLibrary.OrderedNumberBottles;
            var bottleAtIndex = bottles.ElementAt(index);
            return bottleAtIndex.Id;
        }
    }
}
