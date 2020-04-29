using System.Linq;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.Utils.Library
{
    public static class BottleLibrary
    {
        /// <summary>
        /// ボトルの番号から，Number Bottle を返す
        /// </summary>
        /// <param name="bottleNum"> ボトルの番号 </param>
        /// <returns> Number Bottle オブジェクト </returns>
        public static GameObject GetBottle(int bottleNum)
        {
            var numberBottles = GameObject.FindGameObjectsWithTag(TagName.NUMBER_BOTTLE);
            // ボトルの番号がbottleNumの唯一のボトルを探す、二個以上もしくは0個の場合は InvalidOperationExceptionがスローされる
            return numberBottles.Single(bottle => bottle.GetComponent<NumberBottleController>()?.Id == bottleNum);
        }

        private static NumberBottleController[] _orderedBottles = null;
        public static NumberBottleController[] OrderedNumberBottles
        {
            get {
                if (_orderedBottles == null) {
                    _orderedBottles = GameObject.FindObjectsOfType<NumberBottleController>().OrderBy(bottle => bottle.Id).ToArray();
                }
                return _orderedBottles;
            }
        }
    }
}
