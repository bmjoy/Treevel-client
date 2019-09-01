using UnityEngine;
using System;
using System.Collections;
using JetBrains.Annotations;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;

namespace Project.Scripts.GamePlayScene.Bullet
{
    public class TurnCartridgeGenerator : NormalCartridgeGenerator
    {
        /// <summary>
        /// TurnCartridgeのPrefab
        /// </summary>
        [SerializeField] private GameObject _turnCartridgePrefab;
        /// <summary>
        /// TurnCartridgeWarningのPrefab
        /// </summary>
        [SerializeField] private GameObject _turnCartridgeWarningPrefab;

        /// <summary>
        /// 曲がる方向
        /// </summary>
        [CanBeNull] private int[] _turnDirection = null;

        /// <summary>
        /// 曲がる行(または列)
        /// </summary>
        [CanBeNull] private int[] _turnLine = null;

        /// <summary>
        /// 曲がる方向の重み
        /// </summary>
        /// <returns></returns>
        private int[] _randomTurnDirections = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(ECartridgeDirection)).Length - 1);

        /// <summary>
        /// 曲がる行の重み
        /// </summary>
        /// <returns></returns>
        private int[] _randomTurnRow = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(ERow)).Length - 1);

        /// <summary>
        /// 曲がる列の重み
        /// </summary>
        /// <returns></returns>
        private int[] _randomTurnColumn = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(EColumn)).Length - 1);

        /// <summary>
        /// 特定の行を移動し、特定の列で特定の方向に曲がるTurnHoleを生成するGeneratorの初期化
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="cartridgeDirection"> 移動方向 </param>
        /// <param name="row"> 移動する行 </param>
        /// <param name="turnDirection"> 曲がる方向 </param>
        /// <param name="turnLine"> 曲がる列 </param>
        public void Initialize(int ratio, ECartridgeDirection cartridgeDirection, ERow row, int[] turnDirection,
            int[] turnLine)
        {
            Initialize(ratio, cartridgeDirection, row);
            this._turnDirection = turnDirection;
            this._turnLine = turnLine;
        }

        /// <summary>
        /// 特定の列を移動し、特定の行で特定の方向に曲がるTurnHoleを生成するGeneratorの初期化
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="cartridgeDirection"> 移動方向 </param>
        /// <param name="column"> 移動する列 </param>
        /// <param name="turnDirection"> 曲がる方向 </param>
        /// <param name="turnLine"> 曲がる行 </param>
        public void Initialize(int ratio, ECartridgeDirection cartridgeDirection, EColumn column, int[] turnDirection,
            int[] turnLine)
        {
            Initialize(ratio, cartridgeDirection, column);
            this._turnDirection = turnDirection;
            this._turnLine = turnLine;
        }

        /// <summary>
        /// ランダムな行(または列)を移動し、ランダムな列でランダムな方向に曲がるTurnHoleを生成するGeneratorの初期化
        /// </summary>
        /// <param name="ratio"> Generatorの出現割合 </param>
        /// <param name="randomCartridgeDirection"> 移動する方向の重み </param>
        /// <param name="randomRow"> 移動する行の重み </param>
        /// <param name="randomColumn"> 移動する列の重み </param>
        /// <param name="randomTurnDirections"> 曲がる方向の重み </param>
        /// <param name="randomTurnRow"> 曲がる行の重み </param>
        /// <param name="randomTurnColumn"> 曲がる列の重み </param>
        public void Initialize(int ratio,
            int[] randomCartridgeDirection, int[] randomRow, int[] randomColumn,
            int[] randomTurnDirections, int[] randomTurnRow, int[] randomTurnColumn)
        {
            Initialize(ratio, randomCartridgeDirection, randomRow, randomColumn);
            this._randomTurnDirections = randomTurnDirections;
            this._randomTurnRow = randomTurnRow;
            this._randomTurnColumn = randomTurnColumn;
        }

        public override IEnumerator CreateBullet(int bulletId)
        {
            // 銃弾の移動方向を指定する
            var nextCartridgeDirection = (cartridgeDirection == ECartridgeDirection.Random)
                ? GetCartridgeDirection()
                : cartridgeDirection;

            // 銃弾の出現する場所を指定する
            var nextCartridgeLine = line;
            if (nextCartridgeLine == (int) ERow.Random) {
                switch (nextCartridgeDirection) {
                    case ECartridgeDirection.ToLeft:
                    case ECartridgeDirection.ToRight:
                        nextCartridgeLine = GetRow();
                        break;
                    case ECartridgeDirection.ToUp:
                    case ECartridgeDirection.ToBottom:
                        nextCartridgeLine = GetColumn();
                        break;
                    case ECartridgeDirection.Random:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            // 警告の作成
            var warning = Instantiate(_turnCartridgeWarningPrefab);
            warning.GetComponent<Renderer>().sortingOrder = bulletId;
            var warningScript = warning.GetComponent<CartridgeWarningController>();
            var bulletMotionVector =
                warningScript.Initialize(ECartridgeType.Turn, nextCartridgeDirection, nextCartridgeLine);

            // 警告の表示時間だけ待つ
            for (int index = 0; index < BulletWarningParameter.WARNING_DISPLAYED_FRAMES; index++) yield return new WaitForFixedUpdate();
            // 警告を削除する
            Destroy(warning);

            // ゲームが続いているなら銃弾を作成する
            if (gamePlayDirector.state == GamePlayDirector.EGameState.Playing) {
                int[] nextCartridgeTurnDirection = _turnDirection ?? new int[] {
                    GetRandomTurnDirection(nextCartridgeDirection, nextCartridgeLine)
                };

                int[] nextCartridgeTurnLine = _turnLine;
                if (nextCartridgeTurnLine == null) {
                    switch (nextCartridgeDirection) {
                        case ECartridgeDirection.ToLeft:
                        case ECartridgeDirection.ToRight:
                            nextCartridgeTurnLine = new int[] {GetTurnColumn()};
                            break;
                        case ECartridgeDirection.ToUp:
                        case ECartridgeDirection.ToBottom:
                            nextCartridgeTurnLine = new int[] {GetTurnRow()};
                            break;
                        case ECartridgeDirection.Random:
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }

                var cartridge = Instantiate(_turnCartridgePrefab);
                cartridge.GetComponent<TurnCartridgeController>().Initialize(nextCartridgeDirection, nextCartridgeLine,
                    bulletMotionVector, nextCartridgeTurnDirection, nextCartridgeTurnLine);

                // 同レイヤーのオブジェクトの描画順序の制御
                cartridge.GetComponent<Renderer>().sortingOrder = bulletId;
            }
        }

        /// <summary>
        /// 曲がる方向を重みに基づき決定する
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private int GetRandomTurnDirection(ECartridgeDirection direction, int line)
        {
            var randomTurnDirection = 0;
            // 最上行または最下行を移動している場合
            if ((direction == ECartridgeDirection.ToLeft || direction == ECartridgeDirection.ToRight) &&
                (line == (int) ERow.First || line == (int) ERow.Fifth)) {
                if (line == (int) ERow.First) {
                    randomTurnDirection = (int) ECartridgeDirection.ToBottom;
                } else if (line == (int) ERow.Fifth) {
                    randomTurnDirection = (int) ECartridgeDirection.ToUp;
                }
            }
            // 最左列または最も最右列を移動している場合
            else if ((direction == ECartridgeDirection.ToUp || direction == ECartridgeDirection.ToBottom) &&
                (line == (int) EColumn.Left || line == (int) EColumn.Right)) {
                if (line == (int) EColumn.Left) {
                    randomTurnDirection = (int) ECartridgeDirection.ToRight;
                } else if (line == (int) EColumn.Right) {
                    randomTurnDirection = (int) ECartridgeDirection.ToLeft;
                }
            }
            // 上記以外の場合(ランダムに決定する)
            else {
                // Cartridgeの移動方向に応じてCartridgeから見た右方向、左方向を取得する
                int cartridgeLocalLeft;
                int cartridgeLocalRight;
                switch (direction) {
                    case ECartridgeDirection.ToLeft:
                        cartridgeLocalLeft = (int) ECartridgeDirection.ToBottom;
                        cartridgeLocalRight = (int) ECartridgeDirection.ToUp;
                        break;
                    case ECartridgeDirection.ToRight:
                        cartridgeLocalLeft = (int) ECartridgeDirection.ToUp;
                        cartridgeLocalRight = (int) ECartridgeDirection.ToBottom;
                        break;
                    case ECartridgeDirection.ToUp:
                        cartridgeLocalLeft = (int) ECartridgeDirection.ToLeft;
                        cartridgeLocalRight = (int) ECartridgeDirection.ToRight;
                        break;
                    case ECartridgeDirection.ToBottom:
                        cartridgeLocalLeft = (int) ECartridgeDirection.ToRight;
                        cartridgeLocalRight = (int) ECartridgeDirection.ToLeft;
                        break;
                    case ECartridgeDirection.Random:
                        throw new Exception();
                    default:
                        throw new NotImplementedException();
                }

                // 乱数を取得する
                var randomValue = new System.Random().Next(_randomTurnDirections[cartridgeLocalLeft - 1] +
                    _randomTurnDirections[cartridgeLocalRight - 1]) + 1;
                // 乱数に基づいてCartridgeから見て右または左のどちらかの方向を選択する
                randomTurnDirection = randomValue <= _randomTurnDirections[cartridgeLocalLeft - 1]
                    ? (int) Enum.ToObject(typeof(ECartridgeDirection), cartridgeLocalLeft)
                    : (int) Enum.ToObject(typeof(ECartridgeDirection), cartridgeLocalRight);
            }

            return randomTurnDirection;
        }

        /// <summary>
        /// 曲がる行を重みに基づき決定する
        /// </summary>
        /// <returns></returns>
        private int GetTurnRow()
        {
            var index = BulletLibrary.SamplingArrayIndex(randomRow) + 1;
            return (int) Enum.ToObject(typeof(ERow), index);
        }

        /// <summary>
        /// 曲がる列を重みに基づき決定する
        /// </summary>
        /// <returns></returns>
        private int GetTurnColumn()
        {
            var index = BulletLibrary.SamplingArrayIndex(randomColumn) + 1;
            return (int) Enum.ToObject(typeof(EColumn), index);
        }
    }
}
