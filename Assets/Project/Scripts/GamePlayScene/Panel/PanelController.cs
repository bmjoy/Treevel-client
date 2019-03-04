using UnityEngine;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.Panel
{
	public abstract class PanelController : MonoBehaviour
	{
		protected virtual void Awake()
		{
			// パネル画像のサイズを取得
			var panelWidth = GetComponent<SpriteRenderer>().size.x;
			var panelHeight = GetComponent<SpriteRenderer>().size.y;
			// パネルの初期設定
			transform.localScale = new Vector2(PanelSize.WIDTH / panelWidth, PanelSize.HEIGHT / panelHeight);
			GetComponent<Renderer>().sortingLayerName = "Panel";
		}

		public void Initialize(int initialTileNum)
		{
			// 初期位置にするタイルを取得
			var initialTile = GameObject.Find("Tile" + initialTileNum);
			transform.parent = initialTile.transform;
			transform.position = initialTile.transform.position;
		}
	}
}
