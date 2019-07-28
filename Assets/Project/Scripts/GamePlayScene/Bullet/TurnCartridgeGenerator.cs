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
        [SerializeField] private GameObject turnCartridgePrefab;
        [SerializeField] private GameObject turnCartridgeWarningPrefab;

        // 曲がる方向
        [CanBeNull] private int[] turnDirection = null;

        // 曲がる場所
        [CanBeNull] private int[] turnLine = null;

        // 曲がる方向をランダムに決めるときの各方向の重み
        private int[] randomTurnDirections = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(ECartridgeDirection)).Length - 1);

        // 曲がる行をランダムに決めるときの各行の重み
        private int[] randomTurnRow = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(ERow)).Length - 1);

        // 曲がる列をランダムに決めるときの各列の重み
        private int[] randomTurnColumn = BulletLibrary.GetInitialArray(Enum.GetNames(typeof(EColumn)).Length - 1);

        public void Initialize(int ratio, ECartridgeDirection cartridgeDirection, ERow row, int[] turnDirection,
                               int[] turnLine)
        {
            Initialize(ratio, cartridgeDirection, row);
            this.turnDirection = turnDirection;
            this.turnLine = turnLine;
        }

        public void Initialize(int ratio, ECartridgeDirection cartridgeDirection, EColumn column, int[] turnDirection,
            int[] turnLine)
        {
            Initialize(ratio, cartridgeDirection, column);
            this.turnDirection = turnDirection;
            this.turnLine = turnLine;
        }

        public void Initialize(int ratio, ECartridgeDirection cartridgeDirection, ERow row, int[] turnDirection,
            int[] turnLine, int[] randomCartridgeDirection, int[] randomRow, int[] randomColumn,
            int[] randomTurnDirections, int[] randomTurnRow, int[] randomTurnColumn)
        {
            Initialize(ratio, cartridgeDirection, row, randomCartridgeDirection, randomRow, randomColumn);
            this.turnDirection = turnDirection;
            this.turnLine = turnLine;
            this.randomTurnDirections = randomTurnDirections;
            this.randomTurnRow = randomTurnRow;
            this.randomTurnColumn = randomTurnColumn;
        }

        public void Initialize(int ratio, ECartridgeDirection cartridgeDirection, EColumn column, int[] turnDirection,
            int[] turnLine, int[] randomCartridgeDirection, int[] randomRow, int[] randomColumn,
            int[] randomTurnDirections, int[] randomTurnRow, int[] randomTurnColumn)
        {
            Initialize(ratio, cartridgeDirection, column, randomCartridgeDirection, randomRow, randomColumn);
            this.turnDirection = turnDirection;
            this.turnLine = turnLine;
            this.randomTurnDirections = randomTurnDirections;
            this.randomTurnRow = randomTurnRow;
            this.randomTurnColumn = randomTurnColumn;
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
            var warning = Instantiate(turnCartridgeWarningPrefab);
            warning.GetComponent<Renderer>().sortingOrder = bulletId;
            var warningScript = warning.GetComponent<CartridgeWarningController>();
            var bulletMotionVector =
                warningScript.Initialize(ECartridgeType.Turn, nextCartridgeDirection, nextCartridgeLine);

            // 警告の表示時間だけ待つ
            yield return new WaitForSeconds(BulletWarningController.WARNING_DISPLAYED_TIME);
            // 警告を削除する
            Destroy(warning);

            // ゲームが続いているなら銃弾を作成する
            if (gamePlayDirector.state == GamePlayDirector.EGameState.Playing) {
                int[] nextCartridgeTurnDirection = turnDirection ?? new int[] {
                    GetRandomTurnDirection(nextCartridgeDirection, nextCartridgeLine)
                };

                int[] nextCartridgeTurnLine = turnLine;
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

                var cartridge = Instantiate(turnCartridgePrefab);
                cartridge.GetComponent<TurnCartridgeController>().Initialize(nextCartridgeDirection, nextCartridgeLine,
                    bulletMotionVector, nextCartridgeTurnDirection, nextCartridgeTurnLine);

                // 同レイヤーのオブジェクトの描画順序の制御
                cartridge.GetComponent<Renderer>().sortingOrder = bulletId;
            }
        }

        /* 曲がる方向を重みに基づきランダムに決定する */
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
                var randomValue = new System.Random().Next(randomTurnDirections[cartridgeLocalLeft - 1] +
                    randomTurnDirections[cartridgeLocalRight - 1]) + 1;
                // 乱数に基づいてCartridgeから見て右または左のどちらかの方向を選択する
                randomTurnDirection = randomValue <= randomTurnDirections[cartridgeLocalLeft - 1]
                    ? (int) Enum.ToObject(typeof(ECartridgeDirection), cartridgeLocalLeft)
                    : (int) Enum.ToObject(typeof(ECartridgeDirection), cartridgeLocalRight);
            }

            return randomTurnDirection;
        }

        /* 曲がる行を重みに基づき決定する*/
        private int GetTurnRow()
        {
            var index = BulletLibrary.SamplingArrayIndex(randomRow) + 1;
            return (int) Enum.ToObject(typeof(ERow), index);
        }

        /* 曲がる列を重みに基づき決定する */
        private int GetTurnColumn()
        {
            var index = BulletLibrary.SamplingArrayIndex(randomColumn) + 1;
            return (int) Enum.ToObject(typeof(EColumn), index);
        }
    }
}
