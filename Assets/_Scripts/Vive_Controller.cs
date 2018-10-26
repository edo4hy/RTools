using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Vive_Controller : MonoBehaviour {

    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId dashBack = Valve.VR.EVRButtonId.k_EButton_Dashboard_Back;

    private SteamVR_TrackedObject trackedObj;

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); }}

    HashSet<Interactable_Item> objectsHoveringOver = new HashSet<Interactable_Item>();

    private GameObject pickup;

    public Interactable_Item closestItem;
    public Interactable_Item interactingItem;

    private Material highlightMaterial;

    public bool isInteracting;


    // Use this for initialization
    void Start ()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

	// Update is called once per frame
	void Update ()
    {

      if(controller == null)
      {
        Debug.Log("Controller not initialized");
        return;
      }
        
        if(controller.GetPressDown(gripButton) && pickup != null)
        {
            float minDistance = float.MaxValue;
            float distance;
            // Find the gameObject which is closest to the controlelr
            foreach(Interactable_Item item in objectsHoveringOver)
            {

                distance = (item.transform.position - transform.position).sqrMagnitude;
                if(distance < minDistance)
                {
                    minDistance = distance;
                    closestItem = item;
                }
            }
          
            interactingItem = closestItem;
            if(interactingItem)
            {
                interactingItem.GetComponent<Rigidbody>().drag = 0f;
                if (interactingItem.IsInteracting())
                {
                    interactingItem.EndInteraction(this);
                    isInteracting = false;
                }

                interactingItem.BeginInteraction(this);
                isInteracting = true;
            }
        }

        // stop the interaction if grip button is released 
        if (controller.GetPressUp(gripButton) && interactingItem != null)
        {
            interactingItem.EndInteraction(this);
            isInteracting = false;
            interactingItem = null;
        }
    }

    /* ------------------------ Triggers ---------------------------*/

    // Adds gameObject which collide with the controller into the hashSet
    private void OnTriggerEnter(Collider collider)
    {
        // Add to hash table 
        Interactable_Item colliderItem = collider.GetComponent<Interactable_Item>();
        pickup = collider.gameObject;

        if (colliderItem)
        {
            objectsHoveringOver.Add(colliderItem);
        }

    }

    // Remove item from the hast set as it leaves the collider
    private void OnTriggerExit(Collider collider)
    {
        Interactable_Item colliderItem = collider.GetComponent<Interactable_Item>();
        pickup = null;

        if (colliderItem)
        {
            objectsHoveringOver.Remove(colliderItem);
        }

    }

    public Interactable_Item GetInteractingItem()
    {
        return interactingItem;
    }

    public GameObject GetInteractableObject()
    {
        return interactingItem.gameObject;
    }

}
