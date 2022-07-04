using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HostageVictim : NPCController {
    [SerializeField] private HostageVillin hostageVillin;
    public override void PlayDefultMovement(){
        base.PlayDefultMovement();
        TakenHostage();
    }
    protected override void Start(){
        base.Start();
        masterController.OnPlayerFirstShotComplete += ()=>{
            if(hostageVillin.IsDead()){
                Duck();
            }else{
                Debug.Log("Victime Dead");
            }
        };
    }
    private void TakenHostage(){
        animationController.SetDefultAnimation(true);
    }
    public void Duck(){
        animationController.Duck();
    }
}
