using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    [HideInInspector]
    public bool IsSelected { get; set; } = false;

    public void Update()
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
