using UnityEngine;
using System.Collections;
using NewtonVR;

[RequireComponent(typeof(NVRHand))]
[RequireComponent(typeof(SteamVR_TrackedController))]

public class RotationHand_NVR: MonoBehaviour
{
	public GameObject currentRotationTool;
}

