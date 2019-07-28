using Project.Scripts.GamePlayScene.Panel;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.Utils.Library
{
    public static class PanelLibrary
    {
        /* パネルの番号を受け取り，NumberPanelオブジェクトを返す */
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
