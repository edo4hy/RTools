using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewtonVR;

public class WrenchRotation_NVR : RotationalTool_NVR, ITool_Interface
{
    private new void Update()
    {
        base.Update();

        if (toolInRotationMode)
        {
            AffectRotationOnBolt();
        }
    }
  
/*
 * 
 */
    public void AffectRotationOnBolt()
    {
        //Rotations of tools effect on the bolt
        Vector3 rotationAmount = EulerAnglesBetween(startRotation, this.gameObject.transform.rotation);

        if (rotationAmount.y > ratchetClick)
        {
            if (interactingControllerRM != null)
            {
                // Ensure bolt is parented to tool - set start rotation to current rotation - pulse
                snappedObject.transform.SetParent(this.transform, true);
                DisableNVRInteractable(snappedObject);
                isBoltAttached = true;
                startRotation = this.gameObject.transform.rotation;
                SteamVR_Controller.Input((int)interactingControllerRM.GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse(5000);

                if (snappedObject.GetComponent<Bolt_NVR>().removedAmount < snappedObject.GetComponent<Bolt_NVR>().degreeToRemove)
                {

                    snappedObject.GetComponent<Bolt_NVR>().removedAmount += ratchetClick;
                    boltBeingRemovedFactor = ((snappedObject.GetComponent<Bolt_NVR>().distanceToBeRemoved / snappedObject.GetComponent<Bolt_NVR>().degreeToRemove) * ratchetClick);

                    this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x,
                        this.gameObject.transform.position.y + boltBeingRemovedFactor,
                        this.gameObject.transform.position.z);

                    //snappedObject.GetComponent<Bolt_NVR>().ghostPart.transform.position = snappedObject.transform.position;
                    //						if (boltBeingRemovedFactor > 0.20f) {
                    //							Debug.Break ();
                    //						}

                }
                else
                {
                    snappedObject.GetComponent<NVRInteractableItem>().enabled = true;
                    //						toolSnapBackToHand (currentControllerObject.gameObject, false);
                }
            }
        }
        else if (rotationAmount.y < -ratchetClick) 
        {
            // Ensure bolt is parented to tool - set start rotation to current rotation - pulse
            snappedObject.transform.SetParent(this.transform, true);
            DisableNVRInteractable(snappedObject);
            isBoltAttached = true;
            startRotation = this.gameObject.transform.rotation;
            SteamVR_Controller.Input((int)interactingControllerRM.GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse(5000);

            if (snappedObject.GetComponent<Bolt_NVR>().removedAmount > 0)
            {
                snappedObject.GetComponent<Bolt_NVR>().removedAmount -= ratchetClick;

                boltBeingRemovedFactor = ((snappedObject.GetComponent<Bolt_NVR>().distanceToBeRemoved /
                snappedObject.GetComponent<Bolt_NVR>().degreeToRemove) * ratchetClick);

                this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x,
                    this.gameObject.transform.position.y - boltBeingRemovedFactor,
                    this.gameObject.transform.position.z);

                //snappedObject.GetComponent<Bolt_NVR>().ghostPart.transform.position = snappedObject.transform.position;
            }
        }
    }
}








