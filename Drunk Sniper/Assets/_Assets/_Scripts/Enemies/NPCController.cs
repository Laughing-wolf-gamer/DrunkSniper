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
    
    [SerializeField] private Transform[] shootingPoints;
    [SerializeField] private NPCType npcType;
    [SerializeField] protected bool isDead,isAlerted;
    [SerializeField] private float shootDelay = 2f;
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
        masterController = MasterController.current;
        masterController.onShotComplete += ShootAtPlayer;
    }
    private void Update(){
        isBulletMoving = masterController.IsBulletMoving;
        if(isBulletMoving){
            animationController.ReduceAnimationSpeed();
        }else{
            animationController.BackToNormal();
        }
    }
    
    public virtual void PlayDefultMovement(){

    }
    
    public virtual void ShootAtPlayer(){
        Debug.Log("Fired");
        isAlerted = true;
        ShootRoutine = StartCoroutine(ShootPlayerRoutine());
    }
    private IEnumerator ShootPlayerRoutine(){
        while(!isDead){
            if(!isBulletMoving){
                int rand = Random.Range(0,shootingPoints.Length);
                Debug.DrawLine(transform.position,shootingPoints[rand].position,Color.red,20f);
                transform.LookAt(new Vector3(shootingPoints[rand].position.x,transform.position.y,shootingPoints[rand].position.z));
                animationController.Fire();
            }
            yield return new WaitForSeconds(shootDelay);
        }
        if(ShootRoutine != null){
            StopCoroutine(ShootRoutine);
        }
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
