using UnityEngine;
using System;
using System.Linq;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library.Extension;

namespace Project.Scripts.GamePlayScene.Bullet
{
    public class TurnCartridgeController : NormalCartridgeController
    {
        // 回転方向の配列
        private int[] _turnDirection;

        // 回転する行(or列)の配列
        private int[] _turnLine;

        // 回転に関する警告を表示する座標
        private Vector2 _turnPoint;

        // 回転方向に応じて表示わけする警告画像の名前
        private readonly string[] _warningList = {"turnLeft", "turnRight", "turnUp", "turnBottom"};

        // 警告
        [SerializeField] private GameObject _normalCartridgeWarningPrefab;
        private GameObject _warning;

        // １フレームあたりの回転角度
        private float _turnAngle;

        // 回転に要するフレーム数
        private const int COUNT = 50;

        // 回転の何フレーム目か
        private int _rotateCount = -1;

        // 回転中の銃弾の速さ (円周(回転半径 * 2 * pi)の4分の1をフレーム数で割る)
        private float rotatingSpeed =
            ((PanelSize.WIDTH - CartridgeSize.WIDTH) / 2f) * 2f * (float) Math.PI / 4f / COUNT;

        protected override void FixedUpdate()
        {
            // 回転する座標に近づいたら警告を表示する
            if (_rotateCount == -1 && Vector2.Distance(_turnPoint, transform.position) <=
                (PanelSize.WIDTH - CartridgeSize.WIDTH) / 2f + speed * 50) {
                _warning = Instantiate(_normalCartridgeWarningPrefab);
                // 同レイヤーのオブジェクトの描画順序の制御
                _warning.GetComponent<Renderer>().sortingOrder = gameObject.GetComponent<Renderer>().sortingOrder;
                // warningの位置・大きさ等の設定
                var warningScript = _warning.GetComponent<CartridgeWarningController>();
                warningScript.Initialize(_turnPoint, _warningList[_turnDirection[0] - 1]);
                _rotateCount++;
                transform.Translate(motionVector * speed, Space.World);
            }
            // 回転はじめのフレーム
            else if (_rotateCount == 0 && Vector2.Distance(_turnPoint, transform.position) <=
                (PanelSize.WIDTH - CartridgeSize.WIDTH) / 2f) {
                Destroy(_warning);
                _rotateCount++;
                motionVector = motionVector.Rotate(_turnAngle / 2f);
                transform.Rotate(new Vector3(0, 0, _turnAngle / 2f / Mathf.PI * 180f), Space.World);
                transform.Translate(motionVector * rotatingSpeed, Space.World);
            }
            // 回転中のフレーム
            else if (0 < _rotateCount && _rotateCount <= COUNT - 1) {
                _rotateCount++;
                motionVector = motionVector.Rotate(_turnAngle);
                transform.Rotate(new Vector3(0, 0, _turnAngle / Mathf.PI * 180f), Space.World);
                transform.Translate(motionVector * rotatingSpeed, Space.World);
            }
            // 回転おわりのフレーム
            else if (_rotateCount == COUNT) {
                transform.Rotate(new Vector3(0, 0, _turnAngle / 2f / Mathf.PI * 180f), Space.World);
                motionVector = motionVector.Rotate(_turnAngle / 2f);

                // 別のタイル上でまだ回転する場合
                if (_turnDirection.Length >= 2) {
                    // 配列の先頭要素を除く部分配列を取得する
                    _turnDirection = _turnDirection.Skip(1).Take(_turnDirection.Length - 1).ToArray();
                    _turnLine = _turnLine.Skip(1).Take(_turnLine.Length - 1).ToArray();
                    _turnPoint = transform.position * motionVector.Transposition().Abs() + new Vector2(
                            TileSize.WIDTH * (_turnLine[0] - 2),
                            WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
                            TileSize.HEIGHT * (_turnLine[0] - 1)) * motionVector.Abs();
                    _turnAngle = _turnDirection[0] % 2 == 1 ? 90 : -90;
                    _turnAngle = (motionVector.x + motionVector.y) * _turnAngle;
                    _turnAngle = _turnAngle / COUNT / 180.0f * Mathf.PI;
                    _rotateCount = -1;
                }
                // 回転しない場合
                else {
                    _rotateCount = -2;
                }

                transform.Translate(motionVector * speed, Space.World);
            }
            // 回転中以外
            else {
                transform.Translate(motionVector * speed, Space.World);
            }
        }

        public void Initialize(ECartridgeDirection direction, int line, Vector2 motionVector,
            int[] turnDirection, int[] turnLine)
        {
            // 銃弾に必要な引数を受け取る
            Initialize(direction, line, motionVector);
            this._turnDirection = turnDirection;
            this._turnLine = turnLine;
            // 銃弾が曲がるタイルの座標
            _turnPoint = transform.position * motionVector.Transposition().Abs() + new Vector2(
                    TileSize.WIDTH * (turnLine[0] - 2),
                    WindowSize.HEIGHT * 0.5f - (TileSize.MARGIN_TOP + TileSize.HEIGHT * 0.5f) -
                    TileSize.HEIGHT * (turnLine[0] - 1)) * motionVector.Abs();
            // 回転角度
            _turnAngle = turnDirection[0] % 2 == 1 ? 90 : -90;
            _turnAngle = (motionVector.x + motionVector.y) * _turnAngle;
            _turnAngle = _turnAngle / COUNT / 180.0f * Mathf.PI;
        }

        protected override void OnFail()
        {
            base.OnFail();
            rotatingSpeed = 0;
            _turnAngle = 0;
        }
    }
}
