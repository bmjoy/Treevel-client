using System.Collections;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class NormalHoleController : BulletController
	{
		protected int row;
		protected int column;

		protected override void Awake()
		{
			base.Awake();
			transform.localScale = new Vector2(HoleSize.WIDTH / originalWidth, HoleSize.HEIGHT / originalHeight) *
			                       LOCAL_SCALE;
		}

		// コンストラクタがわりのメソッド
		public virtual void Initialize(int row, int column, Vector2 holeWarningPosition)
		{
			this.row = row;
			this.column = column;
			transform.position = holeWarningPosition;
		}

		protected override void OnFail()
		{
		}

		// 当たり判定(holeの表示場所に、tileがあるかどうかを確認する)
		public IEnumerator Delete()
		{
			var gamePlayDirector = FindObjectOfType<GamePlayDirector>();
			var tile = TileLibrary.GetTile(row, column);
			// check whether panel exists on the tile
			if (tile.transform.childCount != 0)
			{
				var panel = tile.transform.GetChild(0);
				if (panel.CompareTag(TagName.NUMBER_PANEL))
				{
					// 数字パネルが銃弾の出現場所に存在する
					gameObject.GetComponent<SpriteRenderer>().color = Color.red;
					gamePlayDirector.Dispatch(GamePlayDirector.GameState.Failure);
				}
				else
				{
					// 数字パネル以外のパネルが銃弾の出現場所に存在する
					yield return new WaitForSeconds(0.5f);
					Destroy(gameObject);
				}
			}
			else
			{
				// 銃弾の出現場所にパネルが存在しない
				// タイルとパネルの間のレイヤー(Hole)に描画する
				gameObject.GetComponent<Renderer>().sortingLayerName = SortingLayerName.HOLE;
				yield return new WaitForSeconds(0.5f);
				Destroy(gameObject);
			}
		}
	}
}
