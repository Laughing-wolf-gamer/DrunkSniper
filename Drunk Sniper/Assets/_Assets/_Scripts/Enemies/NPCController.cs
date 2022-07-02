using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public enum NPCType{
    Patrolling,Static,HostageVictim,HostageVillin
}
[RequireComponent(typeof(EnemyAnimationController))]
[RequireComponent(typeof(RagdollController))]
public class NPCController : MonoBehaviour {

    [SerializeField] private NPCType npcType;
    [SerializeField] protected bool isDead,isAlerted;
    protected EnemyAnimationController animationController;
    protected RagdollController ragdollController;
    
    protected bool isBulletMoving;
    protected Coroutine ShootRoutine;
    protected MasterController masterController;
    private void Awake(){
        animationController = GetComponent<EnemyAnimationController>();
        ragdollController = GetComponent<RagdollController>();
        isDead = false;
        isAlerted = false;
    }
    protected virtual void Start(){
        PlayDefultMovement();
        masterController = MasterController.current;
        masterController.OnPlayerFirstShotComplete += ()=>{
            ShootAtPlayer();
        };
    }
    private void Update(){
        isBulletMoving = masterController.IsBulletMoving;
    }
    
    public virtual void PlayDefultMovement(){

    }
    
    public virtual void ShootAtPlayer(){
        
    }
    
    public void OnEnemyShot(Vector3 shootDirection, Rigidbody shotRB){
        isDead = true;
        StopAnimation();
        ragdollController.EnableRagdoll();
        if (shotRB){
            shotRB.AddForce(shootDirection.normalized * 100f, ForceMode.Impulse);
        }            
    }

    public void StopAnimation(){
        animationController.DisableAnimator();
    }
    public NPCType GetNPCType(){
        return npcType;
    }
    public bool IsDead(){
        return isDead;
    }
}
