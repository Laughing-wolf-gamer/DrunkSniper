using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
public class HostageVillin : NPCController {
    [SerializeField] private Rig handRig;
    [SerializeField] private HostageVictim victim;    
    [SerializeField] private Rigidbody victimeRb;
    public override void PlayDefultMovement(){
        base.PlayDefultMovement();
        TakeHostage();
    }
    private void TakeHostage(){
        animationController.SetDefultAnimation(true);
    }
    protected override void Start(){
        base.Start();
        masterController.OnPlayerFirstShotComplete += ()=>{
            if(!isDead){
                handRig.weight = 0f;
                victim.OnEnemyShot(transform.forward,victimeRb);
                Debug.Log("Shoot The Hostage");
                animationController.Fire();
            }
        };
    }
    
    
}
