using System.Collections.Generic;
using System.Linq;
using Treevel.Modules.GamePlayScene.Bottle;
using UnityEngine;

namespace Treevel.Common.Utils
{
    public static class BottleLibrary
    {
        /// <summary>
        /// ボトルのIDでボトルのゲームオブジェクトを取得
        /// </summary>
        /// <param name="bottleId"> ボトルのID </param>
        /// <returns> Bottle オブジェクト </returns>
        public static GameObject GetBottle(int bottleId)
        {
            var bottles = GameObject.FindObjectsOfType(typeof(GoalBottleController)) as GoalBottleController[];
            // ボトルの番号がbottleNumの唯一のボトルを探す、二個以上もしくは0個の場合は InvalidOperationExceptionがスローされる
            return bottles.Single(bottle => bottle.Id == bottleId).gameObject;
        }

        private static IEnumerable<BottleControllerBase> _orderedBottles;

        public static IEnumerable<BottleControllerBase> OrderedAttackableBottles {
            get {
                if (_orderedBottles == null) {
                    _orderedBottles = GameObject.FindObjectsOfType<BottleControllerBase>()
                        .Where(bottle => bottle.IsAttackable)
                        .OrderBy(bottle => bottle.Id);
                }

                return _orderedBottles;
            }
        }
    }
}
