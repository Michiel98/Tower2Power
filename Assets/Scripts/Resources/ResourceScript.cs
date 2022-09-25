using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ResourceScript : MonoBehaviour
{
    // type of resource
    ResourceType type;
    public ResourceType Type { get { return type; } }
    public void SetType(ResourceType type) => this.type = type;

    // whether or not resource is close enough to be interacted with
    bool interactable;
    public bool IsInteractable { get { return interactable; } }
    public void SetInteractable(bool interactable) => this.interactable = interactable;

    // the amount of units this resource holds, its richness
    int richness;
    public int Richness { get { return richness; } }
    public void SetRichness(int richness) => this.richness = richness;
    public void IncrementRichness() => richness++;
    public void DecrementRichness() => richness--;
}