using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.GameDatas
{
    [CreateAssetMenu(fileName = "stage.asset", menuName = "Stage")]
    public class StageData : ScriptableObject
    {
        [SerializeField] private int id;
        [SerializeField] private List<BulletGroupData> bulletGroups;
        [SerializeField] private List<PanelData> panels;
        [SerializeField] private List<TileData> tiles;

        public List<TileData> TileDatas
        {
            get {
                return tiles;
            }
        }

        public List<PanelData> PanelDatas
        {
            get {
                return panels;
            }
        }

        public List<BulletGroupData> BulletGroups
        {
            get {
                return bulletGroups;
            }
        }

        public int Id
        {
            get {
                return id;
            }
        }
    }
}
