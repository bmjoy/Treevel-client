using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts.StageSelectScene.OverlappedScrollRect
{
    public static class FindEventInterface
    {
        public static void TransmitParentEventSystemHandler<T>(this Transform self, Action<T> action)
        where T : IEventSystemHandler
        {
            var parent = self.transform.parent;

            // 親がいなくなるまで探索する
            while (parent != null) {
                foreach (var component in parent.GetComponents<Component>()) {
                    if (component is T) action((T)(IEventSystemHandler) component);
                }

                parent = parent.parent;
            }
        }
    }
}
