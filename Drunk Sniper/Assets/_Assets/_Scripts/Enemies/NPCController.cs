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
    
    [Header("Shooting Variables")]
    [SerializeField] private LayerMask shootingLayer;
    [SerializeField] private Transform[] shootingPoints;
    [SerializeField] private LineRenderer lr;
    [SerializeField] protected bool lookAt;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private Vector3 offset;

    [Header("Attributes")]
    [SerializeField] private float fallForce = 100f;
    [SerializeField] private NPCType npcType;
    [SerializeField] protected bool isDead,isAlerted,isStandingCover;
    [SerializeField] private float shootDelay = 2f;
    [SerializeField] protected SeeThroughToggle seeThroughToggle;


    protected EnemyAnimationController animationController;
    protected RagdollController ragdollController;
    
    protected bool isBulletMoving;
    protected MasterController masterController;
    private void Awake(){
        animationController = GetComponent<EnemyAnimationController>();
        ragdollController = GetComponent<RagdollController>();
        isDead = false;
        // isAlerted;
    }
    protected virtual void Start(){
        PlayDefultMovement();
        if(lr != null){
            lr.gameObject.SetActive(false);
        }
        // lr.positionCount = 2;
        masterController = MasterController.current;
        masterController.OnShotComplete += ShootAtPlayer;
        if(isAlerted){
            masterController.InvokeShotComplete();
        }
    }
    protected virtual void Update(){
        isBulletMoving = masterController.IsBulletMoving;
        animationController.Alerted(isAlerted);
        if(isBulletMoving){
            animationController.ReduceAnimationSpeed();
        }else{
            animationController.BackToNormal();
        }
    }
    
    public virtual void PlayDefultMovement(){

    }
    
    public virtual void ShootAtPlayer(){
        if(npcType != NPCType.HostageVictim){
            isAlerted = true;
            animationController.SetStandingCover(isStandingCover);
            if(lr != null){
                lr.gameObject.SetActive(true);
                lr.positionCount = 2;
            }
            StartCoroutine(ShootPlayerRoutine());
        }
    }
    private int currentRand;
    private IEnumerator ShootPlayerRoutine(){
        while(!isDead){
            
            if(!isBulletMoving){
                currentRand = Random.Range(0,shootingPoints.Length);
                
                if(lookAt){
                    transform.LookAt(new Vector3(shootingPoints[currentRand].position.x,transform.position.y,shootingPoints[currentRand].position.z));
                }
                
                animationController.Fire();
            }
            yield return new WaitForSeconds(shootDelay);
        }
    }
    public void DeactivateLR(){
        if(lr != null){
            lr.gameObject.SetActive(false);
        }
    }
    public void ShootNow(){
        if(lr != null){
            lr.gameObject.SetActive(true);
            lr.SetPosition(0,gunPoint.position + offset);
            lr.SetPosition(1,shootingPoints[currentRand].position);
        }
        Vector3 dir = (shootingPoints[currentRand].position - transform.position);
        if(Physics.Raycast(transform.position,dir,out RaycastHit hit,Mathf.Infinity, shootingLayer)){
            if(hit.transform.TryGetComponent<IDamageable>(out IDamageable damageable)){
                damageable.TakeDamage(10f);
            }
        }
    }
    public void OnEnemyShot(Vector3 shootDirection, Rigidbody shotRB){
        if(seeThroughToggle != null){
            seeThroughToggle.ChaneLayerOnDeath();
        }
        if(lr != null){
            lr.gameObject.SetActive(false);
        }
        isDead = true;
        StopAnimation();
        ragdollController.EnableRagdoll();
        if (shotRB){
            shotRB.AddForce(shootDirection.normalized * fallForce, ForceMode.Impulse);
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
