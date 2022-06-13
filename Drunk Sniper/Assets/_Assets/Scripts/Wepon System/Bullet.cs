using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private float moveSpeed;

    private Transform hitTransform;
    private bool isEnemyShot;
    private float shootingForce;
    // private Vector3 direction;
    private Vector3 hitPoint;

    public void Launch(float shootingForce, Transform hitTransform, Vector3 hitPoint){
        // direction = (hitPoint - transform.position).normalized;
        isEnemyShot = false;
        this.hitTransform = hitTransform;
        this.shootingForce = shootingForce;
        this.hitPoint = hitPoint;
    }

    private void Update(){
        float moveDistance = shootingForce * Time.deltaTime;
        CollisionCheck(moveDistance);
        Move(moveDistance);
        Rotate();
        // CheckDistanceToEnemy();
    }
    private void CollisionCheck(float moveDistance){
        RaycastHit hit;
        if(Physics.Raycast(transform.position,transform.forward,out hit,moveDistance,collisionMask,QueryTriggerInteraction.Ignore)){
            EnemyController enemyController = hit.transform.GetComponentInParent<EnemyController>();
            if(enemyController){
                Debug.Log("Colided with" + hit.transform.name);
                BulletTimeController.current.SetChangeCamera();
                ShootEnemy(hit.transform,enemyController);
            }else{
                Debug.Log("Colided with" + hit.transform.name);
                BulletTimeController.current.OnBulletCollide();
            }
        }
    }
    
    private void Move(float shootingSpeed)
    {
        float hori = Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime;
        float vert = Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * hori + Vector3.right * vert);
        transform.Translate(Vector3.forward * shootingSpeed);
    }

    private void CheckDistanceToEnemy()
    {
        float distance = Vector3.Distance(transform.position, hitPoint);
        if(distance <= 0.1 && !isEnemyShot)
        {
            EnemyController enemy = hitTransform.GetComponentInParent<EnemyController>();
            if (enemy)
            {
                ShootEnemy(hitTransform, enemy);
            }
        }
    }
    

    private void Rotate()
    {
        visualTransform.Rotate(Vector3.forward, 1200 * Time.deltaTime, Space.Self);
    }

    private void ShootEnemy(Transform hitTransform, EnemyController enemy)
    {
        // BulletTimeController.current.SetChangeCamera();
        isEnemyShot = true;
        Rigidbody shotRB = hitTransform.GetComponent<Rigidbody>();
        enemy.OnEnemyShot(transform.forward, shotRB);
    }

    public float GetBulletSpeed()
    {
        return shootingForce;
    }

	internal Transform GetHitEnemyTransform()
	{
        return hitTransform;
	}
}