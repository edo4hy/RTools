using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewtonVR;

public class ComponentToBucket : MonoBehaviour
{
    public Vive_Controller leftController;
    public Vive_Controller rightController;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider col)
    {
		SnapToGhostShowShilhouette(col);
    }

    private void OnTriggerStay(Collider col)
    {
		SnapToGhostShowShilhouette(col);
    }

    private void OnTriggerExit(Collider col)
    {
		if (col.gameObject == this.GetComponent<Part>().ghostPart)
        {
            if (this.gameObject.GetComponent<Bolt_NVR>())
            {
                if (this.gameObject.GetComponent<Bolt_NVR>().snappedTool == null)
                {
                    this.gameObject.GetComponent<Part>().hasBeenRemoved = true;
                    this.gameObject.GetComponent<Part>().ghostPart.GetComponent<GhostPart>().isGhostBucketFull = false;
                }
            }
            else
            {
                this.gameObject.GetComponent<Part>().hasBeenRemoved = true;
                this.gameObject.GetComponent<Part>().ghostPart.GetComponent<GhostPart>().isGhostBucketFull = false;
            }
			
			col.gameObject.GetComponent<MeshRenderer> ().enabled = false;
        }
    }


    public delegate void SnapPartBackToGhost();
    public static event SnapPartBackToGhost OnSnapPartBackToGhost;

	private void SnapToGhostShowShilhouette(Collider col)
	{
		if (col.gameObject.CompareTag ("Ghost")) 
		{
			if (col.gameObject == this.GetComponent<Part>().ghostPart) 
			{
				// If hand picks up bolt when snapped to ghost - 
				if (this.gameObject.GetComponent<NVRInteractableItem> ().AttachedHand) 
				{
					col.gameObject.GetComponent<GhostPart> ().isGhostBucketFull = false;
					this.gameObject.GetComponent<Part> ().hasBeenRemoved = true;
				}

				if (col.gameObject.GetComponent<GhostPart> () && col.gameObject.GetComponent<GhostPart> ().isGhostBucketFull == true) return; 

				if (this.gameObject.GetComponent<NVRInteractableItem> ().AttachedHand != null)
				{
					col.gameObject.GetComponent<MeshRenderer> ().enabled = true;
				} 
				else
				{
					if (!this.gameObject.GetComponent<Part> ().hasBeenRemoved) 	return; 

					this.transform.position = col.gameObject.transform.position;
					this.transform.rotation = col.gameObject.transform.rotation;
					col.gameObject.GetComponent<MeshRenderer> ().enabled = false;
					this.gameObject.GetComponent<Part> ().hasBeenRemoved = false;
					col.gameObject.GetComponent<GhostPart> ().isGhostBucketFull = true;
                    OnSnapPartBackToGhost();
                    Debug.Log ("Snap back to ghost");
				}
			}
		}
	}



//    private void showShilhouetteAndSnapToGhost(Collider col)
//    {
//		
//        if (leftController.gameObject.GetComponent<NewtonVR.NVRHand>() == null && rightController.gameObject.GetComponent<NewtonVR.NVRHand>() == null) return;
//
//        //if (leftController.gameObject.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting == null && rightController.gameObject.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting == null) return;
//        if (Collider.gameObject.name.Replace("mesh_", "") == this.name)
//        {
//                // Snap to and show Mesh silhouette if collision with either mesh or cube. 
//		if(this.gameObject.GetComponent<Part>().hasBeenRemoved != true){return;}
//
//        if (Collider.gameObject.name.Contains("mesh_"))
//                    {
//                    if ((leftController.gameObject.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting != null && Collider.gameObject.name.Replace("mesh_", "") == leftController.gameObject.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.gameObject.name) ||
//                       (rightController.gameObject.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting != null && Collider.gameObject.name.Replace("mesh_", "") == rightController.gameObject.GetComponent<NewtonVR.NVRHand>().CurrentlyInteracting.gameObject.name))
//                    {
//                        Collider.gameObject.GetComponent<MeshRenderer>().enabled = true;
//                    }else
//                    {
//		                transform.position = Collider.gameObject.transform.position;
//		                transform.rotation = Collider.gameObject.transform.rotation;
//		                Collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
//						this.gameObject.GetComponent<Part> ().hasBeenRemoved = false;
//                        Debug.Log("snap from mesh");
//                    }
//                        
//                    }
//                
//                }
//
//                //hide shilouette if in correct position and not interacting
//                if (transform.position == Collider.gameObject.transform.position && transform.rotation == Collider.gameObject.transform.rotation)
//                {
////			Debug.Log ("6");
//                    if (!leftController.isInteracting && !rightController.isInteracting)
//                    {
//                       if (Collider.gameObject.name.Contains("mesh_"))
//                        {
//                                Collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
//                            //Debug.Log("hide from mesh");
//                }
//            }
//        }
//    }

}


