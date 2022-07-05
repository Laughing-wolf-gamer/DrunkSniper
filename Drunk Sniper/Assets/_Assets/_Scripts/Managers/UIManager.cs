using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class UIManager : MonoBehaviour {
    
    [SerializeField] private TextMeshProUGUI[] enemyCountText;
    [SerializeField] private TextMeshProUGUI[] hostageCountText;
    [SerializeField] private TextMeshProUGUI playerAmooCountText;

    public static UIManager current;
    private void Awake(){
        current = this;
    }
    public void SetEnemyCount(int totalCount,int currentCount){
        foreach(TextMeshProUGUI text in enemyCountText){
            text.SetText(string.Concat(currentCount," / ",totalCount));
        }
    }
    public void SetHostageCount(int totalCount,int currentCount){
        if(totalCount <= 0){
            foreach(TextMeshProUGUI text in hostageCountText){
                text.gameObject.SetActive(false);
            }
            return;
        }
        foreach(TextMeshProUGUI text in hostageCountText){
            text.SetText(string.Concat(currentCount," / ",totalCount));
        }
    }
    public void SetPlayerAmmoo(int currentAmo,int maxAmoo){
        playerAmooCountText.SetText(string.Concat(currentAmo," / ",maxAmoo));
    }
}
