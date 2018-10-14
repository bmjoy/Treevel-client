using System.Collections;
using UnityEngine;

namespace Warning
{
    public class WarningController : MonoBehaviour
    {
        public virtual void Initialize(Vector2 bulletPosition, Vector2 bulletMotionVector, float bulletLocalScale)
        {
        }

        public void deleteWarning(GameObject bullet)
        {
            bullet.SetActive(false);
            StartCoroutine(delete(bullet));
        }

        private IEnumerator delete(GameObject bullet)
        {
            yield return new WaitForSeconds(1.0f);
            bullet.SetActive(true);
            Destroy(gameObject);
        }
    }
}
