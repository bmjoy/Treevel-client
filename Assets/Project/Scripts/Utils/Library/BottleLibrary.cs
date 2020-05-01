using System.Linq;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.Utils.Library
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

        private static NormalBottleController[] _orderedBottles = null;
        public static NormalBottleController[] OrderedAttackableBottles
        {
            get {
                if (_orderedBottles == null) {
                    // TODO find by interface
                    _orderedBottles = GameObject.FindObjectsOfType<NormalBottleController>()
                        .Concat(GameObject.FindObjectsOfType<LifeBottleController>())
                        .OrderBy(bottle => bottle.Id).ToArray();
                }
                return _orderedBottles;
            }
        }
    }
}
