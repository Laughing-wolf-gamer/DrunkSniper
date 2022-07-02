using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
namespace GamerWolf.Utils {
    public enum SceneIndex {
        persistantScene = 0,/*MainMenu = 1,*/
        GameScene = 1,Level_2 = 2,Level_3 = 3,Level_4 = 4 ,Level_5 = 5 ,Level_6 = 6,Level_7 = 7,Level_8 = 8,Level_9 = 11,Level_10 = 12,
        Level_11 = 13,Level_12 = 14,Level_13 = 15,Level_14 = 16,Level_15 = 17,Level_16 = 18,Level_17 = 19,Level_18 = 20,Level_19 = 21,Level_20 = 22
        ,Level_21 = 23,Level_22 = 24,Level_23 = 25,Level_24 = 26, Level_25 = 27, Level_26 = 28, Level_27 = 29,Level_28 = 30,Level_29 = 31,Level_30 = 32,Level_31 = 33,Level_32 = 34,
        Level_33 = 35,Level_34 = 36, Level_35 = 37,Level_36 = 38, Level_37 = 39, Level_38 = 40,Level_39 = 41, Level_40 = 42, Level_41 = 43, Level_42 = 44, Level_43 = 45,
        Level_44 = 46, Leve_45 = 47, Level_46 = 48, Level_47 = 49, Level_48 = 50, Level_49 = 51,Level_50 = 52, Leve_51 = 53, Level_52 = 54, Level_53 = 55, Level_54 = 56, Level_55 = 57,
        Level_56 = 58, Level_57 = 59, Level_58 = 60, Level_59 = 61, Level_60 = 62,Level_61 = 63, Level_62 = 64,Level_63 = 65, Level_64 = 66, Level_65 = 67, Level_66 = 68, Level_67 = 69, Level_68 = 70,
        Level_69 = 71, Level_70 = 72, Level_71 = 73, Level_72 = 74, Level_73 = 75, Level_74 = 76,Level_75 = 77, Level_76 = 78, Level_77 = 79, Level_78 = 80, Level_79 = 81, Level_80 = 82,
        Level_81 = 83, Level_82 = 84, Level_83 = 85, Level_84 = 86, Level_85 = 87, Level_86 = 88,Level_87 = 89, Level_88 = 90, Level_89 = 91, Level_90 = 92, Level_91 = 93, Level_92 = 94,
        Level_93 = 95, Level_94 = 96, Level_95 = 97, Level_96 = 98, Level_97 = 99, Level_98 = 100,Level_99 = 101,Level_100 = 102,CreditMenu = 103
    }

    public class LevelLoader : MonoBehaviour{
        
        [SerializeField] private GameObject loadingScreen;
        // [SerializeField] private List<LevelDataSO> levelList;
        [SerializeField] private Image loadingBar;
        
        [SerializeField] private Sprite[] loadingBarSpriteArray;
        [SerializeField] private SceneIndex currentLevel;
        public static bool showLevel;
        public static LevelLoader instance {get;private set;}
        public static bool isReset{get;private set;}
        private void Awake(){
            #if UNITY_EDITOR
                Debug.unityLogger.logEnabled = true;
            #else
                Application.targetFrameRate = 60;
                Debug.unityLogger.logEnabled = false;
            #endif
            
            if(instance == null){
                instance = this;
            }else{
                Destroy(instance.gameObject);
            }
            
            DontDestroyOnLoad(instance.gameObject);
            if(loadingScreen == null){
                loadingScreen = transform.Find("loadingScreen").gameObject;
            }
        }
        
        
        private void Start(){
            PlayNextLevel();
            
        }
        
        public void PlayNextLevel(){
            // UpdateLevelData();
            SwitchScene(currentLevel);
        }
        // public void ResetGame(){
        //     currentLevel = SceneIndex.Level_1;
        //     PlayNextLevel();
        // }
        /*
        public void SkipLevelTo(int scene){
            for (int i = 0; i < levelList.Count; i++){
                if(scene != 0){
                    if(i == scene){
                        currentLevel = levelList[i - 1].sceneIndex;
                        break;
                    }
                }
                levelList[i].levelSaveData.isCompleted = true;
            }
            SwitchScene(currentLevel);
        }
        */
        // public void UpdateLevelData(){
        //     for (int i = 0; i < levelList.Count; i++){
        //         if(levelList[i].levelSaveData.isCompleted){
        //             continue;
        //         }else{
        //             currentLevel = levelList[i].sceneIndex;
        //             break;
        //         }
                

        //     }
        // }
        // public void MoveToNextLevel(){
        //     if(currentLevel == SceneIndex.Level_100){
        //         SwitchScene(SceneIndex.CreditMenu);
        //         SavingAndLoadingManager.instance.SaveGame();
        //         return;
        //     }
        //     SwitchScene(currentLevel);
        //     SavingAndLoadingManager.instance.SaveGame();
        // }
        
        
        public void SwitchScene(SceneIndex sceneToLoad){
            // CheckForReset();
            StartCoroutine(GetLoadSceneProgress(sceneToLoad));
        }
        
        
        private IEnumerator GetLoadSceneProgress(SceneIndex _sceneToLoad){
            int random = UnityEngine.Random.Range(0,loadingBarSpriteArray.Length);
            loadingBar.sprite = loadingBarSpriteArray[random];
            loadingScreen.SetActive(true);
            
            float totalProgress = 0f;
            loadingBar.fillAmount = totalProgress;
            // float randTime = UnityEngine.Random.Range(0.1f,0.5f);
            yield return new WaitForSeconds(0.1f);
            float randTime = UnityEngine.Random.Range(0.1f,0.5f);
            totalProgress = 0.2f;
            loadingBar.fillAmount = totalProgress;
            yield return new WaitForSeconds(randTime);
            randTime = UnityEngine.Random.Range(0.1f,0.5f);
            totalProgress = 0.5f;
            loadingBar.fillAmount = totalProgress;
            yield return new WaitForSeconds(randTime);
            // randTime = UnityEngine.Random.Range(0.1f,0.5f);
            totalProgress = 0.8f;
            loadingBar.fillAmount = totalProgress;
            yield return new WaitForSeconds(0.5f);
            AsyncOperation operation = SceneManager.LoadSceneAsync((int)_sceneToLoad);
            // totalProgress = 0f;
            while(!operation.isDone && !showLevel){
                // totalProgress = Mathf.Clamp01(operation.progress / 0.9f);
                
                yield return null;
                
            }
            
            loadingScreen.SetActive(false);
            
            
        }
       
    }

}