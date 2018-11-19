using System;
using System.Collections;
using System.Collections.Generic;
using GamePlayScene.Warning;
using UnityEngine;

namespace GamePlayScene.Bullet
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
            List<IEnumerator> coroutines = new List<IEnumerator>();
            startTime = Time.time;
            switch (stageNum)
            {
                case "1":
                    // 銃弾の初期位置
                    Vector2 position = new Vector2(6.5f, 4.61f);
                    // 銃弾の移動ベクトル
                    Vector2 motionVector = new Vector2(-1.0f, 0.0f);
                    // 銃弾の出現時刻
                    const float appearanceTiming = 1.0f;
                    // 銃弾を作るインターバル
                    const float createInterval = 2.0f;
                    // coroutineのリスト
                    coroutines.Add(CreateBullet(position, motionVector, appearanceTiming, createInterval));
                    coroutines.Add(CreateBullet(position: new Vector2(-6.5f, 6.0f),
                        motionVector: new Vector2(1.0f, 0.0f),
                        appearanceTiming: 2.0f, interval: 4.0f));
                    break;
                case "2":
                    coroutines.Add(CreateBullet(position: new Vector2(-6.5f, 6.0f),
                        motionVector: new Vector2(1.0f, 0.0f),
                        appearanceTiming: 2.0f, interval: 4.0f));
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }

            foreach (IEnumerator coroutine in coroutines) StartCoroutine(coroutine);
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
                NormalCartridgeController cartridgeScript = bullet.GetComponent<NormalCartridgeController>();
                cartridgeScript.Initialize(position, motionVector);

                // emerge a bullet warning
                GameObject warning = Instantiate(normalBulletWarningPrefab);
                warning.GetComponent<Renderer>().sortingLayerName = "Warning";
                NormalCartridgeWarningController warningScript = warning.GetComponent<NormalCartridgeWarningController>();
                warningScript.Initialize(bullet.transform.position, cartridgeScript.motionVector,
                    cartridgeScript.localScale, cartridgeScript.originalWidth, cartridgeScript.originalHeight);

                // delete the bullet warning
                warningScript.DeleteWarning(bullet);

                currentTime = Time.time;
                // 一定時間(interval)待つ
                yield return new WaitForSeconds(appearanceTiming + interval * sum - (currentTime - startTime));
            }
        }
    }
}
