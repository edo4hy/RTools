using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITool_Interface
{
    void HandEnterTool(Collider col);

    void ToolSnapToBolt(Collider col);

    void AffectRotationOnBolt();

    void RotateToolWithHand();

    void RemoveToolSnapToHand(Collider col);

    void ReleaseBolt();

    Vector3 EulerAnglesBetween(Quaternion from, Quaternion to);

    void DisableNVRInteractable(GameObject partObject);
}
