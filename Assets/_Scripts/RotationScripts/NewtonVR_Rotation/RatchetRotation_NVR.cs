using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewtonVR;


public class RatchetRotation_NVR : RotationalTool_NVR {

	public float changeTime  = 0f;

	public enum ToolEffect
    {
		tighten, losen
	};
	public ToolEffect toolEffect = ToolEffect.losen;

    private new void Update()
    {
        base.Update();

        if (toolInRotationMode)
        {
            AffectRotationOnBolt();
        }
    }


    private new void OnTriggerStay(Collider col)
    {

        base.OnTriggerStay(col);

        if (col.gameObject.CompareTag("Hand"))
        {
            ChangeRotationDirection(col);
        }

    }

    /*
    * Ratchet Rotations implementation on affectRotation on bolt - 
    * 
    */
    void AffectRotationOnBolt()
	{
		//Rotations of tools effect on the bolt
		Vector3 rotationAmount = EulerAnglesBetween (startRotation, this.gameObject.transform.rotation);
		if (toolEffect == ToolEffect.losen) 
		{
			if (rotationAmount.y > ratchetClick) 
			{
				if (interactingControllerRM != null ) 
				{
                    Debug.Log("We are rotating 2");
                    // Ensure bolt is parented to tool - set start rotation to current rotation - pulse
                    snappedObject.transform.SetParent( this.transform, true);
					DisableNVRInteractable (snappedObject);
					isBoltAttached = true;
					startRotation = this.gameObject.transform.rotation;
					SteamVR_Controller.Input ((int)interactingControllerRM.GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse (5000);

					if (snappedObject.GetComponent<Bolt_NVR> ().removedAmount < snappedObject.GetComponent<Bolt_NVR> ().degreeToRemove) 
					{

						snappedObject.GetComponent<Bolt_NVR> ().removedAmount += ratchetClick;
						boltBeingRemovedFactor = ((snappedObject.GetComponent<Bolt_NVR> ().distanceToBeRemoved / snappedObject.GetComponent<Bolt_NVR> ().degreeToRemove) * ratchetClick);

						this.gameObject.transform.position = new Vector3 (this.gameObject.transform.position.x, 
							this.gameObject.transform.position.y + boltBeingRemovedFactor, 
							this.gameObject.transform.position.z);
						
						snappedObject.GetComponent<Bolt_NVR> ().ghostPart.transform.position = snappedObject.transform.position;
					}
					else 
					{
						snappedObject.GetComponent<NVRInteractableItem> ().enabled = true;
						//						toolSnapBackToHand (currentControllerObject.gameObject, false);
					}
				}

			} else if (rotationAmount.y < -ratchetClick) 
			{
                Debug.Log("We are rotating 3");
                snappedObject.transform.SetParent(snappedObject.GetComponent<Bolt_NVR> ().standardParent.transform, true);
				isBoltAttached = false;

				SteamVR_Controller.Input ((int)interactingControllerRM.GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse (1000);
				startRotation = this.gameObject.transform.rotation;

			}
		} 
		else if (toolEffect == ToolEffect.tighten) 
		{
			if (rotationAmount.y < -ratchetClick) 
			{
				if (interactingControllerRM != null) 
				{
                    Debug.Log("We are rotating 4");
                    // Ensure bolt is parented to tool - set start rotation to current rotation - pulse
                    snappedObject.transform.SetParent( this.transform, true);
					DisableNVRInteractable (snappedObject);
					isBoltAttached = true;
					startRotation = this.gameObject.transform.rotation;
					SteamVR_Controller.Input ((int)interactingControllerRM.GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse (5000);


					if (snappedObject.GetComponent<Bolt_NVR> ().removedAmount > 0) {
						snappedObject.GetComponent<Bolt_NVR> ().removedAmount -= ratchetClick;

						boltBeingRemovedFactor = ((snappedObject.GetComponent<Bolt_NVR> ().distanceToBeRemoved /
						snappedObject.GetComponent<Bolt_NVR> ().degreeToRemove) * ratchetClick);

						this.gameObject.transform.position = new Vector3 (this.gameObject.transform.position.x, 
							this.gameObject.transform.position.y - boltBeingRemovedFactor, 
							this.gameObject.transform.position.z);
						
						snappedObject.GetComponent<Bolt_NVR> ().ghostPart.transform.position = snappedObject.transform.position;
					} 
				}
			} 
			else if (rotationAmount.y > ratchetClick) 
			{
                Debug.Log("We are rotating 5");

                snappedObject.transform.SetParent(snappedObject.GetComponent<Bolt_NVR> ().standardParent.transform, true);
				isBoltAttached = false;

				SteamVR_Controller.Input ((int)interactingControllerRM.GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse (1000);
				startRotation = this.gameObject.transform.rotation;
			}
		}
	}


	private void ToggleRotationDirection(GameObject tool)
	{
		if (toolEffect == ToolEffect.losen) 
		{
			toolEffect = ToolEffect.tighten;
		}
        else
        {
			toolEffect = ToolEffect.losen;
		}
	}


	void ChangeRotationDirection(Collider col)
    {
		if (col.gameObject.GetComponent<SteamVR_TrackedController> ())
        {
			SteamVR_TrackedController controllerController = col.gameObject.GetComponent<SteamVR_TrackedController> ();

			if (controllerController.padPressed == true)
			{
				if (Time.time > changeTime) 
				{
					changeTime = Time.time + 1;
					ToggleRotationDirection (this.gameObject);
				}
			}
		}
	}

}




