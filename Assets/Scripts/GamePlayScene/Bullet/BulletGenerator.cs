using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Warning;

namespace Bullet
{
    public class BulletGenerator : MonoBehaviour
    {
        // normalBullerのPrefab
        public GameObject normalBulletPrefab;
        public GameObject normalBulletWarningPrefab;

        // Generatorが作成された時刻
        public float startTime;

        public void CreateBullets(string stageNum)
        {
            List<IEnumerator> coroutines;
            switch (stageNum)
            {
                case "1":
                    startTime = Time.time;
                    // 銃弾の初期位置
                    Vector2 position = new Vector2(6.5f, 4.0f);
                    // 銃弾の移動ベクトル
                    Vector2 motionVector = new Vector2(-1.0f, 0.0f);
                    // 銃弾の出現時刻
                    var appearanceTiming = 1.0f;
                    // 銃弾を作るインターバル
                    var createInterval = 1.0f;
                    // coroutineのリスト
                    coroutines = new List<IEnumerator>
                    {
                        CreateBullet(position, motionVector, appearanceTiming, createInterval),
                        CreateBullet(position: new Vector2(-6.5f, 6.0f), motionVector: new Vector2(1.0f, 0.0f),
                            appearanceTiming: 2.0f, interval: 4.0f)
                    };

                    foreach (IEnumerator coroutine in coroutines) StartCoroutine(coroutine);
                    break;
                case "2":
                    coroutines = new List<IEnumerator>
                    {
                        CreateBullet(position: new Vector2(-6.5f, 6.0f), motionVector: new Vector2(1.0f, 0.0f),
                            appearanceTiming: 2.0f, interval: 4.0f)
                    };

                    foreach (IEnumerator coroutine in coroutines) StartCoroutine(coroutine);
                    break;
            }
        }

        // 指定した座標(x, y)に一定の時間間隔(interval)で銃弾を作成するメソッド
        private IEnumerator CreateBullet(Vector2 position, Vector2 motionVector, float appearanceTiming, float interval)
        {
            var currentTime = Time.time;

            // wait by the time the first bullet warning emerge
            // 1.0f equals to the period which the bullet warning is emerging
            yield return new WaitForSeconds(appearanceTiming - 1.0f - (currentTime - startTime));

            // the number of bullets which have emerged
            var sum = 0;

            while (true)
            {
                sum++;
                // normalBulletPrefabのGameObjectを作成
                GameObject bullet = Instantiate(normalBulletPrefab);
                // SortingLayerを指定
                bullet.GetComponent<Renderer>().sortingLayerName = "Bullet";
                // 変数の初期設定
                NormalBulletController bulletScript = bullet.GetComponent<NormalBulletController>();
                bulletScript.Initialize(position, motionVector);

                // emerge a bullet warning
                GameObject warning = Instantiate(normalBulletWarningPrefab);
                warning.GetComponent<Renderer>().sortingLayerName = "Warning";
                NormalBulletWarningController warningScript = warning.GetComponent<NormalBulletWarningController>();
                warningScript.Initialize(bullet.transform.position, bulletScript.motionVector,
                    bulletScript.localScale, bulletScript.originalWidth, bulletScript.originalHeight);

                // delete the bullet warning
                warningScript.deleteWarning(bullet);

                currentTime = Time.time;
                // 一定時間(interval)待つ
                yield return new WaitForSeconds(appearanceTiming + interval * sum - (currentTime - startTime));
            }
        }
    }
}
