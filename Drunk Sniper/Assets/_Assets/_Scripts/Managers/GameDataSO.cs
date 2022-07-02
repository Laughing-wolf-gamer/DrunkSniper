using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;


[CreateAssetMenu(fileName = "Game Data",menuName = "ScriptableObject/Game Data")]
public class GameDataSO : ScriptableObject {
    
    public PlayerSaveData saveData;
    public void SetNewLongestDistance(float currentDistance){
        if(currentDistance > saveData.LongestDistance){
            saveData.LongestDistance = currentDistance;
        }
    }
    public void IncreaseCash(int amount){
        saveData.TotalCash += amount;
    }
    public void SpendCash(int amount){
        saveData.TotalCash -= amount;
        if(saveData.TotalCash <= 0){
            saveData.TotalCash = 0;
        }
    }
    public int GetTotalCash(){
        return saveData.TotalCash;
    }
    public void SetReviewd(){
        saveData.settingsSaveData.isReviewd = true;
    }
    public void SetHasAdsInGame(bool value){
        saveData.settingsSaveData.hasAdsInGame = value;
    }
    public bool GetHasAds(){
        return saveData.settingsSaveData.hasAdsInGame;
    }
    public bool IsRevived(){
        return saveData.settingsSaveData.isReviewd;
    }
    public float GetLongestDistance(){
        return saveData.LongestDistance;
    }
    public void ToggelMusic(bool value){
        saveData.settingsSaveData.isMusicOn = value;
    }
    public void ToggelSound(bool value){
        saveData.settingsSaveData.isSoundOn = value;
    }
    public bool GetMusicState(){
        return saveData.settingsSaveData.isMusicOn;
    }
    public bool GetSoundState(){
        return saveData.settingsSaveData.isSoundOn;
    }



    #region Saving and Loading................

    [ContextMenu("Save")]
    public void Save(){
        string data = JsonUtility.ToJson(saveData,true);
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath,"/",name,"GameData",".dat"));
        formatter.Serialize(file,data);
        file.Close();
    }

    [ContextMenu("Load")]
    public void Load(){
        if(File.Exists((string.Concat(Application.persistentDataPath,"/",name,"GameData",".dat")))){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream Stream = File.Open(string.Concat(Application.persistentDataPath,"/",name,"GameData",".dat"),FileMode.Open);
            JsonUtility.FromJsonOverwrite(formatter.Deserialize(Stream).ToString(),saveData);
            Stream.Close();
        }
    }

    #endregion
}
[System.Serializable]
public class PlayerSaveData{
    public int TotalCash;
    public float LongestDistance;

    [Header("Settings")]
    public SettingsSaveData settingsSaveData;
}
[System.Serializable]
public class SettingsSaveData{
    public bool hasAdsInGame = true;
    public bool isReviewd;
    public bool isMusicOn;
    public bool isSoundOn;
}
