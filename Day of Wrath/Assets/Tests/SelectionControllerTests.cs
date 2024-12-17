using NUnit.Framework;
using UnityEngine;

public class UnitCommandControllerTests
{
    [Test]
    public void CalculateGridPositions_WithFourUnits_ReturnsCorrectPositions()
    {
        var gameObject = new GameObject();
        var unitCommandController = gameObject.AddComponent<UnitCommandController>();

        var positions = unitCommandController.CalculateGridPositions(Vector3.zero, 4);

        Assert.AreEqual(4, positions.Count);
        Assert.AreEqual(new Vector3(0, 0, 0), positions[0]);
        Assert.AreEqual(new Vector3(1.5f, 0, 0), positions[1]);
        Assert.AreEqual(new Vector3(0, 0, 1.5f), positions[2]);
        Assert.AreEqual(new Vector3(1.5f, 0, 1.5f), positions[3]);
    }

    [Test]
    public void CalculateGridPositions_WithNineUnits_ReturnsCorrectGridFormation()
    {
        var gameObject = new GameObject();
        var unitCommandController = gameObject.AddComponent<UnitCommandController>();

        var positions = unitCommandController.CalculateGridPositions(Vector3.zero, 9);

        Assert.AreEqual(9, positions.Count);
        Assert.AreEqual(new Vector3(0, 0, 0), positions[0]);
        Assert.AreEqual(new Vector3(1.5f, 0, 0), positions[1]);
        Assert.AreEqual(new Vector3(3f, 0, 0), positions[2]);
        Assert.AreEqual(new Vector3(0, 0, 1.5f), positions[3]);
        Assert.AreEqual(new Vector3(1.5f, 0, 1.5f), positions[4]);
        Assert.AreEqual(new Vector3(3f, 0, 1.5f), positions[5]);
        Assert.AreEqual(new Vector3(0, 0, 3f), positions[6]);
        Assert.AreEqual(new Vector3(1.5f, 0, 3f), positions[7]);
        Assert.AreEqual(new Vector3(3f, 0, 3f), positions[8]);
    }

    [Test]
    public void CalculateGridPositions_WithZeroUnits_ReturnsEmptyList()
    {
        var gameObject = new GameObject();
        var unitCommandController = gameObject.AddComponent<UnitCommandController>();

        var positions = unitCommandController.CalculateGridPositions(Vector3.zero, 0);

        Assert.AreEqual(0, positions.Count);
    }
}
