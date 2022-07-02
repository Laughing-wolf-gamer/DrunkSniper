using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace GamerWolf.Utils{
    public interface IPooledObject{
        void OnObjectReuse();
        void DestroyMySelfWithDelay(float delay = 0f);
        void DestroyNow();
    }
}
