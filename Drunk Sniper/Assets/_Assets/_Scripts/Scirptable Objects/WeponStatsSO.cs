using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Wepon Stats",menuName = "ScriptableObject/Wepon Stats")]
public class WeponStatsSO : ScriptableObject {
    public float bulletSpeed;
    public float scopeFov;
    public float reloadingTime;
    public float coolDownTime = 0.2f;
    public int maxBulletInMag = 5;
}
