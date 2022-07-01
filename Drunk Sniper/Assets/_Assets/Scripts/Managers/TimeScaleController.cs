using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeScaleController : MonoBehaviour {
	[SerializeField] private float slowTimeScale;
	public static TimeScaleController current;
	public bool IsBulletMoving{get;private set;}
	private void Awake(){
		current = this;
	}
	public void SlowDownTime(){
		Time.timeScale = slowTimeScale;
	}
	public void SetBulletMoving(bool value){
		IsBulletMoving = value;
	}
	public void SpeedUpTime(){
		Time.timeScale = 1f;
	}
}
