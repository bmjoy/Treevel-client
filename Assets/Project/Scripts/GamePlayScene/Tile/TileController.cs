using JetBrains.Annotations;
using UnityEngine;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.Tile
{
	public abstract class TileController : MonoBehaviour
	{
		[CanBeNull] public GameObject rightTile;
		[CanBeNull] public GameObject leftTile;
		[CanBeNull] public GameObject upperTile;
		[CanBeNull] public GameObject lowerTile;

		private void Awake()
		{
			var tileWidth = GetComponent<SpriteRenderer>().size.x;
			var tileHeight = GetComponent<SpriteRenderer>().size.y;
			transform.localScale = new Vector2(TileSize.WIDTH / tileWidth, TileSize.HEIGHT / tileHeight);
			GetComponent<Renderer>().sortingLayerName = "Tile";
		}

		public void Initialize(Vector2 position, int tileNum)
		{
			transform.position = position;
			name = "Tile" + tileNum;
		}

		// 自身タイルの上下左右のタイルへの参照を入れる
		public void MakeRelation(GameObject rightTile, GameObject leftTile, GameObject upperTile, GameObject lowerTile)
		{
			this.rightTile = rightTile;
			this.leftTile = leftTile;
			this.upperTile = upperTile;
			this.lowerTile = lowerTile;
		}
	}
}
