using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Treevel.Common.Components;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Bottle;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Treevel.Modules.GamePlayScene.Gimmick
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class TornadoController : AbstractGimmickController
    {
        /// <summary>
        /// 竜巻の移動速度(ワールド座標単位/秒)
        /// </summary>
        [SerializeField] private float _speed = 3.0f;

        /// <summary>
        /// 警告のプレハブ
        /// </summary>
        [SerializeField] protected AssetReferenceGameObject _warningPrefab;

        /// <summary>
        /// 竜巻の移動方向
        /// </summary>
        private EGimmickDirection[] _targetDirections;

        /// <summary>
        /// 攻撃する行／列
        /// </summary>
        private int[] _targetLines;

        /// <summary>
        /// 曲がる方向の重み
        /// </summary>
        private int[] _randomDirections = GimmickLibrary.GetInitialArray(Enum.GetNames(typeof(EGimmickDirection)).Length - 1);

        /// <summary>
        /// 曲がる行の重み
        /// </summary>
        private int[] _randomRow = GimmickLibrary.GetInitialArray(Enum.GetNames(typeof(ERow)).Length - 1);

        /// <summary>
        /// 曲がる列の重み
        /// </summary>
        private int[] _randomColumn = GimmickLibrary.GetInitialArray(Enum.GetNames(typeof(EColumn)).Length - 1);

        /// <summary>
        /// 警告表示座標のリスト
        /// </summary>
        private readonly List<Vector2> _warningPosList = new List<Vector2>();

        /// <summary>
        /// 今の目標インデックス
        /// </summary>
        private int _currentTargetIndex = 0;

        /// <summary>
        /// 警告消した後から曲がるまでの所要時間
        /// </summary>
        private float _moveTimeAfterWarning = 0.5f;

        private Rigidbody2D _rigidBody;

        /// <summary>
        /// 警告オブジェクトインスタンス（ゲーム終了時片付けするためにメンバー変数として持つ）
        /// </summary>
        private GameObject _warningObj;

        /// <summary>
        /// 警告のオフセット（ウィンドウの幅に対する比率）
        /// </summary>
        private const float _WARNING_OFFSET_RATIO = 0.16f;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// ギミックの初期化
        /// </summary>
        /// <param name="gimmickData"> ギミックデータ </param>
        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);

            _targetDirections = gimmickData.targetDirections.ToArray();
            _targetLines = gimmickData.targetLines.ToArray();
            if (gimmickData.randomDirection.Count > 0) _randomDirections = gimmickData.randomDirection.ToArray();
            if (gimmickData.randomColumn.Count > 0) _randomColumn = gimmickData.randomColumn.ToArray();
            if (gimmickData.randomRow.Count > 0) _randomRow = gimmickData.randomRow.ToArray();

            if (_targetDirections.Length <= 0 || _targetLines.Length <= 0 || _targetDirections.Length != _targetLines.Length) {
                throw new InvalidOperationException($"size of targetDirections = {_targetDirections.Length}, size of targetLines = {_targetLines.Length}");
            }

            InitializeDirectionsAndLines();

            SetInitialPosition(_targetDirections[0], _targetLines[0]);
            // 初期位置についたら表示する
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<Collider2D>().enabled = true;
        }

        /// <inheritdoc></inheritdoc>
        public override IEnumerator Trigger()
        {
            var currentDirection = _targetDirections[_currentTargetIndex];
            yield return ShowWarning(_warningPosList[0], null, _warningDisplayTime);

            // 発射
            SetDirection(currentDirection);

            Coroutine displayWarningCoroutine = null;
            while (++_currentTargetIndex < _targetDirections.Length) {
                currentDirection = _targetDirections[_currentTargetIndex];

                // 警告位置
                var warningPos = _warningPosList[_currentTargetIndex];

                // 警告表示時ギミックがいる位置＝警告表示位置＋（警告消した後からタイル位置に着くまでの時間＋警告表示時間）x速度x(-移動方向のベクトル)
                var warningStartDisplayPos = warningPos - _rigidBody.velocity * (_warningDisplayTime + _moveTimeAfterWarning);

                var diffVec = warningStartDisplayPos - (Vector2)transform.position;
                // 警告表示位置までまだ時間ある
                if (Vector2.Dot(diffVec, _rigidBody.velocity) > 0) {
                    // 表示するまでの所要時間
                    var warningStartWaitTime = diffVec.magnitude / _speed;

                    while ((warningStartWaitTime -= Time.fixedDeltaTime) >= 0) yield return new WaitForFixedUpdate();
                }

                // 警告を表示する
                if (displayWarningCoroutine != null) { // 前回のコルチーンがまだ終わってないまま次実行すると警告の破壊が複数回発生してしまう
                    StopCoroutine(displayWarningCoroutine);
                }
                displayWarningCoroutine = StartCoroutine(ShowWarning(warningPos, currentDirection, _warningDisplayTime));

                // 目標位置についたら転向処理（竜巻だからそのままdirection変えればいいのか？）
                while (Vector2.Dot(_rigidBody.velocity, warningPos - (Vector2)transform.position) > 0) {
                    yield return new WaitForFixedUpdate();
                }

                SetDirection(currentDirection);
            }

            // 範囲外になったらオブジェクトを消す
            while (Math.Abs(transform.position.x) < Constants.WindowSize.WIDTH && Math.Abs(transform.position.y) < Constants.WindowSize.HEIGHT)
                yield return new WaitForFixedUpdate();

            Destroy(gameObject);
        }

        /// <summary>
        /// 方向と行列に対して、ランダムなものがあれば有効な数値で入れ替える
        /// </summary>
        private void InitializeDirectionsAndLines()
        {
            var targetNum = _targetDirections.Length;
            for (var i = 0 ; i < targetNum ; i++) {
                var direction = _targetDirections[i];
                var line = _targetLines[i];
                if (direction == EGimmickDirection.Random) {
                    if (i == 0) { // 最初の方向は制限ないのでそのまま乱数生成
                        direction = (EGimmickDirection)Enum.ToObject(typeof(EGimmickDirection), GimmickLibrary.SamplingArrayIndex(_randomDirections) + 1);
                    } else { // それ以降は前回の結果に依存する
                        var previousLine = _targetLines[i - 1];
                        var previousDirection = _targetDirections[i - 1];

                        if (GimmickLibrary.IsHorizontal(previousDirection)) { // 左右を移動している場合
                            if (previousLine == (int)ERow.Fifth) { // 最下行
                                direction = EGimmickDirection.ToUp;
                            } else if (previousLine == (int)ERow.First) { // 最上行
                                direction = EGimmickDirection.ToBottom;
                            } else {
                                var tempRandomDirections = _randomDirections.ToArray(); // 左右を除いた乱数配列
                                tempRandomDirections[(int)EGimmickDirection.ToLeft - 1] = tempRandomDirections[(int)EGimmickDirection.ToRight - 1] = 0;
                                direction = (EGimmickDirection)Enum.ToObject(typeof(EGimmickDirection), GimmickLibrary.SamplingArrayIndex(tempRandomDirections) + 1);
                            }
                        } else if (GimmickLibrary.IsVertical(previousDirection)) { // 上下を移動している場合
                            if (previousLine == (int)EColumn.Left) { // 最左列
                                direction = EGimmickDirection.ToRight;
                            } else if (previousLine == (int)EColumn.Right) { // 最右列
                                direction = EGimmickDirection.ToLeft;
                            } else {
                                var tempRandomDirections = _randomDirections.ToArray(); // 上下を除いた乱数配列
                                tempRandomDirections[(int)EGimmickDirection.ToUp - 1] = tempRandomDirections[(int)EGimmickDirection.ToBottom - 1] = 0;
                                direction = (EGimmickDirection)Enum.ToObject(typeof(EGimmickDirection), GimmickLibrary.SamplingArrayIndex(tempRandomDirections) + 1);
                            }
                        }
                    }
                }

                if (line == -1) {
                    if ((i != 0 && GimmickLibrary.IsHorizontal(direction)) || (i == 0 && GimmickLibrary.IsVertical(direction))) {
                        line = GimmickLibrary.SamplingArrayIndex(_randomColumn) + 1;
                    } else if ((i != 0 && GimmickLibrary.IsVertical(direction)) || (i == 0 && GimmickLibrary.IsHorizontal(direction))) {
                        line = GimmickLibrary.SamplingArrayIndex(_randomRow) + 1;
                    }
                }

                _targetDirections[i] = direction;
                _targetLines[i] = line;

                // 警告の位置も計算しておく
                if (i == 0) {
                    _warningPosList.Add(CalculateFirstWarningPos(direction, line));
                } else {
                    _warningPosList.Add(CalculateOtherWarningPos(_targetDirections[i - 1], _targetLines[i - 1], _targetLines[i]));
                }
            }
        }

        /// <summary>
        /// 警告表示
        /// </summary>
        /// <param name="warningPos">表示する座標</param>
        /// <param name="direction">竜巻の次の移動方向</param>
        /// <param name="displayTime">表示時間</param>
        /// <returns></returns>
        private IEnumerator ShowWarning(Vector2 warningPos, EGimmickDirection? direction, float displayTime)
        {
            // 一個前の警告まだ消えていない
            if (_warningObj != null) {
                _warningPrefab.ReleaseInstance(_warningObj);
            }

            string addressKey;
            switch (direction) {
                case EGimmickDirection.ToLeft:
                    addressKey = Constants.Address.TURN_WARNING_LEFT_SPRITE;
                    break;
                case EGimmickDirection.ToRight:
                    addressKey = Constants.Address.TURN_WARNING_RIGHT_SPRITE;
                    break;
                case EGimmickDirection.ToUp:
                    addressKey = Constants.Address.TURN_WARNING_UP_SPRITE;
                    break;
                case EGimmickDirection.ToBottom:
                    addressKey = Constants.Address.TURN_WARNING_BOTTOM_SPRITE;
                    break;
                case EGimmickDirection.Random:
                default:
                    addressKey = Constants.Address.TORNADO_WARNING_SPRITE;
                    break;
            }

            var sprite = AddressableAssetManager.GetAsset<Sprite>(addressKey);

            AsyncOperationHandle<GameObject> warningOp;
            yield return warningOp = _warningPrefab.InstantiateAsync(warningPos, Quaternion.identity);

            _warningObj = warningOp.Result;
            _warningObj.GetComponent<SpriteRenderer>().sprite = sprite;
            _warningObj.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;

            _warningObj.GetComponent<SpriteUnifier>().Unify();

            // 画像の切り替えでチラつくので切り替えの後に表示する
            _warningObj.GetComponent<SpriteRenderer>().enabled = true;

            // 警告終わるまで待つ
            while ((displayTime -= Time.fixedDeltaTime) >= 0) yield return new WaitForFixedUpdate();

            if (_warningObj != null) {
                _warningPrefab.ReleaseInstance(_warningObj);
            }
        }

        /// <summary>
        /// 曲がり先の警告座標
        /// </summary>
        /// <param name="currentDirection">曲がる前の方向</param>
        /// <param name="currentLine">曲がる前の行列</param>
        /// <param name="nextLine">曲がる後の行列</param>
        /// <returns></returns>
        private Vector2 CalculateOtherWarningPos(EGimmickDirection currentDirection, int currentLine, int nextLine)
        {
            int col, row;
            if (GimmickLibrary.IsHorizontal(currentDirection)) {
                row = currentLine;
                col = nextLine;
            } else if (GimmickLibrary.IsVertical(currentDirection)) {
                col = currentLine;
                row = nextLine;
            } else {
                row = col = 1;
                throw new InvalidOperationException();
            }

            return BoardManager.Instance.GetTilePos(col - 1, row - 1);
        }

        /// <summary>
        /// 登場時の警告座標の計算
        /// </summary>
        /// <param name="direction">登場時方向</param>
        /// <param name="line">登場時目標行列</param>
        /// <returns></returns>
        private Vector2 CalculateFirstWarningPos(EGimmickDirection direction, int line)
        {
            Vector2 motionVector;
            Vector2 warningPosition;
            if (GimmickLibrary.IsHorizontal(direction)) {
                var sign = direction == EGimmickDirection.ToRight ? -1 : 1;
                // x座標は画面端、y座標は同じ行のタイルと同じ値
                warningPosition = new Vector2(sign * Constants.WindowSize.WIDTH / 2,
                    GameWindowController.Instance.GetTileHeight() * (Constants.StageSize.ROW / 2 + 1 - line));
                motionVector = direction == EGimmickDirection.ToLeft ?
                    Vector2.left :
                    Vector2.right;
            } else if (GimmickLibrary.IsVertical(direction)) {
                var sign = direction == EGimmickDirection.ToUp ? -1 : 1;
                // x座標は同じ列のタイルと同じ値、y座標は画面端
                warningPosition = new Vector2(GameWindowController.Instance.GetTileWidth() * (line - (Constants.StageSize.COLUMN / 2 + 1)),
                    sign * Constants.WindowSize.HEIGHT / 2);
                motionVector = direction == EGimmickDirection.ToUp ?
                    Vector2.up :
                    Vector2.down;
            } else {
                throw new NotImplementedException();
            }
            // 画面端から、警告の幅(or高さ)/2の分だけ画面内に移動させた座標に警告を配置
            var warningOffset = Constants.WindowSize.WIDTH * _WARNING_OFFSET_RATIO;
            warningPosition += Vector2.Scale(motionVector, new Vector2(warningOffset, warningOffset)) / 2;
            return warningPosition;
        }

        /// <summary>
        /// 移動方向の設定
        /// </summary>
        /// <param name="direction">移動方向</param>
        private void SetDirection(EGimmickDirection direction)
        {
            switch (direction) {
                case EGimmickDirection.ToUp:
                    _rigidBody.velocity = Vector2.up * _speed;
                    break;
                case EGimmickDirection.ToLeft:
                    _rigidBody.velocity = Vector2.left * _speed;
                    break;
                case EGimmickDirection.ToRight:
                    _rigidBody.velocity = Vector2.right * _speed;
                    break;
                case EGimmickDirection.ToBottom:
                    _rigidBody.velocity = Vector2.down * _speed;
                    break;
                case EGimmickDirection.Random:
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 初期位置設定
        /// </summary>
        /// <param name="direction">初期の移動方向</param>
        /// <param name="line">攻撃する行（1~5）／列（1~3）</param>
        private void SetInitialPosition(EGimmickDirection direction, int line)
        {
            float x = 0, y = 0;
            var tornadoSize = GetComponent<SpriteRenderer>().size;
            if (GimmickLibrary.IsHorizontal(direction)) {
                // 目標列の一番右端のタイルのY座標を取得
                var tileNum = line * Constants.StageSize.COLUMN;
                y = BoardManager.Instance.GetTilePos(tileNum).y;

                if (direction == EGimmickDirection.ToLeft) {
                    x = (Constants.WindowSize.WIDTH + tornadoSize.x) / 2;
                } else {
                    x = -(Constants.WindowSize.WIDTH + tornadoSize.x) / 2;
                }
            } else if (GimmickLibrary.IsVertical(direction)) {
                // 目標行の一列目のタイルのx座標を取得
                var tileNum = line;
                x = BoardManager.Instance.GetTilePos(tileNum).x;

                if (direction == EGimmickDirection.ToUp) {
                    y = -(Constants.WindowSize.HEIGHT + tornadoSize.y) / 2;
                } else {
                    y = (Constants.WindowSize.HEIGHT + tornadoSize.y) / 2;
                }
            } else {
                throw new NotImplementedException();
            }
            transform.position = new Vector2(x, y);
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            // 数字ボトルとの衝突以外は考えない
            var bottle = other.GetComponent<AbstractBottleController>();
            if (bottle == null || !bottle.IsAttackable || bottle.Invincible) return;

            // 衝突したオブジェクトは赤色に変える
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }

        protected override void OnEndGame()
        {
            _rigidBody.velocity = Vector2.zero;

            if (_warningObj != null) {
                _warningPrefab.ReleaseInstance(_warningObj);
            }
        }
    }
}
