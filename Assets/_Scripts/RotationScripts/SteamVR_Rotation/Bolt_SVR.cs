using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using NewtonVR;
//using VRTK;

public class Bolt_SVR : Part {

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

    public Bolt_SVR()
    {
        
    }

    // Use this for initialization
    void Start ()
    {
        if (offsetToolAttach == new Vector3(0, 0, 0))
        {
            offsetToolAttach = new Vector3(0f, 1.44155f, 0f);
        }


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
        if (this.gameObject.GetComponent<Interactable_Item>().GetInteractingControllerObject() != null)
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
