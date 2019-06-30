using System;
using System.Collections;
using JetBrains.Annotations;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

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
		private int[] randomTurnDirections = SetInitialRatio(Enum.GetNames(typeof(CartridgeDirection)).Length - 1);

		// 曲がる行をランダムに決めるときの各行の重み
		private int[] randomTurnRow = SetInitialRatio(Enum.GetNames(typeof(Row)).Length - 1);

		// 曲がる列をランダムに決めるときの各列の重み
		private int[] randomTurnColumn = SetInitialRatio(Enum.GetNames(typeof(Column)).Length - 1);

		public void Initialize(int ratio, CartridgeDirection cartridgeDirection, Row row, int[] turnDirection,
			int[] turnLine)
		{
			Initialize(ratio, cartridgeDirection, row);
			this.turnDirection = turnDirection;
			this.turnLine = turnLine;
		}

		public void Initialize(int ratio, CartridgeDirection cartridgeDirection, Column column, int[] turnDirection,
			int[] turnLine)
		{
			Initialize(ratio, cartridgeDirection, column);
			this.turnDirection = turnDirection;
			this.turnLine = turnLine;
		}

		public void Initialize(int ratio, CartridgeDirection cartridgeDirection, Row row, int[] turnDirection,
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

		public void Initialize(int ratio, CartridgeDirection cartridgeDirection, Column column, int[] turnDirection,
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
			var nextCartridgeDirection = (cartridgeDirection == CartridgeDirection.Random)
				? GetCartridgeDirection()
				: cartridgeDirection;

			// 銃弾の出現する場所を指定する
			var nextCartridgeLine = line;
			if (nextCartridgeLine == (int) Row.Random)
			{
				switch (nextCartridgeDirection)
				{
					case CartridgeDirection.ToLeft:
					case CartridgeDirection.ToRight:
						nextCartridgeLine = GetRow();
						break;
					case CartridgeDirection.ToUp:
					case CartridgeDirection.ToBottom:
						nextCartridgeLine = GetColumn();
						break;
					case CartridgeDirection.Random:
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
				warningScript.Initialize(CartridgeType.Turn, nextCartridgeDirection, nextCartridgeLine);

			// 警告の表示時間だけ待つ
			yield return new WaitForSeconds(BulletWarningController.WARNING_DISPLAYED_TIME);
			// 警告を削除する
			Destroy(warning);

			// ゲームが続いているなら銃弾を作成する
			if (gamePlayDirector.state == GamePlayDirector.GameState.Playing)
			{
				int[] nextCartridgeTurnDirection = turnDirection ?? new int[]
				{
					GetRandomTurnDirection(nextCartridgeDirection, nextCartridgeLine)
				};

				int[] nextCartridgeTurnLine = turnLine;
				if (nextCartridgeTurnLine == null)
				{
					switch (nextCartridgeDirection)
					{
						case CartridgeDirection.ToLeft:
						case CartridgeDirection.ToRight:
							nextCartridgeTurnLine = new int[] {GetTurnColumn()};
							break;
						case CartridgeDirection.ToUp:
						case CartridgeDirection.ToBottom:
							nextCartridgeTurnLine = new int[] {GetTurnRow()};
							break;
						case CartridgeDirection.Random:
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
		private int GetRandomTurnDirection(CartridgeDirection direction, int line)
		{
			var randomTurnDirection = 0;
			// 最上行または最下行を移動している場合
			if ((direction == CartridgeDirection.ToLeft || direction == CartridgeDirection.ToRight) &&
			    (line == (int) Row.First || line == (int) Row.Fifth))
			{
				if (line == (int) Row.First)
				{
					randomTurnDirection = (int) CartridgeDirection.ToBottom;
				}
				else if (line == (int) Row.Fifth)
				{
					randomTurnDirection = (int) CartridgeDirection.ToUp;
				}
			}
			// 最左列または最も最右列を移動している場合
			else if ((direction == CartridgeDirection.ToUp || direction == CartridgeDirection.ToBottom) &&
			         (line == (int) Column.Left || line == (int) Column.Right))
			{
				if (line == (int) Column.Left)
				{
					randomTurnDirection = (int) CartridgeDirection.ToRight;
				}
				else if (line == (int) Column.Right)
				{
					randomTurnDirection = (int) CartridgeDirection.ToLeft;
				}
			}
			// 上記以外の場合(ランダムに決定する)
			else
			{
				// Cartridgeの移動方向に応じてCartridgeから見た右方向、左方向を取得する
				int cartridgeLocalLeft;
				int cartridgeLocalRight;
				switch (direction)
				{
					case CartridgeDirection.ToLeft:
						cartridgeLocalLeft = (int) CartridgeDirection.ToBottom;
						cartridgeLocalRight = (int) CartridgeDirection.ToUp;
						break;
					case CartridgeDirection.ToRight:
						cartridgeLocalLeft = (int) CartridgeDirection.ToUp;
						cartridgeLocalRight = (int) CartridgeDirection.ToBottom;
						break;
					case CartridgeDirection.ToUp:
						cartridgeLocalLeft = (int) CartridgeDirection.ToLeft;
						cartridgeLocalRight = (int) CartridgeDirection.ToRight;
						break;
					case CartridgeDirection.ToBottom:
						cartridgeLocalLeft = (int) CartridgeDirection.ToRight;
						cartridgeLocalRight = (int) CartridgeDirection.ToLeft;
						break;
					case CartridgeDirection.Random:
						throw new Exception();
					default:
						throw new NotImplementedException();
				}

				// 乱数を取得する
				var randomValue = new System.Random().Next(randomTurnDirections[cartridgeLocalLeft - 1] +
				                                           randomTurnDirections[cartridgeLocalRight - 1]) + 1;
				// 乱数に基づいてCartridgeから見て右または左のどちらかの方向を選択する
				randomTurnDirection = randomValue <= randomTurnDirections[cartridgeLocalLeft - 1]
					? (int) Enum.ToObject(typeof(CartridgeDirection), cartridgeLocalLeft)
					: (int) Enum.ToObject(typeof(CartridgeDirection), cartridgeLocalRight);
			}

			return randomTurnDirection;
		}

		/* 曲がる行を重みに基づき決定する*/
		private int GetTurnRow()
		{
			var index = GetRandomParameter(randomRow) + 1;
			return (int) Enum.ToObject(typeof(Row), index);
		}

		/* 曲がる列を重みに基づき決定する */
		private int GetTurnColumn()
		{
			var index = GetRandomParameter(randomColumn) + 1;
			return (int) Enum.ToObject(typeof(Column), index);
		}
	}
}
