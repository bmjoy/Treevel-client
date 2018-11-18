using UnityEngine;

namespace GamePlayScene.Panel
{
    public abstract class PanelController : MonoBehaviour
    {
        protected GamePlayDirector gamePlayDirector;

        protected virtual void Start()
        {
            gamePlayDirector = FindObjectOfType<GamePlayDirector>();
        }

        protected virtual void Update()
        {
        }

        public abstract void Initialize(GameObject finalTile);
    }
}
