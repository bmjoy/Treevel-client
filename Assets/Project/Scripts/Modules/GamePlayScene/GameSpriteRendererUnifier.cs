using Treevel.Common.Components;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene
{
    [DefaultExecutionOrder(1)]
    public class GameSpriteRendererUnifier : SpriteRendererUnifier
    {
        protected override void SetBaseWidth()
        {
            baseWidth = GameWindowController.Instance.GetGameCoreSpaceWidth();
        }
    }
}
