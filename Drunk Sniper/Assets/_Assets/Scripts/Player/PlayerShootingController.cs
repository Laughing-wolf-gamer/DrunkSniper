using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootingController : MonoBehaviour {
    [SerializeField] private LayerMask shootableLayer;
    [SerializeField] BulletTimeController bulletTimeController;
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] Transform bulletSpawnTransform;
    [SerializeField] Scope scope;
    [SerializeField] private float shootingForce;
    [SerializeField] private float minDistanceToPlayAnimation;
    private bool isScopeEnabled = false;
    private float scrollInput = 0f;
    private bool isShooting = false;
    private bool wasScopeOn;
    private Camera cam;

    private void Start(){
        cam = Camera.main;
    }

    private void Update(){
        GetInput();
        HandleScope();
        HandleShooting();
    }

    private void HandleShooting()
    {
       
        if (isShooting){
            Shoot();

        }
    }

    private void Shoot()
    {              
        if (Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out RaycastHit hit,Mathf.Infinity,shootableLayer)){

            Bullet bulletInstance = Instantiate(bulletPrefab, bulletSpawnTransform.position, bulletSpawnTransform.rotation);
            bulletInstance.Launch(shootingForce, hit.collider.transform, hit.point);
            bulletTimeController.StartSequence(bulletInstance, hit.point,22f);
        }
        // {
        //     EnemyController controller = hit.collider.GetComponentInParent<EnemyController>();
        //     Vector3 direction = hit.point - bulletSpawnTransform.position;
        //     if (controller)
        //     {
        //         if (direction.magnitude >= minDistanceToPlayAnimation)
        //         {
        //             controller.StopAnimation();
        //         }
        //         // else
        //         // {
        //         //     controller.OnEnemyShot(direction, hit.collider.GetComponent<Rigidbody>());
        //         // }
        //     }       
        // }
    }

    private void HandleScope()
    {
        scope.ChangeScopeFOV(-scrollInput);
        if (!wasScopeOn)
            scope.ResetScopeFOV();
        scope.SetScopeFlag(isScopeEnabled);
        wasScopeOn = isScopeEnabled;
    }

    private void GetInput()
    {
        if (Input.GetMouseButtonDown(1)){
            isScopeEnabled = !isScopeEnabled;
        }
        isShooting = Input.GetMouseButtonDown(0) && isScopeEnabled;
        scrollInput = Input.mouseScrollDelta.y;
    }
}
