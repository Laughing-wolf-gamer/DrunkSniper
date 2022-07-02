using UnityEngine;
using Baracuda.Monitoring;
using Baracuda.Monitoring.API;
using UnityEngine.InputSystem;
public class PlayerInputSystem : MonoBehaviour {
   #region Evens.........

    public delegate void StartTouch(Vector3 postion,float time);
    public event StartTouch onStartTouch;
    public delegate void EndTouch(Vector3 postion,float time);
    public event EndTouch onEndTouch;

    public delegate void OnEscape();
    public event OnEscape onEscapePressed;
    
    #endregion
    private Camera cam;
    
    private PlayerTouch playertouch;

    
    private void Awake(){
    
        playertouch = new PlayerTouch();
        MonitoringManager.RegisterTarget(this);
        this.RegisterMonitor();
    }
    private void OnDestroy(){
        MonitoringManager.UnregisterTarget(this);
        this.UnregisterMonitor();
    }

    private void OnEnable(){
        playertouch.Enable();
    }
    private void OnDisable(){
        playertouch.Disable();
    }
    private void Start (){
        cam = Camera.main;
        playertouch.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        playertouch.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);
        playertouch.Touch.Pause.performed += OnEscapePress;

    }

    
    private void OnEscapePress(InputAction.CallbackContext context){
        if(onEscapePressed != null){
            onEscapePressed();
        }
    }
    private void StartTouchPrimary(InputAction.CallbackContext context){
        if(onStartTouch != null){
            onStartTouch(GetWorldScreenPoint(playertouch.Touch.primaryPostion.ReadValue<Vector2>()),(float)context.startTime);

        }
    }

    private void EndTouchPrimary(InputAction.CallbackContext context){
        if(onEndTouch != null){
            onEndTouch(GetWorldScreenPoint(playertouch.Touch.primaryPostion.ReadValue<Vector2>()),(float)context.time);
        }
    }

    

    private Vector3 GetWorldScreenPoint(Vector3 positon){
        Vector3 mousePos =  new Vector3(positon.x,positon.y,cam.nearClipPlane);
        return mousePos;
    }
}
