using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Treevel.Common.Entities;
using Treevel.Common.Managers;
using UnityEngine.TestTools;

public class StageDataTest
{
    [UnityTest]
    public IEnumerator TestGoalBottlesEqualsGoalTiles() => UniTask.ToCoroutine(async () =>
    {
        await GameDataManager.InitializeAsync().ToCoroutine();

        GameDataManager.GetAllStages().ToList().ForEach(data => {
            var goalBottleNum = data.BottleDatas.Count(bottleData => bottleData.type == EBottleType.Normal);
            var goalTileNum = data.TileDatas.Count(tileData => tileData.type == ETileType.Goal);
            Assert.AreEqual(goalBottleNum, goalTileNum);
        });
    });
}
