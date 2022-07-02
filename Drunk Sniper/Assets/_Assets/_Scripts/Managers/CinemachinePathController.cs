using Cinemachine;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CinemachineSmoothPath))]
public abstract class CinemachinePathController : MonoBehaviour {
    public CinemachineSmoothPath path;
    public BoxCollider boxCollider;

    public abstract bool CheckIfPathISClear(Transform target, float distance, Quaternion orientation);

}
