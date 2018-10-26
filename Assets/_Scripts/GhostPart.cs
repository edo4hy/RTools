using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPart : MonoBehaviour {

	public bool isGhostBucketFull;

    public GameObject actionArrow;
    public GameObject normalPart;

    void Update()
    {
        
    }

    void Start()
    {


    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.GetComponent<Part>() && col.gameObject.GetComponent<Part>().ghostPart == this.gameObject)
        {
            //this.gameObject.GetComponentInParent<ActionController>().CheckProgressThroughStage();
            //Debug.Log("Un_collided");

        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<Part>() && col.gameObject.GetComponent<Part>().ghostPart == this.gameObject)
        {
            //			this.gameObject.GetComponentInParent<ActionController> ().checkProgressThroughStage ();
            //Debug.Log("Collided");

        }
    }


}
