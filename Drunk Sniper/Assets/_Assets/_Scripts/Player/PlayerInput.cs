using GamerWolf;
using UnityEngine;
using System.Collections;
using Baracuda.Monitoring;
using Baracuda.Monitoring.API;
using UnityEngine.EventSystems;
using System.Collections.Generic;
public class PlayerInput : MonoBehaviour {
    
    
    [SerializeField] private bool onPc;
    [SerializeField] private bool inputEnable;
    private const string MOUSE_X = "Mouse X";
	private const string MOUSE_Y = "Mouse Y";
    public bool isTouchDown{get;private set;}
    public bool isTouching{get;private set;}
    public bool isTouchUp{get;private set;}
    private Vector3 touchDelta;
    private void Awake(){
        MonitoringManager.RegisterTarget(this);
    }
    private void OnDestroy(){
        MonitoringManager.UnregisterTarget(this);
    }
    private void Update(){
        if(inputEnable){
            if(onPc){
                PcInput();
            }else{
                MoblieInput();
            }
        }
    }
    private void MoblieInput(){
        if(Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            if(!EventSystem.current.IsPointerOverGameObject(touch.fingerId)){
                isTouchDown = touch.phase == TouchPhase.Began ? true:false;
                isTouching = touch.phase == TouchPhase.Moved ? true:false;
                isTouchUp = touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled ? true:false;
                
                if(isTouching){
                    touchDelta = touch.deltaPosition;
                } 
                if(isTouchUp){
                    touchDelta = Vector3.zero;
                    Invoke(nameof(ResetTouchUp),0.1f);
                }
            }

        }
    }
    private void ResetTouchUp(){
        isTouchUp = false;
    }
    private void PcInput(){
        touchDelta = new Vector3(Input.GetAxisRaw(MOUSE_X),Input.GetAxisRaw(MOUSE_Y),0f);
        if(!EventSystem.current.IsPointerOverGameObject()){
            isTouchDown = Input.GetMouseButtonDown(0);
            isTouching = Input.GetMouseButton(0);
            isTouchUp = Input.GetMouseButtonUp(0);
        }
    }
    public void ToggleInput(bool value){
        inputEnable = value;
    }
    public bool InputAvailable(){
        return inputEnable;
    }
    public Vector3 GetTouchPosition(){
        return touchDelta;
    }
}
