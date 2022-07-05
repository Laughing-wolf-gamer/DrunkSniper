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

    [Header("Attributes")]
    [SerializeField] private NPCType npcType;
    [SerializeField] protected bool isDead,isAlerted,isStandingCover;
    [SerializeField] private float shootDelay = 2f;
    [SerializeField] protected SeeThroughToggle seeThroughToggle;


    protected EnemyAnimationController animationController;
    protected RagdollController ragdollController;
    
    protected bool isBulletMoving;
    protected Coroutine ShootRoutine;
    protected MasterController masterController;
    private void Awake(){
        animationController = GetComponent<EnemyAnimationController>();
        ragdollController = GetComponent<RagdollController>();
        isDead = false;
        // isAlerted;
    }
    protected virtual void Start(){
        PlayDefultMovement();
        lr.gameObject.SetActive(false);
        // lr.positionCount = 2;
        masterController = MasterController.current;
        masterController.onShotComplete += ShootAtPlayer;
    }
    private void Update(){
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
        isAlerted = true;
        lr.gameObject.SetActive(true);
        lr.positionCount = 2;
        animationController.SetStandingCover(isStandingCover);
        if(ShootRoutine == null){
            ShootRoutine = StartCoroutine(ShootPlayerRoutine());
        }
    }
    private IEnumerator ShootPlayerRoutine(){
        while(!isDead){
            lr.gameObject.SetActive(true);
            if(!isBulletMoving){
                int rand = Random.Range(0,shootingPoints.Length);

                // Debug.DrawLine(gunPoint.localPosition,shootingPoints[rand].position,Color.red);
                lr.SetPosition(0,gunPoint.position);
                lr.SetPosition(1,shootingPoints[rand].position);
                if(lookAt){
                    transform.LookAt(new Vector3(shootingPoints[rand].position.x,transform.position.y,shootingPoints[rand].position.z));
                }
                Vector3 dir = (shootingPoints[rand].position - transform.position);
                if(Physics.Raycast(transform.position,dir,out RaycastHit hit,Mathf.Infinity, shootingLayer)){
                    if(hit.transform.TryGetComponent<IDamageable>(out IDamageable damageable)){
                        damageable.TakeDamage(10f);
                    }
                }
                animationController.Fire();
            }
            yield return new WaitForSeconds(shootDelay);
            lr.gameObject.SetActive(false);
        }
        if(ShootRoutine != null){
            StopCoroutine(ShootRoutine);
        }
    }
    
    public void OnEnemyShot(Vector3 shootDirection, Rigidbody shotRB){
        if(seeThroughToggle != null){
            seeThroughToggle.ChaneLayerOnDeath();
        }
        lr.gameObject.SetActive(false);
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
