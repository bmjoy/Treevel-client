using UnityEngine;

namespace Project.Scripts.GamePlayScene
{
    /// <summary>
    /// ギミックが場外になる判定とその破壊処理を行う
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Boundary : MonoBehaviour
    {

        private void OnTriggerExit2D(Collider2D other)
        {
            // OnEnterだと生成した直後に破壊されるのでExitの時破壊する
            Destroy(other.gameObject);
        }
    }
}
