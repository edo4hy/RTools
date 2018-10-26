using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewtonVR;

[RequireComponent(typeof(NVRInteractableItem))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(Rigidbody))]

public class Bolt_NVR : Part {

    public BoltSize partSize;
    public ToolType takesToolOfType;

    [HideInInspector]
    public float removedAmount;
    public int numRotationRemove;
    public float distanceToBeRemoved;
    [HideInInspector]
    public float degreeToRemove;
    public Vector3 offsetToolAttach;
    [HideInInspector]
    public GameObject snappedTool;



    public Bolt_NVR()
    {
        
    }

    private void Awake()
    {
        MeshCollider col = this.gameObject.GetComponent<MeshCollider>();
        col.convex = true;
        col.isTrigger = true;

        Rigidbody rig = this.gameObject.GetComponent<Rigidbody>();
        rig.isKinematic = true;
        rig.useGravity = false;
        
    }

    // Use this for initialization
    void Start ()
    {
 
        degreeToRemove = numRotationRemove * 360f;
    }
	
	// Update is called once per frame

    void Update()
    {
        PartRemovedByToolThenPickUpByHand();
    }

    /*
     * when bolt has been removed and is picked up whilst still attached to the tool, 
     * Reset the bolt and reset the tool - 
     * Weird bug where the part becomes uninteractable on release, turning on and off meshCollider solves this. 
     */
    public void PartRemovedByToolThenPickUpByHand()
    {
        if (this.gameObject.GetComponent<NVRInteractableItem>().AttachedHand)
        {
            if (snappedTool)
            {
                // Reset collider after change of parent, bug made the part uninteractable 
                this.transform.SetParent(this.GetComponent<Part>().standardParent.transform, true);
                this.GetComponent<MeshCollider>().enabled = false;
                this.GetComponent<MeshCollider>().enabled = true;

                //Reset the tool
                RotationalTool_NVR tool = snappedTool.GetComponent<RotationalTool_NVR>();

                Debug.Log(tool);
                tool.isBoltAttached = false;
                tool.recordNewSnapPosition = true;
                tool.interactingControllerRM = null;
                tool.snappedObject = null;
                tool.boltIsBeingRemovedTimer = 1;

                snappedTool = null;
                hasBeenRemoved = true;
            }
        }
    }
}
