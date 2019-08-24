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
            GameObject[] numberPanels = GameObject.FindGameObjectsWithTag(TagName.NUMBER_PANEL);

            foreach (var numberPanel in numberPanels) {
                var script = numberPanel.GetComponent<NumberPanelController>();

                if (script.GetNumberPanel(panelNum) != null) {
                    return numberPanel;
                }
            }

            return null;
        }
    }
}
