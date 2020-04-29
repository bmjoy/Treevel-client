using System.Linq;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.Utils.Library
{
    public static class BottleLibrary
    {
        /// <summary>
        /// ボトルの番号から，Number Panel を返す
        /// </summary>
        /// <param name="panelNum"> ボトルの番号 </param>
        /// <returns> Number Panel オブジェクト </returns>
        public static GameObject GetPanel(int panelNum)
        {
            var numberPanels = GameObject.FindGameObjectsWithTag(TagName.NUMBER_BOTTLE);
            // ボトルの番号がpanelNumの唯一のボトルを探す、二個以上もしくは0個の場合は InvalidOperationExceptionがスローされる
            return numberPanels.Single(panel => panel.GetComponent<NumberBottleController>()?.Id == panelNum);
        }

        private static NumberBottleController[] _orderedPanels = null;
        public static NumberBottleController[] OrderedNumberPanels
        {
            get {
                if (_orderedPanels == null) {
                    _orderedPanels = GameObject.FindObjectsOfType<NumberBottleController>().OrderBy(panel => panel.Id).ToArray();
                }
                return _orderedPanels;
            }
        }
    }
}
