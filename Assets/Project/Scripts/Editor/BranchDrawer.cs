using Project.Scripts.StageSelectScene;
using UnityEditor;

namespace Project.Scripts.Editor
{
    /// <summary>
    /// BranchDrawerのエディタ拡張
    /// </summary>
    [CustomEditor(typeof(BranchController))]
    public class BranchDrawer : UnityEditor.Editor
    {
        private BranchController branch;

        public void OnEnable()
        {
            branch = (BranchController)target;
        }

        public override void OnInspectorGUI()
        {
            // スクリプトのメンバ変数の表示
            base.OnInspectorGUI();

            // 線の描画
            branch.SetPointPosition();
        }
    }
}
