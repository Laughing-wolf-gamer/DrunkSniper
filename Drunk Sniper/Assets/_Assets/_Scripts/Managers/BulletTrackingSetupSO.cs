using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "New Tracking",menuName = "ScriptableObject/Tracking SetUp")]
public class BulletTrackingSetupSO : TargetTrackingSetupSO{
    public float minDistance;
	public float maxDistance;
}
