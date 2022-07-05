using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SeeThroughToggle : MonoBehaviour {
    
    [SerializeField] private int changeToLayerNumber;
    public void ChaneLayerOnDeath(){
        gameObject.layer = changeToLayerNumber;
    }
}
