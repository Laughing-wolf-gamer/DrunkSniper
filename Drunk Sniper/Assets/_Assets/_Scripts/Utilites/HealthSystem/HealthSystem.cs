using System;
using UnityEngine;
using System.Collections;
using Baracuda.Monitoring;
using Baracuda.Monitoring.API;
using System.Collections.Generic;
public class OnDamegeEventArgs : EventArgs{
    public float healthAmountLeft;
}
public class HealthSystem : MonoBehaviour,IDamageable {
    [SerializeField] protected float maxHealth;
    [Monitor] protected float currentHealth;
    [Monitor] protected bool isDead;


    public event EventHandler<OnDamegeEventArgs> OnTakeDamage;
    public Action OnPlayerDead;
    protected virtual void Awake(){
        MonitoringManager.RegisterTarget(this);
        isDead = false;
        currentHealth = maxHealth;
    }
    protected virtual void OnDestroy(){
        MonitoringManager.UnregisterTarget(this);
    }
    public void TakeDamage(float amount){
        currentHealth -= amount;
        OnTakeDamage?.Invoke(this,new OnDamegeEventArgs{healthAmountLeft = currentHealth});
        if(currentHealth <= 0f &&  !isDead){
            Die();
            currentHealth = 0f;
        }
    }
    private float GetHealthNormalized(){
        return currentHealth / maxHealth;
    }
    public void Die(){
        isDead = true;
        OnPlayerDead?.Invoke();
        // Player Dead;
        // MasterController.current.SetGameOver(false);
    }




}
