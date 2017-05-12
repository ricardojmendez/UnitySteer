using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using UnitySteer2D.Behaviors;

public class MyTests {

	[Test]
    public void SteerForTether2D_MaximumDistanceTest()
    {
		GameObject gameObject = new GameObject();
        SteerForTether2D sft = gameObject.AddComponent<SteerForTether2D>();

        Assert.GreaterOrEqual(sft.MaximumDistance, 0);
	}

    [Test]
    public void DetectableObject2D_RadiusTest()
    {
        GameObject gameObject = new GameObject();
        DetectableObject2D obj = gameObject.AddComponent<DetectableObject2D>();

        Assert.GreaterOrEqual(obj.Radius, 0.1);
    }

    [Test]
    public void DetectableObject2D_CenterTest()
    {
        GameObject gameObject = new GameObject();
        DetectableObject2D obj = gameObject.AddComponent<DetectableObject2D>();
        Vector2 pos = (Vector2)obj.Transform.position + obj.Center;

        Assert.LessOrEqual((obj.Position - pos).magnitude, Mathf.Epsilon);
    }

    [Test]
    public void DetectableObject2D_CenterChangeTest([NUnit.Framework.Range(-5, 10, 3)] int x)
    {
        GameObject gameObject = new GameObject();
        DetectableObject2D obj = gameObject.AddComponent<DetectableObject2D>();
        obj.Center = Vector2.right * x;
        Vector2 pos = (Vector2)obj.Transform.position + Vector2.right * x;

        Assert.LessOrEqual((obj.Position - pos).magnitude, Mathf.Epsilon);
    }
}
