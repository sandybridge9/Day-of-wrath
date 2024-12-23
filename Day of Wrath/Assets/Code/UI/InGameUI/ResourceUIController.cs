using TMPro;
using UnityEngine;

public class ResourceUIController : MonoBehaviour
{
    public TMP_Text goldText;
    public TMP_Text woodText;
    public TMP_Text stoneText;
    public TMP_Text ironText;
    public TMP_Text foodText;

    public ResourceController resourceManager;

    private void Update()
    {
        goldText.text = $"Gold: {resourceManager.GetResourceAmount(ResourceType.Gold)}/{resourceManager.GetCapacity(ResourceType.Gold)}";
        woodText.text = $"Wood: {resourceManager.GetResourceAmount(ResourceType.Wood)}/{resourceManager.GetCapacity(ResourceType.Wood)}";
        stoneText.text = $"Stone: {resourceManager.GetResourceAmount(ResourceType.Stone)}/{resourceManager.GetCapacity(ResourceType.Stone)}";
        ironText.text = $"Iron: {resourceManager.GetResourceAmount(ResourceType.Iron)}/{resourceManager.GetCapacity(ResourceType.Iron)}";
        foodText.text = $"Food: {resourceManager.GetResourceAmount(ResourceType.Food)}/{resourceManager.GetCapacity(ResourceType.Food)}";
    }
}
