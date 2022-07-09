using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatrollingEnemies : NPCController {
    // [SerializeField] private Transform[] movePoints;
    [SerializeField] private float moveSpeed = 20f;
    
    [SerializeField] private Transform[] movePoints;
    // [SerializeField] private Transform[] shootingPoints;
    
    private int currentMoveIndex;
    private float currentMoveSpeed;
    protected override void Start(){
        base.Start();
        
        
    }
    protected override void Update(){
        base.Update();
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
                    if(isBulletMoving){
                        currentMoveSpeed = 1.2f;
                    }else {
                        currentMoveSpeed = moveSpeed;
                    }
                    float dist = Vector3.Distance(transform.position,movePoints[currentMoveIndex].position);
                    
                    transform.LookAt(new Vector3(movePoints[currentMoveIndex].position.x,transform.position.y,movePoints[currentMoveIndex].position.z));
                    if(dist >= 0.01f){
                        transform.position = Vector3.MoveTowards(transform.position,movePoints[currentMoveIndex].position,currentMoveSpeed * Time.deltaTime);
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
}
