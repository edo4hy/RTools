using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using NewtonVR;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Interactable_Item))]
[RequireComponent(typeof(Tool))]

public class RotationalTool_SVR : MonoBehaviour {

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

        //ReleaseBolt();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Hand"))
        {
           
            HandEnterTool(col);
        }

        if (col.gameObject.CompareTag("Bolt"))
        {
            if (boltIsBeingRemoved <= 0)
            {
                ToolSnapToBolt(col);
            }
        }
    }

   public void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("Hand"))
        {
            HandEnterTool(col);
        }
        if (col.gameObject.CompareTag("Bolt"))
        {
            if (boltIsBeingRemoved <= 0)
            {
                ToolSnapToBolt(col);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Hand"))
        {
            RemoveToolSnapToHand(col);
        }
    }


    public void HandEnterTool(Collider col)
    {
        Debug.Log("hand enter tool");
        if(col.gameObject.GetComponent<Vive_Controller>().GetInteractableObject() != null)
        {
            if (col.gameObject.GetComponent<Vive_Controller>().GetInteractableObject() != this.gameObject)
            {
                return;
            }
        }


        if (col.gameObject.GetComponent<HandRotationStatus>().currentRotationTool != null)
        {
            if (col.gameObject.GetComponent<HandRotationStatus>().currentRotationTool != this.gameObject)
            {
                return;
            }
        }

        if (toolInRotationMode)
        {
            if (col.gameObject.GetComponent<SteamVR_TrackedController>().gripped)
            {
                interactingControllerRM = col.gameObject;

                col.gameObject.GetComponent<Vive_Controller>().enabled = false;

                
                col.gameObject.GetComponent<HandRotationStatus>().currentRotationTool = this.gameObject;
                this.gameObject.GetComponent<Interactable_Item>().enabled = false;

                this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                this.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
        else
        {
            // pick up the tool as normal using NVRHand / vrtk
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
            if (col.gameObject.GetComponent<Bolt_SVR>().partSize == this.gameObject.GetComponent<Tool>().toolSize)
            {
                col.gameObject.GetComponent<Bolt_SVR>().snappedTool = this.gameObject;
                toolInRotationMode = true;
                snappedObject = col.gameObject;

                if (OnToolSnappedToBoltEvent != null)
                {
                    Debug.Log("we are oion ");
                    //OnToolSnappedToBoltEvent(this, snappedObject);
                }

                interactingControllerRM = this.gameObject.GetComponent<Interactable_Item>().GetInteractingControllerObject();
                // turn off the NVRHand on this controller
                // turn off the hand on this controller
                //NVRHand hand = interactableItem.AttachedHand.GetComponent<NVRHand>();
                //hand.EndInteraction(interactableItem);
                //hand.enabled = false;
                //interactableItem.EndInteraction();
                //interactableItem.enabled = false;


                this.gameObject.GetComponent<Interactable_Item>().GetInteractingController().enabled = false;
                this.gameObject.GetComponent<Interactable_Item>().enabled = false;

                // Set hand status on controller
                interactingControllerRM.gameObject.GetComponent<HandRotationStatus>().currentRotationTool = this.gameObject;

                //Switch Colliders
                this.gameObject.GetComponent<SphereCollider>().enabled = true;
                this.gameObject.GetComponent<CapsuleCollider>().enabled = false;

                //Set the position of the tool to the bolt
                this.gameObject.transform.position
                = col.transform.position + col.gameObject.GetComponent<Bolt_SVR>().offsetToolAttach;

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

        if (col.gameObject.GetComponent<Bolt_SVR>().snappedTool != null) return false;

        if (!this.gameObject.GetComponent<Interactable_Item>().IsInteracting()) return false;

        return true;
    }


    /*
     * Look at function follows the attached hand and the y rotation is taken to rotate the ratchet and attached gameObject
     * With offset passed in.
     */
    public void RotateToolWithHand()
    {
        if (!interactingControllerRM) return;
       
        if (toolInRotationMode && snappedObject != null
            && snappedObject != null
                && interactingControllerRM != null
                    && interactingControllerRM.gameObject.GetComponent<SteamVR_TrackedController>().gripped
                        && interactingControllerRM.gameObject.GetComponent<HandRotationStatus>()
                            && interactingControllerRM.GetComponent<HandRotationStatus>().currentRotationTool == this.gameObject)
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
     * Remove tool and snap back to hand, with or without bolt 
     * If bolt has been fully removed then bolt will remain parented to tool - if not set back to normal parent
     * NVRHand turned back on and interaction initiated with tool NVRInteractableItem
     * Switch Collider back to capsule collider 
     */
    public void RemoveToolSnapToHand(Collider col)
    {
        if (CheckHandAndToolOnRemoveTool(col))
        {
            if (col.gameObject.GetComponent<SteamVR_TrackedController>().gripped)
            {
                // Then effect tool snap back to hand 
                if (snappedObject.gameObject.GetComponent<Bolt_SVR>().removedAmount >= snappedObject.gameObject.GetComponent<Bolt_SVR>().degreeToRemove)
                {
                    if (!snappedObject.GetComponent<Bolt_SVR>().hasBeenRemoved)
                    {
                        col.gameObject.GetComponentInChildren<HandRotationStatus>().currentRotationTool = null;
                        //bolt has been removed from main part 
                        snappedObject.transform.SetParent(this.transform, true);
                        isBoltAttached = true;
                        toolInRotationMode = false;
                        snappedObject.gameObject.GetComponent<Bolt_SVR>().hasBeenRemoved = true;

                        toolIsBeingRemovedTimer = 0.5f;

                        this.transform.rotation = col.gameObject.transform.rotation;
                        this.transform.position = col.gameObject.transform.position;

                        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;


                        //this.gameObject.GetComponent<NVRInteractableItem>().enabled = true;
                        //col.gameObject.GetComponent<NVRHand>().enabled = true;

                        this.gameObject.GetComponent<Interactable_Item>().enabled = true;
                        col.gameObject.GetComponent<Vive_Controller>().enabled = true;

                        this.gameObject.GetComponent<Interactable_Item>().BeginInteraction(col.gameObject.GetComponent<Vive_Controller>());

                        //this.gameObject.GetComponent<VRTK_InteractableObject>().enabled = true;
                        //col.gameObject.GetComponentInChildren<VRTK_ControllerEvents>().enabled = true;
                        //col.gameObject.GetComponentInChildren<VRTK_InteractTouch>().enabled = true;
                        //col.gameObject.GetComponentInChildren<VRTK_InteractGrab>().enabled = true;


                        ////col.gameObject.GetComponent<NVRHand>().BeginInteraction(this.gameObject.GetComponent<NVRInteractableItem>());
                        ////this.gameObject.GetComponent<NVRInteractableItem>().BeginInteraction(col.gameObject.GetComponent<NVRHand>());
                        //col.gameObject.GetComponent<VRTK_InteractGrab>().AttemptGrab();


                        this.GetComponent<CapsuleCollider>().enabled = true;
                        this.GetComponent<SphereCollider>().enabled = false;
                    }
                }
                else
                {
                    col.gameObject.GetComponent<HandRotationStatus>().currentRotationTool = null;
                    snappedObject.GetComponent<Bolt_SVR>().snappedTool = null;

                    snappedObject.transform.SetParent(snappedObject.GetComponent<Bolt_SVR>().standardParent.transform, true);
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

                    ////this.gameObject.GetComponent<NVRInteractableItem>().enabled = true;
                    ////col.gameObject.GetComponent<NVRHand>().enabled = true;
                    //this.gameObject.GetComponent<VRTK_InteractableObject>().enabled = true;
                    //this.gameObject.GetComponent<VRTK_InteractableObject>().isGrabbable = true;
                    //col.gameObject.GetComponent<VRTK_ControllerEvents>().enabled = true;
                    //col.gameObject.GetComponent<VRTK_InteractTouch>().enabled = true;
                    //col.gameObject.GetComponent<VRTK_InteractGrab>().enabled = true;

                    ////this.gameObject.GetComponent<NVRInteractableItem>().BeginInteraction(col.gameObject.GetComponent<NVRHand>());
                    ////col.gameObject.GetComponent<NVRHand>().BeginInteraction(this.gameObject.GetComponent<NVRInteractableItem>());
                    //col.gameObject.GetComponent<VRTK_InteractGrab>().AttemptGrab();


                    this.gameObject.GetComponent<Interactable_Item>().enabled = true;
                    col.gameObject.GetComponent<Vive_Controller>().enabled = true;

                    this.gameObject.GetComponent<Interactable_Item>().BeginInteraction(col.gameObject.GetComponent<Vive_Controller>());

                    this.GetComponent<CapsuleCollider>().enabled = true;
                    this.GetComponent<SphereCollider>().enabled = false;
                }
            }
            else
            {
                interactingControllerRM = null;
                //col.gameObject.GetComponent<NVRHand>().enabled = true;
                //col.gameObject.GetComponent<NVRHand>().CurrentlyInteracting = null;
                //col.gameObject.GetComponent<VRTK_ControllerEvents>().enabled = true;
                //col.gameObject.GetComponent<VRTK_InteractTouch>().enabled = true;
                //col.gameObject.GetComponent<VRTK_InteractGrab>().enabled = true;

                this.gameObject.GetComponent<Interactable_Item>().enabled = true;
                col.gameObject.GetComponent<Vive_Controller>().enabled = true;

                col.gameObject.GetComponent<HandRotationStatus>().currentRotationTool = null;

                Debug.Log("enable hand");
            }
        }
    }

    /*
     * Checks for Hand and Tool to make sure they are in the correct state for removing the tool 
     */
    bool CheckHandAndToolOnRemoveTool(Collider col)
    {
        if (!col.gameObject.CompareTag("Hand")) return false;

        if (col.gameObject.GetComponent<Vive_Controller>().enabled == true) return false;

        if (snappedObject == null) return false;

        if (col.gameObject != interactingControllerRM) return false;

        if (col.gameObject.GetComponent<HandRotationStatus>().currentRotationTool != this.gameObject) return false;

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
                snappedObject.transform.SetParent(snappedObject.GetComponent<Bolt_SVR>().standardParent.transform, true);
                snappedObject.GetComponent<MeshCollider>().enabled = false;
                snappedObject.GetComponent<MeshCollider>().enabled = true;
                snappedObject.GetComponent<Interactable_Item>().enabled = true;

                snappedObject.GetComponent<Bolt_SVR>().snappedTool = null;
                snappedObject.GetComponent<Bolt_SVR>().hasBeenRemoved = true;

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
        if (partObject.GetComponent<Interactable_Item>().enabled == true)
        {
            partObject.GetComponent<Interactable_Item>().enabled = false;
        }
    }
}
