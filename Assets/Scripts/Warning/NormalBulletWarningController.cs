using System.Collections;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;

namespace Warning
{
    public class NormalBulletWarningController : CartridgeWarningController
    {
        public override void Initialize()
        {
            localScale = (float) (WindowSize.WIDTH * 0.15);
            this.transform.localScale *= new Vector2(localScale, localScale);
        }

        public override void deleteWarning(GameObject bullet)
        {
            bullet.active = false;
            StartCoroutine(delete(bullet));
        }

        private IEnumerator delete(GameObject bullet)
        {
            yield return new WaitForSeconds(1.0f);
            bullet.active = true;
            Destroy(gameObject);
        }
    }
}
