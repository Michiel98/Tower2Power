using UnityEngine;
using Photon.Pun;

public class PlayerProximity : MonoBehaviour
{
    PhotonView view;
    [SerializeField] GameObject manaEffect;
    

    void Update()
    {
        if(!view) view = GetComponent<PhotonView>();
    }

    // when player enters proximity of resource
    void OnTriggerEnter(Collider other)
    {
        if(!view) view = GetComponent<PhotonView>();
        if (!view.IsMine) return;
        if (!other.TryGetComponent(out ResourceScript resource))
        {
            Debug.Log("Triggered object did not have the necessary <ResourceScript> component attached.");
        }
        else
        {
            if (resource.Type == ResourceType.Mana) // triggered object was mana resource
            {
                // absorb mana object
                PlayerInventory.instance.AddResource(resource.Type, resource.Richness);
                PlayManaEffect(manaEffect, other.gameObject.transform.position);
                Destroy(other.gameObject);

                // spawn new mana object as replacement
                ResourceSpawner.instance.CreateManaObject();
            }

            else
            {
                MeshRenderer rend = other.GetComponent<MeshRenderer>();
                resource.SetInteractable(true); // allow interaction with resource
                rend.enabled = true; // make resource visible
                rend.renderingLayerMask = 1u << 4 - 1; // activate outline effect
            }
        }
    }

    // when player leaves proximity of resource
    void OnTriggerExit(Collider other)
    {
        if(!view) view = GetComponent<PhotonView>();
        if (!view.IsMine) return;
        if (!other.TryGetComponent(out ResourceScript resource))
        {
            Debug.Log("Triggered object did not have the necessary <ResourceScript> component attached.");
        }
        else
        {
            MeshRenderer rend = other.GetComponent<MeshRenderer>();
            resource.SetInteractable(false); // disallow interaction with resource
            rend.renderingLayerMask = 1u << 5 - 1; // deactivate outline effect
            // the resource remains visible once discovered!
        }
    }
    
    void PlayManaEffect(GameObject prefab, Vector3 position)
    {
        GameObject effect = Instantiate(prefab, position, Quaternion.identity);
        effect.gameObject.SetActive(true);
        effect.GetComponent<ParticleFollowPath>().StartAnimation(gameObject.transform.position);
    }
}