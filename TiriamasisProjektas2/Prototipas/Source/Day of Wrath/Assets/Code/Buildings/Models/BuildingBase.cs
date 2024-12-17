using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBase : SelectableObject
{
    public float Health;

    public override SelectableObjectType Type { get; } = SelectableObjectType.Building;

    public override void OnSelect()
    {
        // Display something in the interactive UI
        base.OnSelect();
    }
}
