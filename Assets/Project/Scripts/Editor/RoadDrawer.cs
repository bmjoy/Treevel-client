using UnityEditor;
using Project.Scripts.MenuSelectScene.LevelSelect;

namespace Project.Scripts.Editor
{
    /// <summary>
    /// RoadDrawerのエディタ拡張
    /// </summary>
    [CustomEditor(typeof(RoadController))]
    public class RoadDrawer : UnityEditor.Editor
    {
        private RoadController road;

        public void OnEnable()
        {
            road = (RoadController)target;
        }

        public override void OnInspectorGUI()
        {
            // スクリプトのメンバ変数の表示
            base.OnInspectorGUI();

            // 線の描画
            road.SetPointPosition();
        }
    }
}
