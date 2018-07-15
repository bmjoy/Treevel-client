using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGenerator : MonoBehaviour {

    // normalBullerのPrefab
    public GameObject normalBulletPrefab;

	// Use this for initialization
    void Start () {
        float position_x = 3.5f;    // 銃弾のx座標の初期値
        float position_y = 0.0f;    // 銃弾のy座標の初期値
        float createInterval = 1.0f;    // 銃弾を作るインターバル
        // coroutineの開始
        var coroutine = createBullet(position_x, position_y, createInterval);
        StartCoroutine(coroutine);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // 指定した座標(x, y, 0)に一定の時間間隔(interval)で銃弾を作成するメソッド
    IEnumerator createBullet(float x, float y, float interval) {
        while(true) {
            // normalBulletPrefabのGameObjectを作成
            GameObject bullet = Instantiate(normalBulletPrefab) as GameObject;
            // 座標を指定
            bullet.transform.position = new Vector2(x, y);
            // 一定時間(interval)待つ
            yield return new WaitForSeconds(interval);
        }
    }
}
