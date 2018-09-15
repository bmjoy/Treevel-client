using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bullet
{
    public class BulletGenerator : MonoBehaviour
    {
        // normalBullerのPrefab
        public GameObject normalBulletPrefab;
        // Generatorが作成された時刻
        public float startTime;

        // Use this for initialization
        private void Start()
        {
            startTime = Time.time;
            // 銃弾の初期位置
            Vector2 position = new Vector2(3.5f, 1.0f);
            // 銃弾の移動ベクトル
            Vector2 motionVector = new Vector2(-1.0f, 0.0f);
            // 銃弾の出現時刻
            var appearanceTiming = 1.0f;
            // 銃弾を作るインターバル
            var createInterval = 1.0f;
            // coroutineのリスト
            var coroutines = new List<IEnumerator>
            {
                CreateBullet(position, motionVector, appearanceTiming, createInterval),
                CreateBullet(position: new Vector2(3.5f, 2.0f), motionVector: new Vector2(-1.0f, 0.0f),
                    appearanceTiming: 2.0f, interval: 1.0f),
                CreateBullet(position: new Vector2(3.5f, 0.0f), motionVector: new Vector2(-1.0f, 0.0f),
                    appearanceTiming: 3.0f, interval: 1.0f),
                CreateBullet(position: new Vector2(3.5f, 3.0f), motionVector: new Vector2(-1.0f, 0.0f),
                    appearanceTiming: 4.0f, interval: 1.0f),
                CreateBullet(position: new Vector2(3.5f, -1.0f), motionVector: new Vector2(-1.0f, 0.0f),
                    appearanceTiming: 5.0f, interval: 1.0f)
            };

            foreach (IEnumerator coroutine in coroutines)
            {
                StartCoroutine(coroutine);
            }
        }

        // Update is called once per frame
        private void Update()
        {
        }

        // 指定した座標(x, y)に一定の時間間隔(interval)で銃弾を作成するメソッド
        private IEnumerator CreateBullet(Vector2 position, Vector2 motionVector, float appearanceTiming, float interval)
        {
            var currentTime = Time.time;
            yield return new WaitForSeconds(appearanceTiming-(currentTime-startTime));

            // 出現させた銃弾の個数
            var sum = 1;
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

                currentTime = Time.time;
                // 一定時間(interval)待つ
                yield return new WaitForSeconds(appearanceTiming+interval*sum-(currentTime-startTime));
                sum++;
            }
        }
    }
}
