using System.Collections;
using UnityEngine;

namespace Bullet
{
    public class BulletGenerator : MonoBehaviour
    {
        // normalBullerのPrefab
        public GameObject normalBulletPrefab;

        // Use this for initialization
        private void Start()
        {
            // 銃弾の初期位置
            Vector2 position = new Vector2(3.5f, 1.0f);
            // 銃弾の移動ベクトル
            Vector2 motionVector = new Vector2(-1.0f, 0.0f);
            // 銃弾を作るインターバル
            var createInterval = 1.0f;
            // coroutineの開始
            IEnumerator coroutine = CreateBullet(position, motionVector, createInterval);
            StartCoroutine(coroutine);

            position = new Vector2(-2.0f, -2.0f);
            motionVector = new Vector2(1.0f, 1.0f);
            createInterval = 1.0f;
            coroutine = CreateBullet(position, motionVector, createInterval);
            StartCoroutine(coroutine);
        }

        // Update is called once per frame
        private void Update()
        {
        }

        // 指定した座標(x, y)に一定の時間間隔(interval)で銃弾を作成するメソッド
        private IEnumerator CreateBullet(Vector2 position, Vector2 motionVector, float interval)
        {
            while (true)
            {
                // normalBulletPrefabのGameObjectを作成
                GameObject bullet = Instantiate(normalBulletPrefab) as GameObject;
                // 座標を指定
                bullet.transform.position = position;
                // SortingLayerを指定
                bullet.GetComponent<Renderer>().sortingLayerName = "Bullet";
                // 変数の初期設定
                NormalBulletController b = bullet.GetComponent<NormalBulletController>();
                b.Initialize(motionVector);
                // 一定時間(interval)待つ
                yield return new WaitForSeconds(interval);
            }
        }
    }
}
