using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    [HideInInspector]
    public bool IsSelected { get; set; } = false;

    [HideInInspector]
    public virtual SelectableObjectType Type { get;}

    public virtual void Update()
    {
        OnSelect();
    }

    public virtual void OnSelect()
    {
        if(!IsSelected)
        {
            return;
        }
    }
}
