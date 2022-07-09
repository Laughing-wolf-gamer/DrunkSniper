using System;
using Cinemachine;
using UnityEngine;
using GamerWolf.Utils;
using System.Collections;
using UnityEngine.Events;
using Baracuda.Monitoring;
using UnityEngine.Rendering;
using Baracuda.Monitoring.API;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class MasterController : MonoBehaviour {
    [SerializeField] private Volume scopeBlurVolume;
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private UnityEvent OnGameStart;
    [SerializeField] private UnityEvent OnGamePlaying,OnGameEnd,OnGamePaused,OnGameResume,OnPlayerWon,OnPlayerLost;

    [SerializeField] private bool isGamePause;
    public bool isGamePlaying{get;private set;}
    private bool isPlayerWon;
    [Monitor] public bool IsBulletMoving{get;private set;}

    private Vector3 lastPosition;
    private float longestDistance;
    private float currentSpeed;
    private float speedFactordelay;
    private float UpdateDelay = 0.1f;
    private int collectedCoins;
    public Action OnPlayerFirstShotComplete;
    public Action OnShotComplete;
    private float blurAmount;
    
    public static MasterController current;
    private void Awake(){
        current = this;
        MonitoringManager.RegisterTarget(this);
    }
    private void OnDestroy(){
        MonitoringManager.UnregisterTarget(this);
    }

    private void Start(){
        Time.timeScale = 1f;
        isGamePlaying = true;
        StartCoroutine(GameStartRoutine());
    }
    
    private void Update(){
        if(isGamePlaying){
            blurAmount = (blurAmount + Time.deltaTime) % 1f;
            scopeBlurVolume.weight = blurAmount;
            if(Input.GetKeyDown(KeyCode.Escape)){
                if(isGamePause){
                    Resume();
                }else{
                    Pause();
                }
            }
        }
    }
    
    private IEnumerator GameStartRoutine(){
        OnGameStart?.Invoke();
        while(!isGamePlaying){
            yield return null;
        }
        yield return StartCoroutine(GamePlayingRoutine());
        
    }
    private IEnumerator GamePlayingRoutine(){
        OnGamePlaying?.Invoke();
        while(isGamePlaying){
            // uIManager.SetCoinsAmount(collectedCoins);
            yield return null;
        }
        gameData.SetNewLongestDistance(longestDistance);
        gameData.IncreaseCash(collectedCoins);
        // uIManager.SetLongestDistance();
        OnGameEnd?.Invoke();
        yield return new WaitForSeconds(1f);
        if(isPlayerWon){
            OnPlayerWon?.Invoke();
        }else{
            OnPlayerLost?.Invoke();
        }
    }
    
    public void GameStart(){
        isGamePlaying = true;
    }
    public void SetGameOver(bool isPlayerWon){
        this.isPlayerWon = isPlayerWon;
        isGamePlaying = false;

    }
    public void RestartGame(){
        if(LevelLoader.instance != null){
            LevelLoader.instance.PlayNextLevel();
        }else{
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
    
    public void Resume(){
        isGamePause = false;
        Time.timeScale = 1f;
        OnGameResume?.Invoke();
    }
    public void Pause(){
        Time.timeScale = 0f;
        OnGamePaused?.Invoke();
        isGamePause = true;
    }
    public void SetCoins(int amount){
        collectedCoins += amount;
        
    }
    public void SetBulletMoving(bool value){
		IsBulletMoving = value;
	}
    public void InvokeFirstShot(){
		OnPlayerFirstShotComplete?.Invoke();
	}
    public void InvokeShotComplete(){
		OnShotComplete?.Invoke();
	}
}
