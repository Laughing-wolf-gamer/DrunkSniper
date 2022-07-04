using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Scope : MonoBehaviour {
	private const string SCOPE = "Scope";
	[SerializeField] private WeponStatsSO weponStats;
	[SerializeField] GameObject postProcessingVolume;
	[SerializeField] private Camera scopeCamera;
	[SerializeField] private float minScopeFOV;
	[SerializeField] private float maxScopeFOV;
	[SerializeField] private float scopeFOVChangeSpeed;
	private float currentFOVSpeed;
	private Animator animator;

	private void Awake() {
		animator = GetComponent<Animator>();
	}

	public void SetScopeFlag(bool isScopeOn) {
		scopeCamera.fieldOfView = weponStats.scopeFov;
		animator.SetBool(SCOPE, isScopeOn);

	}
	public void Relading(bool isReloading){
		animator.SetBool("Reload",isReloading);
	}

	public void EnablePostProcessing(){
		postProcessingVolume.gameObject.SetActive(true);
	}

	public void DisablePostProcessing(){
		postProcessingVolume.gameObject.SetActive(false);
	}

	public float GetZoomPrc(){
		float range = Mathf.Abs(maxScopeFOV - minScopeFOV);
		float currentZoonDelta = scopeCamera.fieldOfView - minScopeFOV;
		return currentZoonDelta / range;

	}
}
