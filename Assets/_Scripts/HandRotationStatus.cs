using UnityEngine;
using System.Collections;
using NewtonVR;

[RequireComponent(typeof(NVRHand))]
[RequireComponent(typeof(SteamVR_TrackedController))]

public class HandRotationStatus: MonoBehaviour
{
	public GameObject currentRotationTool;
}

