using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class VRMap {
    public Transform VRTarget;
    public Transform RigTarget;
    public Vector3 TrackingPosOffset;
    public Vector3 TrackingRotOffset;

    public void Map() {
        RigTarget.position = VRTarget.TransformPoint(TrackingPosOffset);
        RigTarget.rotation = VRTarget.rotation * Quaternion.Euler(TrackingRotOffset);

    }

}


public class VRig : MonoBehaviour
{
    public VRMap head;
    public VRMap rightHand;
    public VRMap leftHand;

    public Transform headCons;
    public Vector3 headBodyOffset;
    public float TurnSmoothness;


    // Start is called before the first frame update
    void Start()
    {
        headBodyOffset = transform.position - headCons.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = headCons.position + headBodyOffset;
      //  transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(headCons.up,Vector3.up).normalized,Time.deltaTime*TurnSmoothness);
        head.Map();
        rightHand.Map();
        leftHand.Map();


    }
}
