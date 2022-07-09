using UnityEngine;
using GamerWolf.Utils;
using System.Collections;
using UnityEngine.Events;
using Baracuda.Monitoring;
using Baracuda.Monitoring.API;
using System.Collections.Generic;
public class PlayerShootingController : MonoBehaviour {
    [Header("Events")]
    [SerializeField] private UnityEvent OnShotStart,OnShotEnd;
    [SerializeField] private WeponStatsSO weponStats;
    [SerializeField] private float bulletTimeLiveTime = 22f;
    [SerializeField] private LayerMask shootableLayer;
    [SerializeField] private BulletTimeController bulletTimeController;
    [SerializeField] Transform bulletSpawnTransform;
    
    [SerializeField]private Scope scope;
    [SerializeField] private float minDistanceToPlayAnimation;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Scope currentScope;
    private bool isReloading;
    private bool isScopeEnabled = true;
    private float scrollInput = 0f;
    private bool isShooting = false;
    [Monitor] private bool canShoot = true;
    private bool wasScopeOn;
    private Camera cam;
    private int currentBulletLeft;
    private MasterController masterController;
    private SwipeDetection swipeDetection;
    private void Awake(){
        MonitoringManager.RegisterTarget(this);
    }
    private void OnDestroy(){
        MonitoringManager.UnregisterTarget(this);
    }
    private void Start(){
        canShoot = true;
        currentBulletLeft = weponStats.maxBulletInMag;
        UIManager.current.SetPlayerAmmoo(currentBulletLeft,weponStats.maxBulletInMag);
        isReloading = false;
        swipeDetection = SwipeDetection.current;
        masterController = MasterController.current;
        cam = Camera.main;
        isScopeEnabled = true;
        swipeDetection.CanDetectSwipe(isScopeEnabled);
        HandleScope();
        masterController.OnShotComplete += ()=>{
            if(currentBulletLeft <= 0){
                if(!isReloading){
                    StartCoroutine(ReloadingRoutine());
                }
            }else{
                OnShotEnd?.Invoke();
            }
            Invoke(nameof(RestartCanShoot),weponStats.coolDownTime);
        };
    }
    private void RestartCanShoot(){
        canShoot = true;
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
        if(canShoot){
            if (isShooting){
                if(!isReloading){
                    Shoot();
                }
            }
        }
    }

    private void Shoot(){
        canShoot = false;
        if(currentBulletLeft > 0){
            OnShotStart?.Invoke();
            Vector3 hitPoint = Vector3.zero;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity,shootableLayer)){
                hitPoint = hit.point;
            }else{
                hitPoint = ray.GetPoint(100f);
            }
            GameObject bulletInstance = ObjectPoolingManager.current.SpawnFromPool("Bullet",bulletSpawnTransform.position,bulletSpawnTransform.rotation);
            if(bulletInstance.TryGetComponent<Bullet>(out Bullet bullet)){
                bullet.Launch(weponStats.bulletSpeed, hitPoint);
                bulletTimeController.StartSequence(bullet, hitPoint,bulletTimeLiveTime);
            }
            currentBulletLeft--;
            UIManager.current.SetPlayerAmmoo(currentBulletLeft,weponStats.maxBulletInMag);
        }
    }
    private IEnumerator ReloadingRoutine(){
        playerInput.ToggleInput(false);
        isReloading = true;
        currentScope.Relading(true);
        yield return new WaitForSeconds(weponStats.reloadingTime);
        currentScope.Relading(false);
        isReloading = false;
        playerInput.ToggleInput(true);
        currentBulletLeft = weponStats.maxBulletInMag;
        OnShotEnd?.Invoke();
        UIManager.current.SetPlayerAmmoo(currentBulletLeft,weponStats.maxBulletInMag);
    }

    private void HandleScope(){
        scope.SetScopeFlag(isScopeEnabled);
        wasScopeOn = isScopeEnabled;
    }
    

    private void GetInput() {
        if(canShoot){
            isShooting = playerInput.isTouchUp;
        }
    }
    public void ToggleScope(){
        swipeDetection.CanDetectSwipe(isScopeEnabled);
    }
    
    
}
