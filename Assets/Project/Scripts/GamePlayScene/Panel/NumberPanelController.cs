using UnityEngine;

namespace Project.Scripts.GamePlayScene.Panel
{
	public class NumberPanelController : DynamicPanelController
	{
		// 最終タイル
		private GameObject finalTile;

		// パネルが最終タイルにいるかどうかの状態
		public bool adapted;

		protected override void Awake()
		{
			base.Awake();
			name = "NumberPanel";
		}

		protected override void Start()
		{
			base.Start();
			// 初期状態で最終タイルにいるかどうかの状態を変える
			adapted = transform.parent.gameObject == finalTile;
		}

		public void Initialize(int initialTileNum, int finalTileNum)
		{
			Initialize(initialTileNum);
			finalTile = GameObject.Find("Tile" + finalTileNum);
		}

		protected override void UpdateTile(GameObject targetTile)
		{
			base.UpdateTile(targetTile);
			// 最終タイルにいるかどうかで状態を変える
			adapted = transform.parent.gameObject == finalTile;
			// 成功判定
			gamePlayDirector.CheckClear();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			// 銃弾との衝突以外は考えない（現状は，パネル同士での衝突は起こりえない）
			if (!other.gameObject.CompareTag("Bullet")) return;
			gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
			// 失敗状態に移行する
			gamePlayDirector.Dispatch(GamePlayDirector.GameState.Failure);
		}
	}
}
