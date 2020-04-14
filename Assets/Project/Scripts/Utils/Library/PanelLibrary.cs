using System.Linq;
using Project.Scripts.GamePlayScene.Panel;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.Utils.Library
{
    public static class PanelLibrary
    {
        /// <summary>
        /// パネルの番号から，Number Panel を返す
        /// </summary>
        /// <param name="panelNum"> パネルの番号 </param>
        /// <returns> Number Panel オブジェクト </returns>
        public static GameObject GetPanel(int panelNum)
        {
            var numberPanels = GameObject.FindGameObjectsWithTag(TagName.NUMBER_PANEL);
            // パネルの番号がpanelNumの唯一のパネルを探す、二個以上もしくは0個の場合は InvalidOperationExceptionがスローされる
            return numberPanels.Single(panel => panel.GetComponent<NumberPanelController>()?.Id == panelNum);
        }
    }
}
