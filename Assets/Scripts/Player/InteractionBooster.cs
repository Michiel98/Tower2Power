using UnityEngine;

public class InteractionBooster : MonoBehaviour
{
    [Header("Interaction")]
    [Range(0.1f, 100f)] [SerializeField] float defaultInteractionRadius = 10f;
    [SerializeField] [Range(0f, 5f)] float rangeReductionRate = 1f;
    
    [Header("Other")]
    [SerializeField] Transform pulse;
    [SerializeField] Color pulseColor;
    
    [Header("Boosting")]
    [SerializeField] GameObject boostButton;
    [SerializeField] [Range(0, 200)] int boostCost = 10;
    [SerializeField] [Range(1f, 2f)] float boostAmount = 1.1f;
    
    SphereCollider playerCollider;
    SpriteRenderer pulseRenderer;
    
    float interactionRadius;
    float pulseRadius;
    

    void Start()
    {
        
        pulseRenderer = pulse.GetComponent<SpriteRenderer>();
        boostButton.SetActive(false); // disable boost button initially
        
        pulseRadius = 1f;
        interactionRadius = defaultInteractionRadius;
       
    }

    void Update()
    {
        if(!playerCollider) playerCollider = transform.parent.GetComponent<SphereCollider>();
        if(!pulseRenderer) pulseRenderer = pulse.GetComponent<SpriteRenderer>();
        if(PlayerInventory.instance.ManaAmount >= boostCost && !boostButton.activeSelf) boostButton.SetActive(true); // enable boost button
        else if(PlayerInventory.instance.ManaAmount < boostCost && boostButton.activeSelf) boostButton.SetActive(false); // disable boost button
        
        pulseRadius += interactionRadius / 2f * Time.deltaTime;

        if (pulseRadius > interactionRadius) 
        {
            pulseRadius = 1f; // reset pulse
            interactionRadius -= rangeReductionRate; // reduce interaction range with each pulse
            if (interactionRadius < defaultInteractionRadius)interactionRadius = defaultInteractionRadius; // don't degrade further than default range
        }
        
        playerCollider.radius = interactionRadius;

        pulse.localScale = new Vector3(pulseRadius * 2, pulseRadius * 2);
        pulseRenderer.color = new Color(pulseColor.r, pulseColor.g, pulseColor.b,
            Mathf.Lerp(0f, 1f, 1 - pulseRadius / interactionRadius));
    }

    public void BoostInteractionRange()
    {
        if (PlayerInventory.instance.ManaAmount < boostCost) return; // not enough mana for boost
        BoostInteractionRange(boostAmount);
        PlayerInventory.instance.RemoveResource(ResourceType.Mana, boostCost); // remove mana from player
    }
    
    void BoostInteractionRange(float amount)
    {
        interactionRadius *= amount;
        playerCollider.radius *= amount;
    }
}