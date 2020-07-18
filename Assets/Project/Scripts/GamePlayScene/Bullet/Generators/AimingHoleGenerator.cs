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
        /// 撃ち抜くAttackableBottleの番号配列
        /// </summary>
        [CanBeNull] private int[] _aimingBottles = null;

        /// <summary>
        /// 次に参照するaimingBottlesのindex
        /// </summary>
        private int _aimingHoleCount = 1;

        /// <summary>
        /// AimingHoleが撃ち抜くAttackableBottleの確率
        /// </summary>
        /// <returns></returns>
        private int[] _randomAttackableBottles = BulletLibrary.GetInitialArray(StageSize.NUMBER_BOTTLE_NUM); // TODO: ステージデータからボトルの数を計算

        /// <summary>
        /// 特定のAttackableBottleを撃ち抜くAimingHoleのGeneratorの初期化
        /// </summary>
        /// <param name="ratio"> Generatorの出現確率 </param>
        /// <param name="aimingBottles"> 撃ち抜くAttackableBottleの配列 </param>
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
            if (data.randomAttackableBottles.Count > 0) _randomAttackableBottles = data.randomAttackableBottles.ToArray();
        }

        public override IEnumerator CreateBullet(int bulletId)
        {
            // どのAttackableBottleを撃つか指定する
            int[] nextAimingBottle = _aimingBottles ?? new int[] {GetAttackableBottle()};

            // 警告の作成
            AsyncOperationHandle<GameObject> warningOp;
            yield return warningOp = _holeWarningPrefab.InstantiateAsync();
            var warning = warningOp.Result;
            warning.GetComponent<Renderer>().sortingOrder = bulletId;
            var warningScript = warning.GetComponent<AimingHoleWarningController>();
            warningScript.Initialize(nextAimingBottle, ref _aimingHoleCount);
            // 警告の表示時間だけ待つ
            for (int index = 0; index < GimmickWarningParameter.WARNING_DISPLAYED_FRAMES; index++) yield return new WaitForFixedUpdate();

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
        /// 撃ちぬくAttackableBottleを重みに基づき決定する
        /// </summary>
        /// <returns></returns>
        private int GetAttackableBottle()
        {
            var index = BulletLibrary.SamplingArrayIndex(_randomAttackableBottles);
            return CalcBottleIdByRandomArrayIndex(index);
        }

        /// <summary>
        /// 乱数配列のインデックスをボトルのIdに変換する
        /// </summary>
        /// <param name="index">_randomAttackableBottlesから取ったインデックス</param>
        /// <returns>ボトルのID</returns>
        private int CalcBottleIdByRandomArrayIndex(int index)
        {
            var bottles = BottleLibrary.OrderedAttackableBottles;
            var bottleAtIndex = bottles.ElementAt(index);
            return bottleAtIndex.Id;
        }
    }
}
