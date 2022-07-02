using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace GamerWolf.Utils {
    public class ParticalSpawner : MonoBehaviour {
        
        public static ParticalSpawner current{get;private set;}

        private void Awake(){
            current = this;
        }

        public void SpawnEffect(string effectName,Vector3 spawnPoint,Quaternion SpawnRotation){
            GameObject EffectObject = ObjectPoolingManager.current.SpawnFromPool(effectName,spawnPoint,SpawnRotation);
            if(EffectObject.TryGetComponent<ParticleSystem>(out ParticleSystem effect)){
                effect.Play();
            }
        }

    }

}