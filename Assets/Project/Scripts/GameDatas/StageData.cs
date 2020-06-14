using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.Video;

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

        [SerializeField] private List<int> constraintStageNumbers;

        public ETreeId TreeId => treeId;

        public int StageNumber => stageNumber;

        public List<int> ConstraintStageNumbers => constraintStageNumbers;

        public List<TileData> TileDatas => tiles;

        public List<BottleData> BottleDatas => bottles;

        public List<BulletGroupData> BulletGroups => bulletGroups;

        public List<EBulletType> OverviewGimmicks => overviewGimmicks;

        public TutorialData Tutorial => tutorial;

        public bool IsUnLocked()
        {
            // ステージ制限なし
            if (ConstraintStageNumbers.Count == 0)
                return true;

            var constraintStagesStatus = ConstraintStageNumbers
                .Where((stageNumber) => GameDataBase.GetStage(treeId, stageNumber) != null) // 存在しないステージを弾く
                .Select((stageNumber) => StageStatus.Get(treeId, stageNumber));

            return constraintStagesStatus.All(s => s.passed);
        }
    }
}
