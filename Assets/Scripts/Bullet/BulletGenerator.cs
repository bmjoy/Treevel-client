using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGenerator : MonoBehaviour {

    // normalBullerのPrefab
    public GameObject normalBulletPrefab;

	// Use this for initialization
    void Start ()
    {
        // 銃弾の初期位置
        Vector2 position = new Vector2(3.5f, 1.0f);
        // 銃弾の移動ベクトル
        Vector2 motion_vector = new Vector2(-1.0f, 0.0f);
        // 銃弾を作るインターバル
        float createInterval = 1.0f;
        // coroutineの開始
        var coroutine = createBullet(position, motion_vector, createInterval);
        StartCoroutine(coroutine);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    // 指定した座標(x, y)に一定の時間間隔(interval)で銃弾を作成するメソッド
    IEnumerator createBullet(Vector2 position, Vector2 motion_vector, float interval)
    {
        while(true)
        {
            // normalBulletPrefabのGameObjectを作成
            GameObject bullet = Instantiate(normalBulletPrefab) as GameObject;
            // 座標を指定
            bullet.transform.position = position;
            // 変数の初期設定
            CartridgeController b = bullet.GetComponent<NormalBulletController>();
            b.initialize(motion_vector);
            // 一定時間(interval)待つ
            yield return new WaitForSeconds(interval);
        }
    }
}
