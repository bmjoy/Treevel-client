using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.GamePlayScene.Bullet;
using Project.Scripts.GamePlayScene.Panel;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils.Definitions;


namespace Project.Scripts.GamePlayScene
{
	public class StageGenerator : MonoBehaviour
	{
		private TileGenerator tileGenerator;

		private PanelGenerator panelGenerator;

		private BulletGenerator bulletGenerator;

		private void Awake()
		{
			tileGenerator = GameObject.Find("TileGenerator").GetComponent<TileGenerator>();
			panelGenerator = GameObject.Find("PanelGenerator").GetComponent<PanelGenerator>();
			bulletGenerator = GameObject.Find("BulletGenerator").GetComponent<BulletGenerator>();
		}

		public void CreateStages(int stageId)
		{
			List<IEnumerator> coroutines = new List<IEnumerator>();

			switch (stageId)
			{
				case 1:
					// 銃弾実体生成
					coroutines.Add(bulletGenerator.CreateCartridge(
						cartridgeType: CartridgeType.Turn,
						appearanceTime: 1.0f,
						interval: 1.0f,
						direction: CartridgeDirection.ToLeft,
						line: (int) Row.Second,
						loop: true,
						additionalInfo: BulletGenerator.SetTurnCartridgeInfo(
							turnDirection: new[] {(int) CartridgeDirection.ToUp, (int) CartridgeDirection.ToLeft},
							turnLine: new[] {(int) Column.Right, (int) Row.First})));
					coroutines.Add(bulletGenerator.CreateCartridge(
						cartridgeType: CartridgeType.Normal,
						direction: CartridgeDirection.ToUp,
						line: (int) Column.Right,
						appearanceTime: 2.0f,
						interval: 4.0f));
					// タイル作成
					tileGenerator.CreateNormalTiles();
					// パネル作成
					panelGenerator.CreateDynamicDummyPanel(initialTileNum: 3);
					panelGenerator.CreateNumberPanel(panelNumber: 1, initialTileNum: 4, finalTileNum: 4);
					panelGenerator.CreateNumberPanel(panelNumber: 2, initialTileNum: 5, finalTileNum: 5);
					panelGenerator.CreateNumberPanel(panelNumber: 3, initialTileNum: 6, finalTileNum: 6);
					panelGenerator.CreateNumberPanel(panelNumber: 4, initialTileNum: 7, finalTileNum: 7);
					panelGenerator.CreateNumberPanel(panelNumber: 5, initialTileNum: 8, finalTileNum: 8);
					panelGenerator.CreateNumberPanel(panelNumber: 6, initialTileNum: 9, finalTileNum: 9);
					panelGenerator.CreateNumberPanel(panelNumber: 7, initialTileNum: 10, finalTileNum: 10);
					panelGenerator.CreateNumberPanel(panelNumber: 8, initialTileNum: 14, finalTileNum: 11);
					panelGenerator.CreateStaticDummyPanel(initialTileNum: 15);
					break;
				case 2:
					// 銃弾実体生成
					coroutines.Add(bulletGenerator.CreateCartridge(
						cartridgeType: CartridgeType.Normal,
						appearanceTime: 2.0f,
						interval: 0.5f,
						direction: CartridgeDirection.ToRight,
						line: (int) Row.Fifth,
						loop: false));
					coroutines.Add(bulletGenerator.CreateHole(
						holeType: HoleType.Normal,
						appearanceTime: 1.0f,
						interval: 2.0f,
						row: (int) Row.Second,
						column: (int) Column.Left));
					// タイル作成
					tileGenerator.CreateWarpTiles(firstTileNum: 2, secondTileNum: 14);
					tileGenerator.CreateNormalTiles();
					// パネル作成
					panelGenerator.CreateNumberPanel(panelNumber: 1, initialTileNum: 1, finalTileNum: 4);
					panelGenerator.CreateNumberPanel(panelNumber: 2, initialTileNum: 3, finalTileNum: 5);
					panelGenerator.CreateNumberPanel(panelNumber: 3, initialTileNum: 5, finalTileNum: 6);
					panelGenerator.CreateNumberPanel(panelNumber: 4, initialTileNum: 6, finalTileNum: 7);
					panelGenerator.CreateNumberPanel(panelNumber: 5, initialTileNum: 8, finalTileNum: 8);
					panelGenerator.CreateNumberPanel(panelNumber: 6, initialTileNum: 11, finalTileNum: 9);
					panelGenerator.CreateNumberPanel(panelNumber: 7, initialTileNum: 13, finalTileNum: 10);
					panelGenerator.CreateNumberPanel(panelNumber: 8, initialTileNum: 15, finalTileNum: 11);
					break;
				case 3:
					// 銃弾実体生成
					coroutines.Add(bulletGenerator.CreateHole(
						holeType: HoleType.Aiming,
						appearanceTime: 2.0f,
						interval: 2.0f,
						additionalInfo: BulletGenerator.SetAimingHoleInfo(
							aimingPanel: new[] {1})));
					// タイル作成
					tileGenerator.CreateWarpTiles(firstTileNum: 2, secondTileNum: 14);
					tileGenerator.CreateNormalTiles();
					// パネル作成
					panelGenerator.CreateNumberPanel(panelNumber: 1, initialTileNum: 1, finalTileNum: 4);
					panelGenerator.CreateNumberPanel(panelNumber: 2, initialTileNum: 3, finalTileNum: 5);
					panelGenerator.CreateNumberPanel(panelNumber: 3, initialTileNum: 5, finalTileNum: 6);
					panelGenerator.CreateNumberPanel(panelNumber: 4, initialTileNum: 6, finalTileNum: 7);
					panelGenerator.CreateNumberPanel(panelNumber: 5, initialTileNum: 8, finalTileNum: 8);
					panelGenerator.CreateNumberPanel(panelNumber: 6, initialTileNum: 11, finalTileNum: 9);
					panelGenerator.CreateNumberPanel(panelNumber: 7, initialTileNum: 13, finalTileNum: 10);
					panelGenerator.CreateNumberPanel(panelNumber: 8, initialTileNum: 15, finalTileNum: 11);
					break;
				default:
					throw new NotImplementedException();
			}

			// 銃弾の一括作成
			bulletGenerator.CreateBullets(coroutines);
		}
	}
}
