using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using UnityEngine.TestTools;

namespace Tests.EditModeTest
{
    public class StageDataTest
    {
        [UnityTest]
        public IEnumerator TestNumOfGoalBottlesEqualsGoalTiles()
        {
            var task = GameDataManager.InitializeAsync();

            while (task.Status == UniTaskStatus.Pending) yield return null;

            if (!task.Status.IsCompletedSuccessfully()) {
                Assert.Fail();
            }

            GameDataManager.GetAllStages().ToList().ForEach(data => {
                var goalBottleNum = data.BottleDatas.Count(bottleData => bottleData.type == EBottleType.Normal);
                var goalTileNum = data.TileDatas.Count(tileData => tileData.type == ETileType.Goal);
                Assert.AreEqual(goalBottleNum, goalTileNum, $"invalid stage data: [{data.StageId}]");
            });
        }

        [UnityTest]
        public IEnumerator TestStageDataNotDuplicate()
        {
            var task = GameDataManager.InitializeAsync();

            while (task.Status == UniTaskStatus.Pending) yield return null;

            if (!task.Status.IsCompletedSuccessfully()) {
                Assert.Fail();
            }

            var stages = GameDataManager.GetAllStages();
            var duplicates = stages.GroupBy(x => x.StageId)
                .Where(g => g.Count() > 1)
                .Select(y => new { StageId = y.Key })
                .ToList();

            var isDuplicated = duplicates.Any();
            Assert.That(isDuplicated, Is.False, duplicates.ToString);
        }
    }
}
