using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToBucket : MonoBehaviour {

    private NewtonVR.NVRHand controller;

	// Use this for initialization
	void Start () {
        controller = GetComponent<NewtonVR.NVRHand>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void showSilhouette(Collider Collider)
    {

    }

    private void OnTriggerStay(Collider Collider)
    {
        showSilhouette(Collider);
    }

    private void OnTriggerEnter(Collider Collider)
    {
        showSilhouette(Collider);

    }


}
