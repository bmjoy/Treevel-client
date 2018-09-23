using Directors;
using UnityEngine;

namespace Bullet
{
    // Bulletに共通したフィールド、メソッドの定義
    public abstract class BulletController : MonoBehaviour
    {
        protected GamePlayDirector gamePlayDirector;

        // Use this for initialization
        protected virtual void Start()
        {
            gamePlayDirector = GameObject.Find("GamePlayDirector").GetComponent<GamePlayDirector>();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
        }

        protected virtual void FixedUpdate()
        {
        }
    }
}
