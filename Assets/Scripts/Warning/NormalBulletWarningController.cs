using System.Collections;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;

namespace Warning
{
    public class NormalBulletWarningController : CartridgeWarningController
    {
        public override void Initialize()
        {
            width = (float) (WindowSize.WIDTH * 0.15);
            height = width;
            this.transform.localScale *= new Vector2(width, height);
        }

        public override void deleteWarning()
        {
            StartCoroutine(delete());
        }

        private IEnumerator delete()
        {
            yield return new WaitForSeconds(1.0f);
            Destroy(gameObject);
        }
    }
}
