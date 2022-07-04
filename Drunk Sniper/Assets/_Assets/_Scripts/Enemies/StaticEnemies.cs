using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaticEnemies : NPCController {
    
    
    
    [SerializeField] private float coverTime;
    public override void PlayDefultMovement(){
        base.PlayDefultMovement();
        StartCoroutine(StartCoviering());
    }
    protected override void Start(){
        base.Start();
        base.ShootAtPlayer();
    }
    private IEnumerator StartCoviering(){
        while(!isDead){
            if(isAlerted){
                animationController.SetDefultAnimation(false);
            }else{
                animationController.SetDefultAnimation(true);
                base.ShootAtPlayer();
            }
            yield return new WaitForSeconds(coverTime);
        }
    }
    
}
