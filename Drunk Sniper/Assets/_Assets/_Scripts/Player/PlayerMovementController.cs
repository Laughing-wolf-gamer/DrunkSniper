using UnityEngine;
using System.Collections;
using Baracuda.Monitoring;
using Baracuda.Monitoring.API;
using System.Collections.Generic;
public class PlayerMovementController : HealthSystem {
	[SerializeField] Scope scope;
	[SerializeField] private PlayerLevelData playerLevelData;
	[SerializeField] private Transform rifleTransformParent;
	[SerializeField] private PlayerInput playerInput;
	[SerializeField] private float rotationSpeed,rotSmoothTime;
	[SerializeField] private float minMouseSensivity;
	[SerializeField] private float maxMouseSensivity;
	[SerializeField] private float mouseSensvityChangeRate;


	private float horizontalInput;
	private float verticalInput;
	private float mouseInputX;
	private float mouseInputY;
	
	private float currentRotationY;
	private float currentRotationX;
	private float mouseSensivity;
	private Rigidbody rb;

	private Vector2 moveStartPos;
	private Vector2 moveDirection;

	protected override void Awake() {
		base.Awake();
		MonitoringManager.RegisterTarget(this);
		rb = GetComponent<Rigidbody>();
	}
	protected override void OnDestroy(){
		base.OnDestroy();
		MonitoringManager.UnregisterTarget(this);
	}

	private void Start() {
		mouseSensivity = maxMouseSensivity;
		currentRotationY = transform.eulerAngles.y;
		currentRotationX = transform.eulerAngles.x;
	}
	
	private void GetInput(){
		mouseInputX = playerInput.GetTouchPosition().x;
		mouseInputY = playerInput.GetTouchPosition().y;
		mouseSensivity = minMouseSensivity + scope.GetZoomPrc() * Mathf.Abs(minMouseSensivity - maxMouseSensivity);
	}

	protected virtual void Update(){
		if(MasterController.current.isGamePlaying){
			if(!MasterController.current.IsBulletMoving){
				GetInput();
				HandleRotation();
			}else{
				mouseSensivity = 0f;
			}
		}
	}

	private void HandleRotation() {
		float yaw = mouseInputX * Time.deltaTime * rotationSpeed * mouseSensivity;
		currentRotationY += yaw;
		float pitch = mouseInputY * Time.deltaTime * rotationSpeed * mouseSensivity;
		currentRotationX -= pitch;
		currentRotationY = Mathf.Clamp(currentRotationY,playerLevelData.yMinLook,playerLevelData.yMaxLook);
		currentRotationX = Mathf.Clamp(currentRotationX, playerLevelData.xMinLook,playerLevelData.xMaxLook);
		Quaternion newRotX = Quaternion.Euler(currentRotationX,0f, 0);
		rifleTransformParent.localRotation = Quaternion.Slerp(rifleTransformParent.localRotation,newRotX,rotSmoothTime * Time.deltaTime);
		Quaternion newRotY = Quaternion.Euler(transform.localRotation.x, currentRotationY, transform.localRotation.z);
		transform.localRotation = Quaternion.Slerp(transform.localRotation,newRotY,rotSmoothTime * Time.deltaTime);
	}
}