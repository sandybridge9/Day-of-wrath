using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    [HideInInspector]
    public bool IsSelected { get; set; } = false;

    public virtual SelectableObjectType Type { get; }

    private SelectionIndicator selectionIndicator;

    private void Start()
    {
        selectionIndicator = GetComponent<SelectionIndicator>();
    }

    protected virtual void Update()
    {
        OnSelect();
    }

    public virtual void OnSelect()
    {
        if (IsSelected)
        {
            selectionIndicator.Show();
        }
        else
        {
            selectionIndicator.Hide();
        }
    }
}
