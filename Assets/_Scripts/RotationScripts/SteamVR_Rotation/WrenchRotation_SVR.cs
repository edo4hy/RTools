using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using NewtonVR;
//using VRTK;

public class WrenchRotation_SVR : RotationalTool_SVR, ITool_Interface
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

                if (snappedObject.GetComponent<Bolt_SVR>().removedAmount < snappedObject.GetComponent<Bolt_SVR>().degreeToRemove)
                {

                    snappedObject.GetComponent<Bolt_SVR>().removedAmount += ratchetClick;
                    boltBeingRemovedFactor = ((snappedObject.GetComponent<Bolt_SVR>().distanceToBeRemoved / snappedObject.GetComponent<Bolt_SVR>().degreeToRemove) * ratchetClick);

                    this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x,
                        this.gameObject.transform.position.y + boltBeingRemovedFactor,
                        this.gameObject.transform.position.z);

                    //snappedObject.GetComponent<Bolt_SVR>().ghostPart.transform.position = snappedObject.transform.position;
                    //						if (boltBeingRemovedFactor > 0.20f) {
                    //							Debug.Break ();
                    //						}

                }
                else
                {
                    snappedObject.GetComponent<Interactable_Item>().enabled = true;
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

            if (snappedObject.GetComponent<Bolt_SVR>().removedAmount > 0)
            {
                snappedObject.GetComponent<Bolt_SVR>().removedAmount -= ratchetClick;

                boltBeingRemovedFactor = ((snappedObject.GetComponent<Bolt_SVR>().distanceToBeRemoved /
                snappedObject.GetComponent<Bolt_SVR>().degreeToRemove) * ratchetClick);

                this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x,
                    this.gameObject.transform.position.y - boltBeingRemovedFactor,
                    this.gameObject.transform.position.z);

                //snappedObject.GetComponent<Bolt_SVR>().ghostPart.transform.position = snappedObject.transform.position;
            }
        }
    }
}








