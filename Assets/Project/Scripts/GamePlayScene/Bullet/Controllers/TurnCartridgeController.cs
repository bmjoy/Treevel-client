using System;
using System.Collections;
using Project.Scripts.GamePlayScene.Bullet.Generators;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library.Extension;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet.Controllers
{
    public class TurnCartridgeController : NormalCartridgeController
    {
        /// <summary>
        /// 曲がる方向の配列
        /// </summary>
        private int[] _turnDirection;

        /// <summary>
        /// 曲がる行(または列)の配列
        /// </summary>
        /// FIXME: 使われてない
        private int[] _turnLine;

        /// <summary>
        /// NormalWarningのPrefab
        /// </summary>
        [SerializeField] private GameObject _normalCartridgeWarningPrefab;

        /// <summary>
        /// 警告を表示するフレームの配列
        /// </summary>
        private int[] _warningTiming;

        /// <summary>
        /// 次に表示する警告のインデックス
        /// </summary>
        private int _warningIndex = 0;

        /// <summary>
        /// 曲がる方向を示す警告の座標の配列
        /// </summary>
        private Vector2[] _turnPoint;

        /// <summary>
        /// 1フレームあたりの回転角度
        /// </summary>
        private float[] _turnAngle;

        /// <summary>
        /// 回転に要するフレーム数
        /// </summary>
        private const int _COUNT = 50;

        /// <summary>
        /// 警告表示後から銃弾が警告座標に到達するフレーム数
        /// </summary>
        private const int _RUNNING_FRAMES = 30;

        /// <summary>
        /// 回転中の銃弾の速さ (円周(= 回転半径 * 2 * pi)の4分の1をフレーム数で割る)
        /// </summary>
        private const float _ROTATING_SPEED = ((PanelSize.WIDTH - CartridgeSize.WIDTH) / 2f) * 2f * (float) Math.PI / 4f / _COUNT;

        /// <summary>
        /// 銃弾が回転しているかどうかを表す状態
        /// </summary>
        private bool _rotating = false;

        /// <summary>
        /// 銃弾が待機しているかどうかを表す状態
        /// </summary>
        private bool _waiting = true;

        /// <summary>
        /// 銃弾が動き始めるまでの残りのフレーム数
        /// </summary>
        private int _waitingTime = TurnCartridgeGenerator.turnCartridgeWaitingFrames;

        /// <summary>
        /// 警告を表示し、銃弾を回転させるcoroutineの配列
        /// </summary>
        private IEnumerator[] _rotateCoroutines;

        /// <summary>
        /// 曲がる方向に応じて表示分けする警告画像の名前
        /// </summary>
        /// FIXME: 使われてない
        private enum _ETurnWarning {
            TurnLeft,
            TurnRight,
            TurnUp,
            TurnBottom
        }

        protected override void FixedUpdate()
        {
            if (gamePlayDirector.state != GamePlayDirector.EGameState.Playing) return;

            // 警告を表示するタイミングかどうかを毎フレーム監視する
            _warningTiming[_warningIndex]--;

            if (_warningTiming[_warningIndex] == 0) {
                // 警告を表示、その後、銃弾の回転
                StartCoroutine(_rotateCoroutines[_warningIndex]);
                // 次の警告の表示タイミングを監視する
                if (_warningIndex != _turnDirection.Length - 1)
                    _warningIndex++;
            }

            if (_waiting) {
                // 待機中
                _waitingTime--;
                if (_waitingTime == 0)
                    _waiting = false;
            } else {
                if (_rotating) {
                    // 回転中
                    transform.Translate(motionVector * _ROTATING_SPEED, Space.World);
                } else {
                    // 直進中
                    transform.Translate(motionVector * speed, Space.World);
                }
            }
        }

        /// <summary>
        /// 銃弾の移動ベクトル、曲がる方向、曲がる行(または列)、初期座標を設定する
        /// 警告を表示するフレームを計算する
        /// </summary>
        /// <param name="direction"> 移動方向 </param>
        /// <param name="line"> 移動する行(または列) </param>
        /// <param name="motionVector"> 移動ベクトル </param>
        /// <param name="turnDirection">　曲がる方向 </param>
        /// <param name="turnLine"> 曲がる行(または列) </param>
        public void Initialize(ECartridgeDirection direction, int line, Vector2 motionVector,
            int[] turnDirection, int[] turnLine)
        {
            // 銃弾に必要な引数を受け取る
            Initialize(direction, line, motionVector);
            _turnDirection = turnDirection;
            _turnLine = turnLine;
            // 表示する全ての警告の座標および回転の角度を計算
            var cartridgePosition = transform.position;
            var cartridgeMotionVector = motionVector;
            _turnAngle = new float[turnDirection.Length];
            _turnPoint = new Vector2[turnDirection.Length];
            for (var index = 0; index < turnDirection.Length; index++) {
                // 警告の座標
                _turnPoint[index] = cartridgePosition * cartridgeMotionVector.Transposition().Abs() + new Vector2(
                        TileSize.WIDTH * (turnLine[index] - (StageSize.COLUMN / 2 + 1)),
                        TileSize.HEIGHT * (StageSize.ROW / 2 + 1 - turnLine[index])) * cartridgeMotionVector.Abs();
                // 1フレームあたりの回転の角度
                _turnAngle[index] = _turnDirection[index] % 2 == 1 ? 90 : -90;
                _turnAngle[index] = (cartridgeMotionVector.x + cartridgeMotionVector.y) * _turnAngle[index];
                _turnAngle[index] = _turnAngle[index] / (_COUNT - 1) / 180.0f * Mathf.PI;
                // 求めた警告の座標を銃弾の座標として次の警告の座標を求める
                cartridgePosition = _turnPoint[index];
                cartridgeMotionVector = cartridgeMotionVector.Rotate(_turnAngle[index] * (_COUNT - 1));
            }

            // 1つ目の警告を表示させるタイミングを求める
            // 警告座標に到達する時間(= 銃弾が進む距離 / 銃弾の速さ)のNフレーム前が表示タイミング
            _warningTiming = new int[turnDirection.Length];
            _warningTiming[0] = TurnCartridgeGenerator.turnCartridgeWaitingFrames + (int)Math.Round((Vector2.Distance(transform.position, _turnPoint[0]) - (PanelSize.WIDTH - CartridgeSize.WIDTH) / 2f) / speed - BulletWarningParameter.WARNING_DISPLAYED_FRAMES - _RUNNING_FRAMES, MidpointRounding.AwayFromZero);
            // 警告の表示および銃弾が回転する挙動を特定のタイミングで発火できるようにcoroutineにセットする
            _rotateCoroutines = new IEnumerator[turnDirection.Length];
            _rotateCoroutines[0] = DisplayTurnWarning(_turnPoint[0], _turnDirection[0], _turnAngle[0]);
            for (var index = 1; index < turnDirection.Length; index++) {
                // 1つ前の警告の表示タイミングから何フレーム後に表示させるかを求める
                // 警告座標に到達する時間(= 1つ前の表示タイミングのNフレーム後 + 銃弾が回転にかかるフレーム数 + (警告と警告との距離) / 銃弾の速さ)のNフレーム前
                _warningTiming[index] = (int)Math.Round(_COUNT + (Vector2.Distance(_turnPoint[index - 1], _turnPoint[index]) - (PanelSize.WIDTH - CartridgeSize.WIDTH)) / speed, MidpointRounding.AwayFromZero);
                _rotateCoroutines[index] = DisplayTurnWarning(_turnPoint[index], _turnDirection[index], _turnAngle[index]);
            }
        }

        /// <summary>
        /// 銃弾を回転させるcoroutine
        /// </summary>
        /// <param name="turnAngle"> 1フレームあたりの回転角 </param>
        /// <returns></returns>
        private IEnumerator RotateCartridge(float turnAngle)
        {
            _rotating = true;
            // 回転はじめのフレーム
            motionVector = motionVector.Rotate(turnAngle / 2f);
            transform.Rotate(new Vector3(0, 0, turnAngle / 2f / Mathf.PI * 180f), Space.World);
            yield return new WaitForFixedUpdate();
            // 回転中のフレーム
            for (var index = 0; index < _COUNT - 2; index++) {
                motionVector = motionVector.Rotate(turnAngle);
                transform.Rotate(new Vector3(0, 0, turnAngle / Mathf.PI * 180f), Space.World);
                yield return new WaitForFixedUpdate();
            }
            // 回転終わりのフレーム
            motionVector = motionVector.Rotate(turnAngle / 2f);
            transform.Rotate(new Vector3(0, 0, turnAngle / 2f / Mathf.PI * 180f), Space.World);
            yield return new WaitForFixedUpdate();
            _rotating = false;
            yield break;
        }

        /// <summary>
        /// 警告を表示するcoroutine
        /// </summary>
        /// <param name="warningPosition"> 警告の座標 </param>
        /// <param name="turnDirection"> 回転の方向 </param>
        /// <param name="turnAngle"> 1フレームあたりの回転角 </param>
        /// <returns></returns>
        private IEnumerator DisplayTurnWarning(Vector2 warningPosition, int turnDirection, float turnAngle)
        {
            var warning = Instantiate(_normalCartridgeWarningPrefab);
            // 同レイヤーのオブジェクトの描画順序の制御
            warning.GetComponent<Renderer>().sortingOrder = gameObject.GetComponent<Renderer>().sortingOrder;
            // warningの位置・大きさ等の設定
            var warningScript = warning.GetComponent<CartridgeWarningController>();
            warningScript.Initialize(warningPosition, Enum.GetName(typeof(_ETurnWarning), turnDirection - 1));
            // 警告の表示時間だけ待つ
            for (var index = 0; index < BulletWarningParameter.WARNING_DISPLAYED_FRAMES; index++) yield return new WaitForFixedUpdate();
            // 警告を削除する
            Destroy(warning);
            for (var index = 0; index < _RUNNING_FRAMES; index++) yield return new WaitForFixedUpdate();
            // 銃弾を回転させる
            StartCoroutine(RotateCartridge(turnAngle));
            yield break;
        }

        protected override void OnFail()
        {
            base.OnFail();
            StopAllCoroutines();
            motionVector = new Vector2(0, 0);
        }
    }
}
