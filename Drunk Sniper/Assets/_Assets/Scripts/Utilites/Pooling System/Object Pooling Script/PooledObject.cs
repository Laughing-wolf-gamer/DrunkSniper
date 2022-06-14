using UnityEngine;
using UnityEngine.Events;
namespace GamerWolf.Utils {
    public class PooledObject : MonoBehaviour, IPooledObject{

        [SerializeField] private float lifeTime = 3f;
        [SerializeField] private UnityEvent onObjectUse,onObjectDestroy;
        
        public void DestroyMySelfWithDelay(float delay = 0f){
            onObjectDestroy?.Invoke();
            Invoke(nameof(DestroyNow),delay);
            
        }

        public void OnObjectReuse(){
            onObjectUse?.Invoke();
            DestroyMySelfWithDelay(lifeTime);
        }

        public void DestroyNow(){
            gameObject.SetActive(false);
        }
    }

}