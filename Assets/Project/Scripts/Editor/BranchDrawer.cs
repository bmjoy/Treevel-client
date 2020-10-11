using Treevel.Modules.StageSelectScene;
using UnityEditor;

namespace Project.Scripts.Editor
{
    /// <summary>
    /// BranchDrawerのエディタ拡張
    /// </summary>
    [CustomEditor(typeof(BranchController))]
    public class BranchDrawer : UnityEditor.Editor
    {
        private BranchController _branch;

        public void OnEnable()
        {
            _branch = (BranchController)target;
        }

        public override void OnInspectorGUI()
        {
            // スクリプトのメンバ変数の表示
            base.OnInspectorGUI();

            // 線の描画
            _branch.SetPointPosition();
        }
    }
}
