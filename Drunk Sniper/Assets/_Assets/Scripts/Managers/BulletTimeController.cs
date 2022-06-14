using System;
using Cinemachine;
using UnityEngine;
using System.Linq;
using GamerWolf.Utils;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TimeScaleController))]
public class BulletTimeController : MonoBehaviour
{
	[Serializable]
	public class TargetTrackingSetup{
		// public CinemachinePathController avaliableTrack;
		public string avaliableTrack;
		public string avaliableDolly;
	}

	[Serializable]
	public class BulletTrackingSetup : TargetTrackingSetup
	{
		public float minDistance;
		public float maxDistance;
	}

	[SerializeField] private GameObject canvas;
	[SerializeField] private CinemachineBrain cameraBrain;
	[SerializeField] private BulletTrackingSetupSO[] bulletTrackingSetupSos;
	[SerializeField] private TargetTrackingSetupSO[] enemyTrackingSetUpSos;
	// [SerializeField] private BulletTrackingSetup[] bulletTackingSetup;
	// [SerializeField] private TargetTrackingSetup[] enemyTrackingSetup;
	[SerializeField] private PlayerShootingController shootingController;
	[SerializeField] private float distanceToChangeCamera;
	[SerializeField] private float finishingCameraDuration;

	private TimeScaleController timeScaleController;
	private CinemachineSmoothPath trackInstance;
	private CameraCartController dollyInstance;
	private Bullet activeBullet;
	private Vector3 targetPosition;
	private List<TargetTrackingSetupSO> clearTracks = new List<TargetTrackingSetupSO>();
	private bool isLastCameraActive = false;
	private float maxTimeToLiveDolly;
	public static BulletTimeController current;
	


	private void Awake(){
		current = this;
		timeScaleController = GetComponent<TimeScaleController>();
	}
	
	internal void StartSequence(Bullet activeBullet, Vector3 targetPosition,float maxTimeToLiveDolly)
	{
		canChangeCamera = false;
		ResetVariables();
		canCountDown = true;
		isEndedSequence = false;
		float distanceToTarget = Vector3.Distance(activeBullet.transform.position, targetPosition);
		var setupsInRange = bulletTrackingSetupSos.Where(s => distanceToTarget > s.minDistance && distanceToTarget < s.maxDistance).ToArray();
		var selectedTrackingSetup = SelectTrackingSetup(activeBullet.transform,setupsInRange,activeBullet.transform.rotation);
		if (selectedTrackingSetup == null)
			return;
		this.activeBullet = activeBullet;
		this.targetPosition = targetPosition;
		this.maxTimeToLiveDolly = maxTimeToLiveDolly;
		
		GameObject newTrack = ObjectPoolingManager.current.SpawnFromPool(selectedTrackingSetup.avaliableTrack);
		if(newTrack.TryGetComponent<CinemachinePathController>(out CinemachinePathController avialbelPath)){
			CreateBulletPath(activeBullet.transform, avialbelPath);
		}
		CreateDolly(selectedTrackingSetup);
		cameraBrain.gameObject.SetActive(true);
		shootingController.gameObject.SetActive(false);
		canvas.gameObject.SetActive(false);
		float speed = CalculateDollySpeed();
		dollyInstance.InitDolly(trackInstance, activeBullet.transform, speed);
	}

	private void CreateDolly(TargetTrackingSetupSO setup)
	{
		var selectedDolly = setup.avaliableDolly;
		var newObjct = ObjectPoolingManager.current.SpawnFromPool(selectedDolly);
		if(newObjct.TryGetComponent<CameraCartController>(out CameraCartController cart)){
			dollyInstance = cart;
		}
		// dollyInstance = Instantiate(selectedDolly);
	}

	private void CreateBulletPath(Transform bulletTransform, CinemachinePathController selectedPath)
	{
		trackInstance = Instantiate(selectedPath.path, bulletTransform);
		trackInstance.transform.localPosition = selectedPath.transform.position;
		trackInstance.transform.localRotation = selectedPath.transform.rotation;
	}

	private float CalculateDollySpeed()
	{
		if (trackInstance == null || activeBullet == null)
			return 0f;

		float distanceToTarget = Vector3.Distance(activeBullet.transform.position, targetPosition);
		float speed = activeBullet.GetBulletSpeed();
		float pathDistance = trackInstance.PathLength;
		return pathDistance * speed / distanceToTarget;
	}


	private void CreateEnemyPath(Transform enemytransform, Transform bulletTransform, CinemachinePathController selectedPath)
	{
		Quaternion rotation = Quaternion.Euler(Vector3.up * bulletTransform.root.eulerAngles.y);
		trackInstance = Instantiate(selectedPath.path, enemytransform.position, rotation);
	}

	private TargetTrackingSetupSO SelectTrackingSetup(Transform trans, TargetTrackingSetupSO[] setups, Quaternion orientation)
	{
		clearTracks.Clear();
		for (int i = 0; i < setups.Length; i++)
		{
			GameObject newObject = ObjectPoolingManager.current.SpawnFromPool(setups[i].avaliableTrack);
			if(newObject.TryGetComponent<CinemachinePathController>(out CinemachinePathController newPath)){

				if (CheckIfPathIsClear(newPath, trans, orientation)){
					clearTracks.Add(setups[i]);
				}
			}
		}
		if (clearTracks.Count == 0)
			return null;
		return clearTracks[UnityEngine.Random.Range(0, clearTracks.Count)];
	}

	private bool CheckIfPathIsClear(CinemachinePathController path, Transform trans, Quaternion orientation)
	{
		return path.CheckIfPathISClear(trans, Vector3.Distance(trans.position, targetPosition), orientation);
	}
	private bool isEndedSequence;
	private bool canCountDown;
	private bool canChangeCamera;
	public void SetChangeCamera(){
		canChangeCamera = true;
		// ChangeCamera();
	}
	public void OnBulletCollide(){
		canChangeCamera = false;
		canCountDown = false;
		isEndedSequence = true;
		StartCoroutine(FinishSequence(0f));
		// maxTimeToLiveDolly = -1f;
		// if(!isEndedSequence){
		// 	StartCoroutine(FinishSequence());
		// 	isEndedSequence = true;
		// }
		// cameraBrain.gameObject.SetActive(false);
		// shootingController.gameObject.SetActive(true);
		// canvas.gameObject.SetActive(true);
		// timeScaleController.SpeedUpTime();
		// DestroyCinemachineSetup();
		// Destroy(activeBullet.gameObject);
		// ResetVariables();
	}
	private void Update()
	{
		if (!activeBullet)
			return;
		if(canCountDown){

			if(maxTimeToLiveDolly > 0f){
				maxTimeToLiveDolly -= Time.deltaTime;
			}else{
				if(!isEndedSequence){
					canChangeCamera = false;
					canCountDown = false;
					StartCoroutine(FinishSequence(0f));
					isEndedSequence = true;
				}
			}
		}

		if(canChangeCamera){
			ChangeCamera();
		}
		// if (CheckIfBulletIsNearTarget()){
		// }
	}

	private bool CheckIfBulletIsNearTarget()
	{
		return Vector3.Distance(activeBullet.transform.position, targetPosition) < distanceToChangeCamera;
	}
	

	private void ChangeCamera()
	{
		
		if (isLastCameraActive)
			return;
		canCountDown = false;
		isLastCameraActive = true;
		DestroyCinemachineSetup();
		Transform hitTransform = activeBullet.GetHitEnemyTransform();
		if (hitTransform)
		{
			Quaternion rotation = Quaternion.Euler(Vector3.up * activeBullet.transform.rotation.eulerAngles.y);
			var selectedTrackingSetup = SelectTrackingSetup(hitTransform, enemyTrackingSetUpSos, rotation);
			if (selectedTrackingSetup != null){
				GameObject newObject = ObjectPoolingManager.current.SpawnFromPool(selectedTrackingSetup.avaliableTrack);
				CinemachinePathController newPath;
				if(newObject.TryGetComponent<CinemachinePathController>(out newPath)){
					CreateEnemyPath(hitTransform, activeBullet.transform, newPath);

				}
				CreateDolly(selectedTrackingSetup);
				dollyInstance.InitDolly(trackInstance, hitTransform.transform);
				timeScaleController.SlowDownTime();
			}
		}
		StartCoroutine(FinishSequence(finishingCameraDuration));
	}

	private void DestroyCinemachineSetup()
	{
		if(trackInstance != null){
			Destroy(trackInstance.gameObject);
		}
		if(dollyInstance != null){
			Destroy(dollyInstance.gameObject);
		}
	}

	private IEnumerator FinishSequence(float _finishingCameraDuration){
		yield return new WaitForSecondsRealtime(_finishingCameraDuration);
		canChangeCamera = false;
		cameraBrain.gameObject.SetActive(false);
		shootingController.gameObject.SetActive(true);
		canvas.gameObject.SetActive(true);
		timeScaleController.SpeedUpTime();
		DestroyCinemachineSetup();
		if(activeBullet != null){
			activeBullet.DestroyNow();
		}else{
			Debug.LogError("No Bullet Found to Destory");
		}
		ResetVariables();
	}

	private void ResetVariables()
	{
		isLastCameraActive = false;
		trackInstance = null;
		dollyInstance = null;
		activeBullet = null;
		clearTracks.Clear();
		targetPosition = Vector3.zero;
	}
}