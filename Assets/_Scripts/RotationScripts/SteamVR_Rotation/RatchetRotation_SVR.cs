using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using NewtonVR;
//using VRTK;


public class RatchetRotation_SVR : RotationalTool_SVR {

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
                    // Ensure bolt is parented to tool - set start rotation to current rotation - pulse
                    snappedObject.transform.SetParent( this.transform, true);
					DisableNVRInteractable (snappedObject);
					isBoltAttached = true;
					startRotation = this.gameObject.transform.rotation;
					SteamVR_Controller.Input ((int)interactingControllerRM.GetComponentInParent<SteamVR_TrackedObject>().index).TriggerHapticPulse (5000);

					if (snappedObject.GetComponent<Bolt_SVR> ().removedAmount < snappedObject.GetComponent<Bolt_SVR> ().degreeToRemove) 
					{

						snappedObject.GetComponent<Bolt_SVR> ().removedAmount += ratchetClick;
						boltBeingRemovedFactor = ((snappedObject.GetComponent<Bolt_SVR> ().distanceToBeRemoved / snappedObject.GetComponent<Bolt_SVR> ().degreeToRemove) * ratchetClick);

						this.gameObject.transform.position = new Vector3 (this.gameObject.transform.position.x, 
							this.gameObject.transform.position.y + boltBeingRemovedFactor, 
							this.gameObject.transform.position.z);
						
						//snappedObject.GetComponent<Bolt_SVR> ().ghostPart.transform.position = snappedObject.transform.position;
					}
					else 
					{
						snappedObject.GetComponent<Interactable_Item> ().enabled = true;
						//						toolSnapBackToHand (currentControllerObject.gameObject, false);
					}
				}

			} else if (rotationAmount.y < -ratchetClick) 
			{
                snappedObject.transform.SetParent(snappedObject.GetComponent<Bolt_SVR> ().standardParent.transform, true);
				isBoltAttached = false;

				SteamVR_Controller.Input ((int)interactingControllerRM.GetComponentInParent<SteamVR_TrackedObject>().index).TriggerHapticPulse (1000);
				startRotation = this.gameObject.transform.rotation;
			}
		} 
		else if (toolEffect == ToolEffect.tighten) 
		{
			if (rotationAmount.y < -ratchetClick) 
			{
				if (interactingControllerRM != null) 
				{
                    // Ensure bolt is parented to tool - set start rotation to current rotation - pulse
                    snappedObject.transform.SetParent( this.transform, true);
					DisableNVRInteractable (snappedObject);
					isBoltAttached = true;
					startRotation = this.gameObject.transform.rotation;
					SteamVR_Controller.Input ((int)interactingControllerRM.GetComponentInParent<SteamVR_TrackedObject>().index).TriggerHapticPulse (5000);


					if (snappedObject.GetComponent<Bolt_SVR> ().removedAmount > 0) {
						snappedObject.GetComponent<Bolt_SVR> ().removedAmount -= ratchetClick;

						boltBeingRemovedFactor = ((snappedObject.GetComponent<Bolt_SVR> ().distanceToBeRemoved /
						snappedObject.GetComponent<Bolt_SVR> ().degreeToRemove) * ratchetClick);

						this.gameObject.transform.position = new Vector3 (this.gameObject.transform.position.x, 
							this.gameObject.transform.position.y - boltBeingRemovedFactor, 
							this.gameObject.transform.position.z);
						
						//snappedObject.GetComponent<Bolt_SVR> ().ghostPart.transform.position = snappedObject.transform.position;
					} 
				}
			} 
			else if (rotationAmount.y > ratchetClick) 
			{
                snappedObject.transform.SetParent(snappedObject.GetComponent<Bolt_SVR> ().standardParent.transform, true);
				isBoltAttached = false;

				SteamVR_Controller.Input ((int)interactingControllerRM.GetComponentInParent<SteamVR_TrackedObject>().index).TriggerHapticPulse (1000);
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




