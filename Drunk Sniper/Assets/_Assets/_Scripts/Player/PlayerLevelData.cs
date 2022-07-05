using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Player Data",menuName = "ScriptableObject/Player Level Data")]
public class PlayerLevelData : ScriptableObject {
    public float bulletSpeed;
    public float xMinLook,xMaxLook;
    public float yMinLook,yMaxLook;
    public WeponStatsSO weponStatsSO;
    
}
