using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
    
    [SerializeField] private List<NPCController> enemeyList;
    [SerializeField] private List<NPCController> hostagesList;

    private bool hasHostage;
    private int totalEnemey;
    private int totalHostage;

    private void Start(){
        if(hostagesList.Count > 0){
            hasHostage = true;
            totalHostage = hostagesList.Count;
        }else{
            hasHostage = false;
        }
        totalEnemey = enemeyList.Count;
        UIManager.current.SetEnemyCount(enemeyList.Count,enemeyList.Count);
        MasterController.current.onShotComplete += ()=>{
            if(hasHostage){
                CheckHostageAmount();
                if(hostagesList.Count <= 0){
                    MasterController.current.SetGameOver(false);
                    return;
                }
            }
            CheckEnemyKilled();
            UIManager.current.SetEnemyCount(totalEnemey,enemeyList.Count);
            UIManager.current.SetHostageCount(totalHostage,hostagesList.Count);
        };
    }
    public void CheckHostageAmount(){
        for (int i = 0; i < hostagesList.Count; i++){
            if(hostagesList[i].IsDead()){
                hostagesList.Remove(hostagesList[i]);
            }
        }
    }
    public void CheckEnemyKilled(){
        for (int i = 0; i < enemeyList.Count; i++){
            if(enemeyList[i].IsDead()){
                enemeyList.Remove(enemeyList[i]);
            }
        }
        if(enemeyList.Count <= 0){
            MasterController.current.SetGameOver(true);
            return;
        }
    }
}
