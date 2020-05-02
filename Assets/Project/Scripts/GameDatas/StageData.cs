using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GameDatas
{
    [CreateAssetMenu(fileName = "stage.asset", menuName = "Stage")]
    public class StageData : ScriptableObject
    {
        [SerializeField] private int id;
        [SerializeField] private List<TileData> tiles;
        [SerializeField] private List<BottleData> bottles;
        [SerializeField] private List<BulletGroupData> bulletGroups;
        [SerializeField] private List<EBulletType> overviewGimmicks;

        public int Id => id;

        public List<TileData> TileDatas => tiles;

        public List<BottleData> BottleDatas => bottles;

        public List<BulletGroupData> BulletGroups => bulletGroups;

        public List<EBulletType> OverviewGimmicks => overviewGimmicks;
    }
}
