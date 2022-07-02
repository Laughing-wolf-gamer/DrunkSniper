using UnityEngine;
using GamerWolf.Utils;
using System.Collections;
using System.Collections.Generic;

public class PlayerShootingController : MonoBehaviour {
    [SerializeField] private float bulletTimeLiveTime = 22f;
    [SerializeField] private LayerMask shootableLayer;
    [SerializeField] private BulletTimeController bulletTimeController;
    [SerializeField] Transform bulletSpawnTransform;
    [SerializeField] Scope scope;
    [SerializeField] private float shootingForce;
    [SerializeField] private float minDistanceToPlayAnimation;
    private bool isScopeEnabled = true;
    private float scrollInput = 0f;
    private bool isShooting = false;
    private bool wasScopeOn;
    private Camera cam;
    [SerializeField] private PlayerInput playerInput;
    private MasterController masterController;
    private SwipeDetection swipeDetection;
    private void Start(){
        swipeDetection = SwipeDetection.current;
        masterController = MasterController.current;
        cam = Camera.main;
        isScopeEnabled = true;
        swipeDetection.CanDetectSwipe(isScopeEnabled);
        HandleScope();
    }

    private void Update(){
        if(masterController.isGamePlaying){
            if(!masterController.IsBulletMoving){
                GetInput();
                HandleShooting();
            }
        }
    }

    private void HandleShooting(){
        if (isShooting){
            Shoot();
        }
    }

    private void Shoot(){              
        Vector3 hitPoint = Vector3.zero;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity,shootableLayer)){
            hitPoint = hit.point;
        }else{
            hitPoint = ray.GetPoint(100f);
        }
        GameObject bulletInstance = ObjectPoolingManager.current.SpawnFromPool("Bullet",bulletSpawnTransform.position,bulletSpawnTransform.rotation);
        if(bulletInstance.TryGetComponent<Bullet>(out Bullet bullet)){
            bullet.Launch(shootingForce, hitPoint);
            bulletTimeController.StartSequence(bullet, hitPoint,bulletTimeLiveTime);
        }
    }

    private void HandleScope(){
        scope.SetScopeFlag(isScopeEnabled);
        wasScopeOn = isScopeEnabled;
    }
    

    private void GetInput() {
        isShooting = playerInput.isTouchUp && isScopeEnabled;
    }
    public void ToggleScope(){
        // isScopeEnabled = !isScopeEnabled;
        SwipeDetection.current.CanDetectSwipe(isScopeEnabled);
    }
    public void Fire(){
        if(isScopeEnabled){
            Shoot();
        }
        
    }
}
