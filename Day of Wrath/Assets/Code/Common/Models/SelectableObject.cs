using UnityEngine;

[System.Serializable]
public class SelectableObject : MonoBehaviour
{
    [HideInInspector]
    public bool IsSelected { get; set; } = false;

    public virtual SelectableObjectType Type { get; }

    public virtual Team Team { get; set; }

    private SelectionIndicator selectionIndicator;

    protected virtual void Start()
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

    public bool IsFromDifferentTeam(Team team)
    {
        return Team != team;
    }
}
