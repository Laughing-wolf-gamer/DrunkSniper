using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Pool",menuName = "Gamer Wolf Utilities/Object Pooling/Pool")]
public class PoolSO : ScriptableObject {
    public string tag;
    public GameObject prefabs;
    public int size;
    
    
}
