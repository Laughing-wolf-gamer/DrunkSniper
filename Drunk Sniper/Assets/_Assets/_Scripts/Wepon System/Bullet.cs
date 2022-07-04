using UnityEngine;
using GamerWolf.Utils;
using System.Collections;
using Baracuda.Monitoring;
using Baracuda.Monitoring.API;
using System.Collections.Generic;
public class Bullet : MonoBehaviour ,IPooledObject{
    [SerializeField] private float speedResetTime;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private float moveSpeed;

    
    private Transform hitTransform;
    private bool isEnemyShot;
    private float shootingForce;
    private Vector3 hitPoint;
    private float currentSpeed;
    private float vert,hori;
    private float speedSmoothTimeRef;
    private bool isFirstShot;
    private MasterController masterController;
    private void Awake(){
        MonitoringManager.RegisterTarget(this);
        this.RegisterMonitor();
    }
    private void OnDestroy(){
        MonitoringManager.UnregisterTarget(this);
        this.UnregisterMonitor();
    }
    private void Start(){
        masterController = MasterController.current;
        SwipeDetection.current.OnSwipe += (x,y)=>{
            hori = x;
            vert = y;
            currentSpeed = moveSpeed;
        };
    }
    
    public void Launch(float shootingForce, Vector3 hitPoint){
        isEnemyShot = false;
        MasterController.current.SetBulletMoving(true);
        this.shootingForce = shootingForce;
        this.hitPoint = hitPoint;
        
    }

    private void Update(){
        if(SwipeDetection.current.OnPC()){
            if(Input.GetKeyDown(KeyCode.RightArrow)){
                hori = 1f;       
                currentSpeed = moveSpeed;
            }else if(Input.GetKeyDown(KeyCode.LeftArrow)){
                hori = -1f;
                currentSpeed = moveSpeed;
            }else{
                hori = 0f;
            }
            if(Input.GetKeyDown(KeyCode.UpArrow)){
                vert = 1f;
                currentSpeed = moveSpeed;
            }else if(Input.GetKeyDown(KeyCode.DownArrow)){
                vert = -1f;
                currentSpeed = moveSpeed;
            }else{
                vert = 0f;
            }

        }
        float moveDistance = shootingForce * Time.deltaTime;
        CollisionCheck(moveDistance);
        Move(moveDistance);
        Rotate();
    }
    private void CollisionCheck(float moveDistance){
        RaycastHit hit;
        if(!isEnemyShot){    
            if(Physics.Raycast(transform.position,transform.forward,out hit,moveDistance,collisionMask,QueryTriggerInteraction.Ignore)){
                DestroyNow();
                NPCController enemyController = hit.transform.GetComponentInParent<NPCController>();
                if(enemyController != null){
                    hitTransform = hit.transform;
                    BulletTimeController.current.SetChangeCamera();
                    ShootEnemy(hit.transform,enemyController);
                    GameObject bloodObject = ObjectPoolingManager.current.SpawnFromPool("Blood",hit.point,Quaternion.FromToRotation(Vector3.up, transform.forward));
                    if(bloodObject.TryGetComponent<ParticleSystem>(out ParticleSystem bloodEffect)){
                        bloodEffect.Play();
                    }
                }else{
                    GameObject impactObject = ObjectPoolingManager.current.SpawnFromPool("Bullet Impact",hit.point,Quaternion.FromToRotation(Vector3.up, transform.forward));
                    if(impactObject.TryGetComponent<ParticleSystem>(out ParticleSystem bulletImpactEffect)){
                        bulletImpactEffect.Play();
                    }
                    BulletTimeController.current.OnBulletCollide();
                }
                if(!isFirstShot){
                    MasterController.current.InvokeFirstShot();
                }
                Debug.Log("Colided with" + hit.transform.name);
            }
        }
        
    }
    
    private void Move(float shootingSpeed){
        shootingForce += Time.deltaTime / 10f;
        transform.Rotate(Vector3.left * vert * currentSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * hori * currentSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * shootingSpeed);
    }

    private void LateUpdate(){
        if(currentSpeed > 0f){
            currentSpeed = Mathf.SmoothDamp(currentSpeed,0f,ref speedSmoothTimeRef,speedResetTime * Time.deltaTime);
        }
    }

    // private void CheckDistanceToEnemy(){
    //     float distance = Vector3.Distance(transform.position, hitPoint);
    //     if(distance <= 0.1 && !isEnemyShot){
    //         EnemyController enemy = hitTransform.GetComponentInParent<EnemyController>();
    //         if (enemy){
    //             ShootEnemy(hitTransform, enemy);
    //         }
    //     }
    // }
    

    private void Rotate(){
        visualTransform.Rotate(Vector3.forward, 1200 * Time.deltaTime, Space.Self);
    }

    private void ShootEnemy(Transform hitTransform, NPCController enemy){
        // BulletTimeController.current.SetChangeCamera();
        isEnemyShot = true;
        Rigidbody shotRB = hitTransform.GetComponent<Rigidbody>();
        enemy.OnEnemyShot(transform.forward, shotRB);
    }

    public float GetBulletSpeed()
    {
        return shootingForce;
    }

	internal Transform GetHitEnemyTransform(){
        return hitTransform;
	}

    public void OnObjectReuse(){
        isEnemyShot = false;
        gameObject.SetActive(true);
    }

    public void DestroyMySelfWithDelay(float delay = 0){
        Invoke(nameof(DestroyNow),delay);
    }

    public void DestroyNow(){
        CancelInvoke(nameof(DestroyNow));
        MasterController.current.SetBulletMoving(false);
        MasterController.current.InvokeShotComplete();
        gameObject.SetActive(false);
    }
}