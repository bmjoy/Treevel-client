using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bullet
{
	public class NormalHoleController : HoleController
	{
		protected override void Awake()
		{
			base.Awake();
			transform.localScale = new Vector2(HoleSize.WIDTH / originalWidth, HoleSize.HEIGHT / originalHeight) *
			                       LOCAL_SCALE;
		}
	}
}
