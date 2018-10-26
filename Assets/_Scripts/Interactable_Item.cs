using UnityEngine;
using System.Collections;


// Reference script on how to set up the controller and the architecture of the page
// https://github.com/b0ard/YoutubeVive/commit/c81f35bbe3f7904bc4890c7c9713fafa1229f534

public class Interactable_Item : MonoBehaviour {

    public Rigidbody rigidbody;

    private bool currentlyInteracting;

    private float velocityFactor = 20000f;
    private Vector3 posDelta;

    private float rotationFactor = 400f;
    private Quaternion rotationDelta;
    private float angle;
    private Vector3 axis;


	public Vive_Controller attached_viveController;

    public Transform interactionPoint;
    private bool defaultInteractionPoint;

    private Vector3 rotationTarget = new Vector3(0, 0, 0);
    private const float angularVelocityFactor = 50f;
    private const float maxAngVelocity = 2000f;

    private Vector3 positionTarget = new Vector3(0, 0, 0);
    private const float velocityFactorF = 6000f;
    private const float maxVelocity = 1000f;

    public bool turnOnKinematicOnDrop;

    // Use this for initialization
    void Start () {
		rigidbody = GetComponent<Rigidbody>();
        if(rigidbody.mass < 10f)
        {
            rigidbody.mass = 10f;
        }
        if(interactionPoint == null)
        {
            interactionPoint = new GameObject().transform;
            defaultInteractionPoint = true;
        }
        velocityFactor /= rigidbody.mass; // heavier objects are harder to move
        rotationFactor /= rigidbody.mass;
    }

    // Update is called once per frame
    void Update()
    {
        if (attached_viveController && currentlyInteracting)
        {
            if (defaultInteractionPoint)
            {
                // Interaction point has not been defined so picked up based on point of interaction
                posDelta = attached_viveController.transform.position - interactionPoint.position;
                positionTarget = (posDelta * velocityFactorF) * Time.deltaTime;
                this.rigidbody.velocity = Vector3.MoveTowards(this.rigidbody.velocity, positionTarget, maxVelocity);

                rotationDelta = attached_viveController.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
                rotationDelta.ToAngleAxis(out angle, out axis);

                if (angle > 180)
                {
                    angle -= 360;
                }

                this.rigidbody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationFactor;
            }
            else
            {
                //// The Interaction point has been defined
                posDelta = attached_viveController.transform.position - interactionPoint.position;
                positionTarget = (posDelta * velocityFactorF) * Time.deltaTime;
                this.rigidbody.velocity = Vector3.MoveTowards(this.rigidbody.velocity, positionTarget, maxVelocity);

                rotationDelta = attached_viveController.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
                rotationDelta.ToAngleAxis(out angle, out axis);

                if(angle > 180)
                {
                    angle -= 360;
                }

                rotationTarget = angle * axis;
                float angularVelocityMagic = angularVelocityFactor;

                rotationTarget = (rotationTarget * angularVelocityMagic) * Time.deltaTime;
                this.rigidbody.angularVelocity = Vector3.MoveTowards(this.rigidbody.angularVelocity, rotationTarget, maxAngVelocity);
            }
        }
    }

    public void BeginInteraction(Vive_Controller wand)
    {
        Debug.Log("Beginiing interaction");
        TurnKinematicOff();
        attached_viveController = wand;
        if (defaultInteractionPoint)
        {
            interactionPoint.position = wand.transform.position;
            interactionPoint.rotation = wand.transform.rotation;
        }
        interactionPoint.SetParent(transform, true);
        currentlyInteracting = true;
    }

    public void EndInteraction(Vive_Controller wand)
    {
        Debug.Log("Ending interaction");
        if (turnOnKinematicOnDrop)
        {
            TurnKinematicOn();
        }
        if (wand == attached_viveController)
        {
            attached_viveController = null;
            currentlyInteracting = false;
        }
    }

    public bool IsInteracting()
    {
        return currentlyInteracting;
    }

    public Vive_Controller GetInteractingController()
    {
        return attached_viveController;
    }

    public GameObject GetInteractingControllerObject()
    {

        if (attached_viveController)
        {
            return attached_viveController.gameObject;
        }
        else
        {
            return null;
        }
    }

    public void TurnKinematicOn()
    {
        Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        if (!rb) return;
        rb.isKinematic = true;
    }

    public void TurnKinematicOff()
    {
        Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        if (!rb) return;
        rb.isKinematic = false;
    }
}
