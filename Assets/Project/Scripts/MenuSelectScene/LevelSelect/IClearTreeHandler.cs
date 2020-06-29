using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.MenuSelectScene.LevelSelect
{
    /// <summary>
    /// 木のクリア条件を決める
    /// </summary>
    public interface IClearTreeHandler
    {
        ETreeState IsClear(ETreeId treeId);
    }
}
