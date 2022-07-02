using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Baracuda.Monitoring;
using Baracuda.Monitoring.API;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour {
	// private const string HORIZONTAL = "Horizontal";
	// private const string VERTICAL = "Vertical";
	

	[SerializeField] Scope scope;
	[SerializeField] private Vector2 minMaxYRot,minMaxXRot;
	[SerializeField] private Transform rifleTransformParent;
	[SerializeField] private PlayerInput playerInput;
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

	private void Awake() {
		MonitoringManager.RegisterTarget(this);
		this.RegisterMonitor();
		rb = GetComponent<Rigidbody>();
	}
	private void OnDestroy(){
		MonitoringManager.UnregisterTarget(this);
		this.UnregisterMonitor();
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

	private void Update(){
		GetInput();
		
		HandleRotation();
	}

	private void HandleRotation() {
		float yaw = mouseInputX * Time.deltaTime * rotationSpeed * mouseSensivity;
		currentRotationY += yaw;
		float pitch = mouseInputY * Time.deltaTime * rotationSpeed * mouseSensivity;
		currentRotationX -= pitch;
		currentRotationY = Mathf.Clamp(currentRotationY,minMaxYRot.x,minMaxYRot.y);
		currentRotationX = Mathf.Clamp(currentRotationX, minMaxXRot.x,minMaxXRot.y);
		Quaternion newRotX = Quaternion.Euler(currentRotationX, 0, 0);
		rifleTransformParent.localRotation = Quaternion.Slerp(rifleTransformParent.localRotation,newRotX,rotSmoothTime * Time.deltaTime);
		Quaternion newRotY = Quaternion.Euler(0, currentRotationY, 0);
		transform.localRotation = Quaternion.Slerp(transform.localRotation,newRotY,rotSmoothTime * Time.deltaTime);
	}
}