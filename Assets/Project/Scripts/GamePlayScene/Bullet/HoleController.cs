using System.Collections;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public abstract class HoleController : BulletController
	{
		private int row;
		private int column;

		// コンストラクタがわりのメソッド
		public void Initialize(int row, int column, Vector3 holeWarningPosition)
		{
			this.row = row;
			this.column = column;
			transform.position = holeWarningPosition;
		}

		protected override void OnFail()
		{
		}

		public IEnumerator Delete()
		{
			var gamePlayDirector = FindObjectOfType<GamePlayDirector>();
			var tile = GameObject.Find("Tile" + ((row - 1) * 3 + column));
			// check whether panel exists on the tile
			if (tile.transform.childCount != 0)
			{
				gameObject.GetComponent<SpriteRenderer>().color = Color.red;
				gamePlayDirector.Dispatch(GamePlayDirector.GameState.Failure);
			}
			else
			{
				// display the hole betweeen tile layer and panel layer
				gameObject.GetComponent<Renderer>().sortingLayerName = "Hole";
				yield return new WaitForSeconds(0.5f);
				Destroy(gameObject);
			}
		}
	}
}
