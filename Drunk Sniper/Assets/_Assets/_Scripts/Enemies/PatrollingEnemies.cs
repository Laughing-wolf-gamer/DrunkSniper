using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatrollingEnemies : NPCController {
    // [SerializeField] private Transform[] movePoints;
    [SerializeField] private float moveSpeed = 20f;
    
    [SerializeField] private Transform[] movePoints;
    [SerializeField] private Transform[] shootingPoints;
    
    private int currentMoveIndex;
    protected override void Start(){
        base.Start();
        
    }
    public override void PlayDefultMovement(){
        base.PlayDefultMovement();
        StartCoroutine(MovementRoutine());
    }

    private IEnumerator MovementRoutine(){
        animationController.SetDefultAnimation(true);
        while(!isDead){
            if(!isAlerted){
                if(movePoints.Length > 0){
                    float dist = Vector3.Distance(transform.position,movePoints[currentMoveIndex].position);
                    transform.LookAt(new Vector3(movePoints[currentMoveIndex].position.x,transform.position.y,movePoints[currentMoveIndex].position.z));
                    if(dist >= 0.01f){
                        transform.position = Vector3.MoveTowards(transform.position,movePoints[currentMoveIndex].position,moveSpeed * Time.deltaTime);
                    }else{
                        currentMoveIndex ++;
                        if(currentMoveIndex > movePoints.Length - 1){
                            currentMoveIndex = 0;
                        }

                    }
                }
            }
            yield return null;
        }
    }
    // public override void ShootAtPlayer(){
    //     base.ShootAtPlayer();
    //     // Debug.Log("Fired");
    //     // isAlerted = true;
    //     // ShootRoutine = StartCoroutine(ShootPlayerRoutine());
    // }
    // private IEnumerator ShootPlayerRoutine(){
    //     while(!isDead){
    //         if(!isBulletMoving){
    //             int rand = Random.Range(0,shootingPoints.Length);
    //             Debug.DrawLine(transform.position,shootingPoints[rand].position,Color.red,20f);
    //             transform.LookAt(new Vector3(shootingPoints[rand].position.x,transform.position.y,shootingPoints[rand].position.z));
    //             animationController.Fire();
    //         }
    //         yield return new WaitForSeconds(shootDelay);
    //     }
    //     if(ShootRoutine != null){
    //         StopCoroutine(ShootRoutine);
    //     }
    // }
}
