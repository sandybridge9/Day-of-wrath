using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    [HideInInspector]
    public bool IsSelected { get; set; } = false;

    [HideInInspector]
    public virtual SelectableObjectType Type { get;}
}
