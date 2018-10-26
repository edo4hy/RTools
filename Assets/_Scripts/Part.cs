using UnityEngine;
using System.Collections;
using NewtonVR;

public class Part : MonoBehaviour {


    public Material normalMaterial;
    public Material highlightMaterial;
    public Material silouetteMaterial;

    //[HideInInspector]
	public GameObject standardParent;
    //[HideInInspector]
	public GameObject ghostPart;
    //[HideInInspector]
	public GameObject removedObject;

    //[HideInInspector]
	public bool hasBeenRemoved;

	public int actionStage;
    public int subActionStage;


    void Start()
    {
        normalMaterial = gameObject.GetComponent<Renderer>().material;

		if (standardParent == null) {
			standardParent = GameObject.Find ("PartStore");
		}
		if (removedObject == null) {
			removedObject = GameObject.Find ("PartStoreG");
		}
    }
}
