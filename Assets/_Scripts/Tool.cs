using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour {


	public BoltSize toolSize;
	public ToolType toolType;

	public bool snappedToPosition;

	public GameObject snappedObject;

	public bool beingRemoved;

	public bool inSnappedRotationMode;

	public enum ToolEffect {
		tighten, losen

	};

	public ToolEffect toolEffect = ToolEffect.losen;
}
