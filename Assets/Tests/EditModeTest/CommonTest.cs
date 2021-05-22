using NUnit.Framework;
using Treevel.Common.Extensions;
using UnityEngine;

public class CommonTest
{
    [Test]
    public void FailTest()
    {
        Assert.Fail();
    }

    [Test]
    public void TestVector2Extension_Abs()
    {
        Assert.AreEqual(new Vector2(-1, -1).Abs(), new Vector2(1, 1));
        Assert.AreEqual(new Vector2(1, -1).Abs(), new Vector2(1, 1));
        Assert.AreEqual(new Vector2(-1, 1).Abs(), new Vector2(1, 1));
        Assert.AreEqual(new Vector2(1, 1).Abs(), new Vector2(1, 1));
    }

    [Test]
    public void TestVector2Extension_NormalizeDirection()
    {
        Assert.AreEqual(new Vector2(1, 2).NormalizeDirection(), Vector2.up);
        Assert.AreEqual(new Vector2(1, -2).NormalizeDirection(), Vector2.down);
        Assert.AreEqual(new Vector2(1, 0.5f).NormalizeDirection(), Vector2.right);
        Assert.AreEqual(new Vector2(-1, 0.5f).NormalizeDirection(), Vector2.left);
    }
}
