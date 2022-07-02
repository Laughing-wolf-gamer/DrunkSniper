using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TimeScaleController : MonoBehaviour {
	[SerializeField] private float slowTimeScale;
	
	public static TimeScaleController current;
	
	private void Awake(){
		current = this;
	}
	public void SlowDownTime(){
		Time.timeScale = slowTimeScale;
	}
	
	public void SpeedUpTime(){
		Time.timeScale = 1f;
	}
	
}
