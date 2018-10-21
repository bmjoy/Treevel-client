using System.Collections;
using Bullet;
using Directors;
using UnityEngine;

namespace Warning
{
    public class CartridgeWarningController : WarningController
    {
        public float localScale;
        public float originalHeight;
        public float originalWidth;

        private void OnEnable()
        {
            GamePlayDirector.OnSucceed += OnSucceed;
            GamePlayDirector.OnFail += OnFail;
        }

        private void OnDisable()
        {
            GamePlayDirector.OnSucceed -= OnSucceed;
            GamePlayDirector.OnFail -= OnFail;
        }

        private void OnFail()
        {
            Destroy(gameObject);
        }

        private void OnSucceed()
        {
            Destroy(gameObject);
        }


        public void deleteWarning(GameObject bullet)
        {
            var tempSpeed = bullet.GetComponent<CartridgeController>().speed;
            bullet.GetComponent<CartridgeController>().speed = 0.0f;
            StartCoroutine(delete(bullet, tempSpeed));
        }

        private IEnumerator delete(GameObject bullet, float speed)
        {
            yield return new WaitForSeconds(1.0f);
            bullet.GetComponent<CartridgeController>().speed = speed;
            Destroy(gameObject);
        }
    }
}
