using UnityEngine;

namespace Project.Scripts.GamePlayScene.Panel
{
	public abstract class PanelController : MonoBehaviour
	{
		protected GamePlayDirector gamePlayDirector;

		protected virtual void Start()
		{
			gamePlayDirector = FindObjectOfType<GamePlayDirector>();
		}

		public abstract void Initialize(GameObject finalTile);
	}
}
