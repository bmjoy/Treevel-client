using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using UnityEngine.TestTools;

namespace Tests.EditModeTest
{
    public class StageDataTest
    {
        private List<StageData> _stageDataList;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            if (_stageDataList != null) {
                yield break;
            }

            var task = GameDataManager.InitializeAsync();

            while (task.Status == UniTaskStatus.Pending) yield return null;

            if (!task.Status.IsCompletedSuccessfully()) {
                Assert.Fail();
            }

            _stageDataList = GameDataManager.GetAllStages().ToList();
        }

        [Test]
        public void TestNumOfGoalBottlesEqualsGoalTiles()
        {
            _stageDataList.ForEach(data => {
                var goalBottleNum = data.BottleDatas.Count(bottleData => bottleData.type == EBottleType.Normal);
                var goalTileNum = data.TileDatas.Count(tileData => tileData.type == ETileType.Goal);
                Assert.AreEqual(goalBottleNum, goalTileNum, $"invalid stage data: [{data.StageId}]");
            });
        }

        [Test]
        public void TestStageDataNotDuplicate()
        {
            var duplicates = _stageDataList.GroupBy(x => x.StageId)
                .Where(g => g.Count() > 1)
                .Select(y => new { StageId = y.Key })
                .ToList();

            var isDuplicated = duplicates.Any();
            Assert.That(isDuplicated, Is.False, duplicates.ToString);
        }
    }
}
