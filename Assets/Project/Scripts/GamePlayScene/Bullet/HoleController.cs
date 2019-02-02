using System.Collections;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public abstract class HoleController : BulletController
	{
		private GamePlayDirector gamePlayDirector;
		private int row;
		private int column;

		private void Start()
		{
			gamePlayDirector = FindObjectOfType<GamePlayDirector>();
			StartCoroutine(Delete());
		}

		private IEnumerator Delete()
		{
			var tile = GameObject.Find("Tile" + ((row - 1) * 3 + column));
			// check whether panel exists on the tile
			if (tile.transform.childCount != 0)
			{
				gameObject.GetComponent<SpriteRenderer>().color = Color.red;
				gamePlayDirector.Dispatch(GamePlayDirector.GameState.Failure);
				yield return new WaitForSeconds(0.5f);
			}
			else
			{
				// display the hole betweeen tile layer and panel layer
				gameObject.GetComponent<Renderer>().sortingLayerName = "Hole";
				yield return new WaitForSeconds(0.5f);
				Destroy(gameObject);
			}
		}

		private void OnEnable()
		{
			GamePlayDirector.OnSucceed += OnSucceed;
			GamePlayDirector.OnFail += OnFail;
		}

		// コンストラクタがわりのメソッド
		public virtual void Initialize(int row, int column)
		{
			this.row = row;
			this.column = column;
			gameObject.SetActive(false);
			gameObject.GetComponent<Renderer>().sortingLayerName = "Bullet";
		}

		protected override void OnFail()
		{
		}
	}
}
