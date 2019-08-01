﻿using System;
using System.Collections;
using Project.Scripts.GamePlayScene.BulletWarning;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class NormalCartridgeGenerator : BulletGenerator
	{
		[SerializeField] private GameObject normalCartridgePrefab;
		[SerializeField] private GameObject normalCartridgeWarningPrefab;

		// 銃弾の移動方向
		protected ECartridgeDirection cartridgeDirection;

		// 移動方向に応じて行または列は決まるので、何番目かの情報を保存する
		protected int line;

		// 銃弾がどの方向に進行するかをランダムに決めるときの各方向の重み
		private int[] randomCartridgeDirection =
			SetInitialRatio(Enum.GetNames(typeof(ECartridgeDirection)).Length - 1);

		// 銃弾がどの行に出現するかをランダムに決めるときの各行の重み
		protected int[] randomRow = SetInitialRatio(Enum.GetNames(typeof(Row)).Length - 1);

		// 銃弾がどの列に出現するかをランダムに決めるときの各列の重み
		protected int[] randomColumn = SetInitialRatio(Enum.GetNames(typeof(Column)).Length - 1);

		/* メンバ変数の初期化を行う
		   行と列の違いおよび、ランダムに値を決めるかどうかでオーバーロードしている */
		public void Initialize(int ratio, ECartridgeDirection cartridgeDirection, Row row)
		{
			this.ratio = ratio;
			this.cartridgeDirection = cartridgeDirection;
			line = (int) row;
		}

		public void Initialize(int ratio, ECartridgeDirection cartridgeDirection, Column column)
		{
			this.ratio = ratio;
			this.cartridgeDirection = cartridgeDirection;
			line = (int) column;
		}

		public void Initialize(int ratio, ECartridgeDirection cartridgeDirection, Row row,
			int[] randomCartridgeDirection, int[] randomRow, int[] randomColumn)
		{
			Initialize(ratio, cartridgeDirection, row);
			this.randomCartridgeDirection = randomCartridgeDirection;
			this.randomRow = randomRow;
			this.randomColumn = randomColumn;
		}

		public void Initialize(int ratio, ECartridgeDirection cartridgeDirection, Column column,
			int[] randomCartridgeDirection, int[] randomRow, int[] randomColumn)
		{
			Initialize(ratio, cartridgeDirection, column);
			this.randomCartridgeDirection = randomCartridgeDirection;
			this.randomRow = randomRow;
			this.randomColumn = randomColumn;
		}

		/* 1つの銃弾を生成する */
		public override IEnumerator CreateBullet(int bulletId)
		{
			// 銃弾の出現方向を指定する
			var nextCartridgeDirection = (cartridgeDirection == ECartridgeDirection.Random)
				? GetCartridgeDirection()
				: cartridgeDirection;

			// 銃弾の出現する場所を指定する
			var nextCartridgeLine = line;
			if (nextCartridgeLine == (int) Row.Random)
			{
				switch (nextCartridgeDirection)
				{
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

			// warningの作成
			var warning = Instantiate(normalCartridgeWarningPrefab);
			warning.GetComponent<Renderer>().sortingOrder = bulletId;
			var warningScript = warning.GetComponent<CartridgeWarningController>();
			var bulletMotionVector =
				warningScript.Initialize(ECartridgeType.Normal, nextCartridgeDirection, nextCartridgeLine);

			// 警告の表示時間だけ待つ
			yield return new WaitForSeconds(BulletWarningController.WARNING_DISPLAYED_TIME);
			// 警告を削除する
			Destroy(warning);

			// ゲームが続いているなら銃弾を作成する
			if (gamePlayDirector.state == GamePlayDirector.EGameState.Playing)
			{
				var cartridge = Instantiate(normalCartridgePrefab);
				cartridge.GetComponent<NormalCartridgeController>()
					.Initialize(nextCartridgeDirection, nextCartridgeLine, bulletMotionVector);

				// 同レイヤーのオブジェクトの描画順序の制御
				cartridge.GetComponent<Renderer>().sortingOrder = bulletId;
			}
		}

		/* Cartridgeの移動方向を重みに基づき決定する */
		protected ECartridgeDirection GetCartridgeDirection()
		{
			var index = GetRandomParameter(randomCartridgeDirection) + 1;
			return (ECartridgeDirection) Enum.ToObject(typeof(ECartridgeDirection), index);
		}

		/* Cartridgeの出現する行を重みに基づき決定する*/
		protected int GetRow()
		{
			var index = GetRandomParameter(randomRow) + 1;
			return (int) Enum.ToObject(typeof(Row), index);
		}

		/* Cartridgeの出現する列を重みに基づき決定する */
		protected int GetColumn()
		{
			var index = GetRandomParameter(randomColumn) + 1;
			return (int) Enum.ToObject(typeof(Column), index);
		}
	}
}
