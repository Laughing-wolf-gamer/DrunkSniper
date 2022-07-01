using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public enum NPCType{
    Patrolling,Static,Hostage,hostageHoldingEnemy
}
[RequireComponent(typeof(EnemyAnimationController))]
[RequireComponent(typeof(RagdollController))]
public class EnemyController : MonoBehaviour {

    [SerializeField] private NPCType npcType;
    [SerializeField] protected bool isDead,isAlerted;
    [SerializeField] private Transform[] shootingPoints;
    [SerializeField] private float shootDelay = 2f;
    protected EnemyAnimationController animationController;
    protected RagdollController ragdollController;
    public Action OnPlayerShoot;
    protected bool isBulletMoving;
    protected Coroutine ShootRoutine;
    private void Awake(){
        animationController = GetComponent<EnemyAnimationController>();
        ragdollController = GetComponent<RagdollController>();
        isDead = false;
        isAlerted = false;
    }
    protected virtual void Start(){
        PlayDefultMovement();
        OnPlayerShoot += ()=>{
            ShootAtPlayer();
        };
    }
    private void Update(){
        isBulletMoving = TimeScaleController.current.IsBulletMoving;
    }
    [ContextMenu("OnShot")]
    public void OnFire(){
        OnPlayerShoot?.Invoke();
    }
    public virtual void PlayDefultMovement(){

    }
    
    public void ShootAtPlayer(){
        if(ShootRoutine == null){
            Debug.Log("Fired");
            isAlerted = true;
            ShootRoutine = StartCoroutine(ShootPlayerRoutine());
        }
    }
    private IEnumerator ShootPlayerRoutine(){
        while(!isDead){
            int rand = Random.Range(0,shootingPoints.Length);
            transform.LookAt(new Vector3(shootingPoints[rand].position.x,transform.position.y,shootingPoints[rand].position.z));
            animationController.Fire();
            yield return new WaitForSeconds(shootDelay);
        }
        StopCoroutine(ShootRoutine);
    }
    public void OnEnemyShot(Vector3 shootDirection, Rigidbody shotRB){
        
        StopAnimation();
        ragdollController.EnableRagdoll();
        if (shotRB)
        {
            shotRB.AddForce(shootDirection.normalized * 100f, ForceMode.Impulse);
        }            
    }

    public void StopAnimation(){
        animationController.DisableAnimator();
    }

}
