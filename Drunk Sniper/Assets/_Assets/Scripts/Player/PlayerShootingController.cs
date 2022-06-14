using UnityEngine;
using GamerWolf.Utils;
using System.Collections;
using System.Collections.Generic;

public class PlayerShootingController : MonoBehaviour {
    [SerializeField] private LayerMask shootableLayer;
    [SerializeField] BulletTimeController bulletTimeController;
    // [SerializeField] Bullet bulletPrefab;
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

    private void HandleShooting(){
        if(SwipeDetection.current.OnPC()){
            if (isShooting){
                Shoot();
            }
        }
    }

    private void Shoot(){              
        if (Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out RaycastHit hit,Mathf.Infinity,shootableLayer)){
            GameObject bulletInstance = ObjectPoolingManager.current.SpawnFromPool("Bullet",bulletSpawnTransform.position,bulletSpawnTransform.rotation);
            if(bulletInstance.TryGetComponent<Bullet>(out Bullet bullet)){
                bullet.Launch(shootingForce, hit.point);
                bulletTimeController.StartSequence(bullet, hit.point,22f);
            }
        }
    }

    private void HandleScope(){
        scope.ChangeScopeFOV(-scrollInput);
        if (!wasScopeOn)
            scope.ResetScopeFOV();
        scope.SetScopeFlag(isScopeEnabled);
        wasScopeOn = isScopeEnabled;
        // if(SwipeDetection.current.OnPC()){
        // }
    }
    public void ChangeScopeSize(float amount){
        scope.ChangeView(amount);
        if (!wasScopeOn)
            scope.ResetScopeFOV();
        scope.SetScopeFlag(isScopeEnabled);
        wasScopeOn = isScopeEnabled;
    }

    private void GetInput() {
        if(SwipeDetection.current.OnPC()){
            if(Input.GetMouseButtonDown(1)){
                ToggleScope();  
            }
            scrollInput = Input.mouseScrollDelta.y;
            isShooting = Input.GetMouseButtonDown(0) && isScopeEnabled;
        }
    }
    public void ToggleScope(){
        isScopeEnabled = !isScopeEnabled;
        SwipeDetection.current.CanDetectSwipe(isScopeEnabled);
    }
    public void Fire(){
        if(isScopeEnabled){
            Shoot();
        }
        
    }
}
