using UnityEngine;
using System.Collections.Generic;


namespace GamerWolf.Utils{

    
    [DefaultExecutionOrder(-1)]
    public class ObjectPoolingManager : MonoBehaviour{

        #region Singelton.
        public static ObjectPoolingManager current{get;private set;}

        private void Awake(){
            if(current == null){
                current = this;
            }else{
                Debug.LogError(nameof(ObjectPoolingManager) + " is Found in the Scene");
                Destroy(current.gameObject);
            }
            Init();
        }

        #endregion

        // [System.Serializable]
        // public class Pool{
            
        // }

        [SerializeField] private List<PoolSO> pools = new List<PoolSO>();
        private Dictionary<string , Queue<GameObject>> poolDictionary;
        private GameObject parentObj;

        private void Init(){
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            CreatePool();
        }

        public void CreatePool(){
            foreach(PoolSO pool in pools){
                parentObj = new GameObject(pool.tag + " Pooled Object Parent");
                parentObj.transform.SetParent(transform);
                Queue<GameObject> objectPool = new Queue<GameObject>();
                for(int i = 0; i < pool.size; i++){
                    if(pool.prefabs != null){
                        GameObject obj = Instantiate(pool.prefabs) as GameObject;
                        obj.SetActive(false);
                        obj.transform.SetParent(parentObj.transform);
                        obj.name = string.Concat(pool.tag ," ",obj.transform.GetSiblingIndex().ToString());
                        objectPool.Enqueue(obj);
                    }
                    
                    
                }
                poolDictionary.Add(pool.tag,objectPool);
            }
        }
        private string GetRandomTag(){
            int randomNum = Random.Range(0,pools.Count);
            return pools[randomNum].tag;
        }

        public GameObject SpawnRandomFromPool(Vector3 _spawnPoint,Quaternion _rotations){
            return SpawnFromPool(GetRandomTag(),_spawnPoint,_rotations);
        }
        public GameObject SpawnFromPool(string tag){
            return SpawnFromPool(tag,Vector3.zero,Quaternion.identity);
        }
        public GameObject SpawnFromPool(string tag,Vector3 _spawnPosition,Quaternion _rotation,Transform parent){
            GameObject spawnObject = SpawnFromPool(tag,_spawnPosition,_rotation);
            spawnObject.transform.SetParent(parent);
            return spawnObject;
        }
        
        public GameObject SpawnFromPool(string tag,Vector3 _spawnPosition,Quaternion _rotation){
            if(!poolDictionary.ContainsKey(tag)){
                Debug.Log("Pool With the " + tag + " is not Found");
                return null;
            }
            GameObject objectToSpawn = poolDictionary[tag].Dequeue();
            objectToSpawn.transform.position = _spawnPosition;
            objectToSpawn.transform.rotation = _rotation;
            objectToSpawn.SetActive(true);
            IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();
            if(pooledObject != null){
                pooledObject.OnObjectReuse();
            }
            
            poolDictionary[tag].Enqueue(objectToSpawn);
            return objectToSpawn;
        }
    }

}
