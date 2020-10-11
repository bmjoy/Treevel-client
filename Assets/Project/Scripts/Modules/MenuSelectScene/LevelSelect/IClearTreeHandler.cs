using Treevel.Common.Entities;

namespace Treevel.Modules.MenuSelectScene.LevelSelect
{
    /// <summary>
    /// 木のクリア条件を決める
    /// </summary>
    public interface IClearTreeHandler
    {
        ETreeState GetTreeState();
    }
}
