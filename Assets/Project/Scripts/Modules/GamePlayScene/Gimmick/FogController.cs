using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Gimmick
{
    public class FogController : AbstractGimmickController
    {
        public override IEnumerator Trigger()
        {
            yield return null;
        } 

        protected override void OnEndGame()
        {
        }
    }
}
