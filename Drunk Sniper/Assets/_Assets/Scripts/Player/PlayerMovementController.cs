using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Baracuda.Monitoring;
using Baracuda.Monitoring.API;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour {
	private const string HORIZONTAL = "Horizontal";
	private const string VERTICAL = "Vertical";
	private const string MOUSE_X = "Mouse X";
	private const string MOUSE_Y = "Mouse Y";

	[SerializeField] Scope scope;
	[SerializeField] private float minMaxYRot,minMaxXRot;
	[SerializeField] private Transform rifleTransformParent;
	[SerializeField] private float moveSpeed;
	[SerializeField] private float rotationSpeed,rotSmoothTime;
	[SerializeField] private float minMouseSensivity;
	[SerializeField] private float maxMouseSensivity;
	[SerializeField] private float mouseSensvityChangeRate;


	private float horizontalInput;
	private float verticalInput;
	[Monitor] private float mouseInputX;
	[Monitor] private float mouseInputY;
	private float currentRotationY;
	private float currentRotationX;
	private float mouseSensivity;
	private Rigidbody rb;

	private Vector2 moveStartPos;
	[Monitor] private Vector2 moveDirection;

	private void Awake()
	{
		MonitoringManager.RegisterTarget(this);
		this.RegisterMonitor();
		rb = GetComponent<Rigidbody>();
	}
	private void OnDestroy(){
		MonitoringManager.UnregisterTarget(this);
		this.UnregisterMonitor();
	}

	private void Start()
	{
		mouseSensivity = maxMouseSensivity;
		currentRotationY = transform.eulerAngles.y;
		currentRotationX = transform.eulerAngles.x;
		// Cursor.lockState = CursorLockMode.Locked;
	}


	// private void FixedUpdate(){
	// 	// HandleTranslation();
	// }
	

	// private void HandleTranslation()
	// {
	// 	var moveVector = new Vector3(horizontalInput, 0f, verticalInput);
	// 	var worldMoveVector = transform.TransformDirection(moveVector);
	// 	worldMoveVector = new Vector3(worldMoveVector.x, 0f, worldMoveVector.z);
	// 	rb.AddForce(worldMoveVector.normalized * Time.deltaTime * moveSpeed, ForceMode.Force);
	// }

	

	private void GetInput(){
		if(SwipeDetection.current.OnPC()){
			horizontalInput = Input.GetAxisRaw(HORIZONTAL);
			verticalInput = Input.GetAxisRaw(VERTICAL);
			mouseInputX = Input.GetAxisRaw(MOUSE_X);
			mouseInputY = Input.GetAxisRaw(MOUSE_Y);
		}else{
			GetTouchInput();
			mouseInputX = moveDirection.x;
			mouseInputY = moveDirection.y;
		}
		mouseSensivity = minMouseSensivity + scope.GetZoomPrc() * Mathf.Abs(minMouseSensivity - maxMouseSensivity);
	}
	private void GetTouchInput(){
		
		if(Input.touchCount > 0){
			Touch touch = Input.GetTouch(0);
			if(!EventSystem.current.IsPointerOverGameObject(touch.fingerId)){
				switch(touch.phase){
					case TouchPhase.Moved:
						moveDirection = touch.deltaPosition;
					break;
					case TouchPhase.Canceled:
					case TouchPhase.Ended:
						moveDirection = Vector3.zero;
					break;
				}
			}
		}
	}

	private void Update(){
		GetInput();
		
		HandleRotation();
	}

	private void HandleRotation()
	{
		float yaw = mouseInputX * Time.deltaTime * rotationSpeed * mouseSensivity;
		currentRotationY += yaw;
		float pitch = mouseInputY * Time.deltaTime * rotationSpeed * mouseSensivity;
		currentRotationX -= pitch;
		currentRotationY = Mathf.Clamp(currentRotationY,-minMaxYRot,minMaxYRot);
		currentRotationX = Mathf.Clamp(currentRotationX, -minMaxXRot,minMaxXRot);
		Quaternion newRotX = Quaternion.Euler(currentRotationX, 0, 0);
		rifleTransformParent.localRotation = Quaternion.Slerp(rifleTransformParent.localRotation,newRotX,rotSmoothTime * Time.deltaTime);
		Quaternion newRotY = Quaternion.Euler(0, currentRotationY, 0);
		transform.localRotation = Quaternion.Slerp(transform.localRotation,newRotY,rotSmoothTime * Time.deltaTime);
	}
}