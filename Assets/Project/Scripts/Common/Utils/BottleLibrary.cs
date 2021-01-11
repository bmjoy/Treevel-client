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
        /// <returns> Number Bottle オブジェクト </returns>
        public static GameObject GetBottle(int bottleId)
        {
            var bottles = GameObject.FindObjectsOfType(typeof(NormalBottleController)) as NormalBottleController[];
            // ボトルの番号がbottleNumの唯一のボトルを探す、二個以上もしくは0個の場合は InvalidOperationExceptionがスローされる
            return bottles.Single(bottle => bottle.Id == bottleId).gameObject;
        }

        private static AbstractBottleController[] _orderedBottles = null;

        public static AbstractBottleController[] OrderedAttackableBottles {
            get {
                if (_orderedBottles == null) {
                    _orderedBottles = GameObject.FindObjectsOfType<AbstractBottleController>()
                        .Where(bottle => bottle.IsAttackable)
                        .OrderBy(bottle => bottle.Id).ToArray();
                }

                return _orderedBottles;
            }
        }
    }
}
