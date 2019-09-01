using UnityEngine;

namespace Project.Scripts.Utils.Definitions
{
    public static class BulletWarningParameter
    {
        /// <summary>
        /// 警告を表示する秒数
        /// </summary>
        public const float WARNING_DISPLAYED_TIME = 1.0f;
        　
        /// <summary>
        ///  警告を表示するフレーム数
        /// </summary>
        /// <returns></returns>
        public static int WARNING_DISPLAYED_FRAMES = (int) (1.0f / Time.fixedDeltaTime * WARNING_DISPLAYED_TIME);
    }
}
