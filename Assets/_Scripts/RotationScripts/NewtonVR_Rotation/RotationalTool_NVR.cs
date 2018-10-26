using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewtonVR;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(NVRInteractableItem))]
[RequireComponent(typeof(Tool))]

public class RotationalTool_NVR : MonoBehaviour {

    public bool toolInRotationMode;
    public float boltIsBeingRemoved = 0;
    public float toolIsBeingRemovedTimer = 0;
    public GameObject snappedObject;
    public GameObject interactingControllerRM;
    public bool isBoltAttached = false;

    public Vector3 snapRotationOffset;
    public float rotateWithHandOffset = 0;

    public float ratchetClick = 10;
    public float boltBeingRemovedFactor = 0;
    public bool recordNewSnapPosition;

    public Quaternion startRotation;
    public float boltIsBeingRemovedTimer = 0;


    // Use this for initialization
    void Start()
    {
        // Set up newtonVR - 
        NVRInteractableItem nvrItem = this.GetComponent<NVRInteractableItem>();
        nvrItem.CanAttach = true;
        nvrItem.DisableKinematicOnAttach = true;
        nvrItem.EnableKinematicOnDetach = true;
        nvrItem.EnableGravityOnDetach = false;
        //nvrItem.DisablePhysicalMaterialsOnAttach = true;

    }

    // Update is called once per frame
    public void Update()
    {
        if (toolIsBeingRemovedTimer > 0)
        {
            toolIsBeingRemovedTimer -= Time.fixedDeltaTime;
        }

        if (boltIsBeingRemovedTimer > 0)
        {
            boltIsBeingRemovedTimer -= Time.fixedDeltaTime;
        }

        RotateToolWithHand();

        if (snappedObject == null)
        {
            return;
        }

        if (recordNewSnapPosition)
        {
            startRotation = this.gameObject.transform.rotation;
            recordNewSnapPosition = false;
        }

        ReleaseBolt();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<RotationHand_NVR>() != null)
        {
            HandEnterTool(col);
        }

        if (col.gameObject.GetComponent<Bolt_NVR>() != null)
        {
            if (boltIsBeingRemoved <= 0)
            {
                ToolSnapToBolt(col);
            }
        }
    }

   public void OnTriggerStay(Collider col)
    {
        if (col.gameObject.GetComponent<RotationHand_NVR>() != null)
        {
            HandEnterTool(col);
        }
        if (col.gameObject.GetComponent<Bolt_NVR>() != null)
        {
            if (boltIsBeingRemoved <= 0)
            {
                ToolSnapToBolt(col);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        Debug.Log(col.gameObject.GetComponent<RotationHand_NVR>());
        Debug.Log(col);

        if (col.gameObject.GetComponentInParent<RotationHand_NVR>() != null)
        {
            Debug.Log("0");
            RemoveToolSnapToHand(col);
        }
    }


    public void HandEnterTool(Collider col)
    {
        if (col.gameObject.GetComponent<NVRHand>().CurrentlyInteracting != null)
        {
            if (col.gameObject.GetComponent<NVRHand>().CurrentlyInteracting != this.gameObject.GetComponent<NVRInteractableItem>())
            {
                return;
            }
        }
        if (col.gameObject.GetComponent<RotationHand_NVR>().currentRotationTool != null)
        {
            if (col.gameObject.GetComponent<RotationHand_NVR>().currentRotationTool != this.gameObject)
            {
                return;
            }
        }

        if (toolInRotationMode)
        {
            if (col.gameObject.GetComponent<SteamVR_TrackedController>().gripped)
            {
                interactingControllerRM = col.gameObject;

                col.gameObject.GetComponent<NVRHand>().enabled = false;
                col.gameObject.GetComponent<RotationHand_NVR>().currentRotationTool = this.gameObject;
                this.gameObject.GetComponent<NVRInteractableItem>().enabled = false;

                this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                this.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
        else
        {
            // pick up the tool as normal using NVRHand
        }
    }

    public delegate void ToolSnappedToBoltEvent(RatchetRotation_NVR e, GameObject snappedObject);
    public static event ToolSnappedToBoltEvent OnToolSnappedToBoltEvent;

    /*
     * snapToBolt - 
     * Tool colliding with bolt snaps to bolts position with offset.
     * NVR Hand and NVR interactable item functionality is disbaled so only RatchetRotation affects tool.
     * Set bolt's parent to the tool
     * Swich Colliders to sphere collider 
     * */
    public void ToolSnapToBolt(Collider col)
    {
        if (CheckToolAndBoltStateForSnap(col))
        {
            if (col.gameObject.GetComponent<Bolt_NVR>().partSize == this.gameObject.GetComponent<Tool>().toolSize)
            {
                col.gameObject.GetComponent<Bolt_NVR>().snappedTool = this.gameObject;
                toolInRotationMode = true;
                snappedObject = col.gameObject;

                if (OnToolSnappedToBoltEvent != null)
                {
                    Debug.Log("we are oion ");
                    //OnToolSnappedToBoltEvent(this, snappedObject);
                }

                NVRInteractableItem interactableItem = this.gameObject.GetComponent<NVRInteractableItem>();
                interactingControllerRM = interactableItem.AttachedHand.gameObject;

                // turn off the NVRHand on this controller
                // turn off the hand on this controller
                NVRHand hand = interactableItem.AttachedHand.GetComponent<NVRHand>();
                hand.EndInteraction(interactableItem);
                hand.enabled = false;
                interactableItem.EndInteraction(hand);
                interactableItem.enabled = false;

                interactingControllerRM.gameObject.GetComponent<RotationHand_NVR>().currentRotationTool = this.gameObject;

                //Switch Colliders
                this.gameObject.GetComponent<SphereCollider>().enabled = true;
                this.gameObject.GetComponent<CapsuleCollider>().enabled = false;

                //Set the position of the tool to the bolt
                this.gameObject.transform.position
                = col.transform.position + col.gameObject.GetComponent<Bolt_NVR>().offsetToolAttach;

                Quaternion Q = new Quaternion();
                Q.eulerAngles = new Vector3(0f, this.transform.rotation.eulerAngles.y, 0f) + snapRotationOffset;
                this.gameObject.transform.rotation = Q;

                this.GetComponent<Rigidbody>().velocity = Vector3.zero;
                this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                Debug.Log("Finsihsed snapping");
            }
        }               
    }

    bool CheckToolAndBoltStateForSnap(Collider col)
    {
        if (snappedObject != null) return false;

        if (toolInRotationMode) return false;

        if (isBoltAttached) return false;

        if (col.gameObject.GetComponent<Part>().hasBeenRemoved) return false;

        if (toolIsBeingRemovedTimer > 0) return false;

        if (col.gameObject.GetComponent<Bolt_NVR>().snappedTool != null) return false;

        if (this.gameObject.GetComponent<NVRInteractableItem>().AttachedHand == null) return false;

        return true;
    }


    /*
     * Look at function follows the attached hand and the y rotation is taken to rotate the ratchet and attached gameObject
     * With offset passed in.
     */
    public void RotateToolWithHand()
    {
        if (CheckRotationStatus())
        { 
           
            this.gameObject.transform.GetChild(1).gameObject.transform.LookAt(interactingControllerRM.gameObject.transform);

            // Rotate the tool - Inner object Looks at controller - only the y from the rotation is passed to the tool
            Quaternion Q = new Quaternion();
            Q.eulerAngles = new Vector3(this.gameObject.transform.rotation.eulerAngles.x,
                this.gameObject.transform.GetChild(1).gameObject.transform.rotation.eulerAngles.y + rotateWithHandOffset,
                this.gameObject.transform.rotation.eulerAngles.z);

            this.gameObject.transform.rotation = Q;
        }
    }

    /* 
     * Boolean check required for hand, tool and object to be in the correct state for rotation
     */
    public bool CheckRotationStatus()
    {
        if (!toolInRotationMode) return false;

        if (snappedObject == null) return false;

        if (interactingControllerRM == null) return false;

        if (!interactingControllerRM.gameObject.GetComponent<SteamVR_TrackedController>().gripped) return false;

        if (interactingControllerRM.GetComponent<RotationHand_NVR>().currentRotationTool != this.gameObject) return false;

        return true;
    }

    /*
     * Remove tool and snap back to hand, with or without bolt 
     * If bolt has been fully removed then bolt will remain parented to tool - if not set back to normal parent
     * NVRHand turned back on and interaction initiated with tool NVRInteractableItem
     * Switch Collider back to capsule collider 
     */
    public void RemoveToolSnapToHand(Collider col)
    {
        if (CheckHandAndToolOnRemoveTool(col))
        {
            if (col.gameObject.GetComponentInParent<SteamVR_TrackedController>().gripped)
            {
                Debug.Log("6");
                // Then effect tool snap back to hand 
                if (snappedObject.gameObject.GetComponent<Bolt_NVR>().removedAmount >= snappedObject.gameObject.GetComponent<Bolt_NVR>().degreeToRemove)
                {
                    Debug.Log("7");
                    if (!snappedObject.GetComponent<Bolt_NVR>().hasBeenRemoved)
                    {
                        Debug.Log("8.1");
                        col.gameObject.GetComponentInParent<RotationHand_NVR>().currentRotationTool = null;
                        //bolt has been removed from main part 
                        snappedObject.transform.SetParent(this.transform, true);
                        isBoltAttached = true;
                        toolInRotationMode = false;
                        snappedObject.gameObject.GetComponent<Bolt_NVR>().hasBeenRemoved = true;

                        toolIsBeingRemovedTimer = 0.5f;

                        this.transform.rotation = col.gameObject.transform.rotation;
                        this.transform.position = col.gameObject.transform.position;

                        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;


                        this.gameObject.GetComponent<NVRInteractableItem>().enabled = true;
                        col.gameObject.GetComponentInParent<NVRHand>().enabled = true;

                        //col.gameObject.GetComponent<NVRHand>().BeginInteraction(this.gameObject.GetComponent<NVRInteractableItem>());
                        this.gameObject.GetComponent<NVRInteractableItem>().BeginInteraction(col.gameObject.GetComponent<NVRHand>());

                        this.GetComponent<CapsuleCollider>().enabled = true;
                        this.GetComponent<SphereCollider>().enabled = false;
                    }
                }
                else
                {
                    Debug.Log("8.2");
                    col.gameObject.GetComponentInParent<RotationHand_NVR>().currentRotationTool = null;
                    snappedObject.GetComponent<Bolt_NVR>().snappedTool = null;

                    snappedObject.transform.SetParent(snappedObject.GetComponent<Bolt_NVR>().standardParent.transform, true);
                    snappedObject = null;

                    interactingControllerRM = null;

                    toolInRotationMode = false;
                    recordNewSnapPosition = true;

                    isBoltAttached = false;

                    toolIsBeingRemovedTimer = 0.5f;

                    this.transform.rotation = col.gameObject.transform.rotation;
                    this.transform.position = col.gameObject.transform.position;

                    this.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                    this.gameObject.GetComponent<NVRInteractableItem>().enabled = true;
                    col.gameObject.GetComponentInParent<NVRHand>().enabled = true;

                    //this.gameObject.GetComponent<NVRInteractableItem>().BeginInteraction(col.gameObject.GetComponentInParent<NVRHand>());
                    col.gameObject.GetComponentInParent<NVRHand>().BeginInteraction(this.gameObject.GetComponent<NVRInteractableItem>());

                    this.GetComponent<CapsuleCollider>().enabled = true;
                    this.GetComponent<SphereCollider>().enabled = false;
                }
            }
            else
            {
                interactingControllerRM = null;
                col.gameObject.GetComponentInParent<NVRHand>().enabled = true;
                col.gameObject.GetComponentInParent<NVRHand>().CurrentlyInteracting = null;
                col.gameObject.GetComponentInParent<RotationHand_NVR>().currentRotationTool = null;

                Debug.Log("enable hand");
            }
        }
    }

    /*
     * Checks for Hand and Tool to make sure they are in the correct state for removing the tool 
     */
    bool CheckHandAndToolOnRemoveTool(Collider col)
    {
        if (col.gameObject.GetComponentInParent<NVRHand>().enabled == true) return false;
        Debug.Log("2");


        if (snappedObject == null) return false;
        Debug.Log("3");


        if (col.gameObject.GetComponentInParent<RotationHand_NVR>().gameObject != interactingControllerRM) return false;
        Debug.Log("4");


        if (col.gameObject.GetComponentInParent<RotationHand_NVR>().currentRotationTool != this.gameObject) return false;
        Debug.Log("5");


        return true;
    }


    public delegate void BoltReleasedEvent();
    public static event BoltReleasedEvent OnBoltReleasedEvent;
    /*
     * Press Trigger button to release bolt- 
     * Turn mesh on and off to solve freeze bug 
     * Reset both bolt and tool to allow future interactions 
     * Call on release bolt Event - Listen to in ActionController for the progression to next stage
     * */
    public void ReleaseBolt()
    {
        if (interactingControllerRM == null)
            return;

        if (interactingControllerRM.GetComponent<SteamVR_TrackedController>().triggerPressed)
        {
            if (this.snappedObject != null)
            {
                snappedObject.transform.SetParent(snappedObject.GetComponent<Bolt_NVR>().standardParent.transform, true);
                snappedObject.GetComponent<MeshCollider>().enabled = false;
                snappedObject.GetComponent<MeshCollider>().enabled = true;
                snappedObject.GetComponent<NVRInteractableItem>().enabled = true;
                snappedObject.GetComponent<NVRInteractableItem>().ResetInteractable();

                snappedObject.GetComponent<Bolt_NVR>().snappedTool = null;
                snappedObject.GetComponent<Bolt_NVR>().hasBeenRemoved = true;

                OnBoltReleasedEvent();

                isBoltAttached = false;
                recordNewSnapPosition = true;
                interactingControllerRM = null;
                snappedObject = null;

                boltIsBeingRemovedTimer = 1;
            }
        }
    }

    /*
 * Calculates the angle of rotation from, to 
 */
    public Vector3 EulerAnglesBetween(Quaternion from, Quaternion to)
    {
        Vector3 delta = to.eulerAngles - from.eulerAngles;

        if (delta.x > 180)
            delta.x -= 360;
        else if (delta.x < -180)
            delta.x += 360;

        if (delta.y > 180)
            delta.y -= 360;
        else if (delta.y < -180)
            delta.y += 360;

        if (delta.z > 180)
            delta.z -= 360;
        else if (delta.z < -180)
            delta.z += 360;

        return delta;
    }

    public void DisableNVRInteractable(GameObject partObject)
    {
        if (partObject.GetComponent<NVRInteractableItem>().enabled == true)
        {
            partObject.GetComponent<NVRInteractableItem>().enabled = false;
        }
    }
}
