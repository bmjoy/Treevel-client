using Project.Scripts.Utils.Definitions;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.GameDatas
{
    [CreateAssetMenu(fileName = "stage.asset", menuName = "Stage")]
    public class StageData : ScriptableObject
    {
        [SerializeField] private ETreeId treeId;
        [SerializeField] private int stageNumber;
        [SerializeField] private List<TileData> tiles;
        [SerializeField] private List<BottleData> bottles;
        [SerializeField] private List<BulletGroupData> bulletGroups;
        [SerializeField] private List<EBulletType> overviewGimmicks;
        [SerializeField] private TutorialData tutorial;

        public ETreeId TreeId => treeId;

        public int StageNumber => stageNumber;

        public List<TileData> TileDatas => tiles;

        public List<BottleData> BottleDatas => bottles;

        public List<BulletGroupData> BulletGroups => bulletGroups;

        public List<EBulletType> OverviewGimmicks => overviewGimmicks;

        public TutorialData Tutorial => tutorial;
    }
}
