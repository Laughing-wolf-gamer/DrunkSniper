using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimationController : MonoBehaviour {
    private Animator animator;

    private void Awake(){
        animator = GetComponent<Animator>();
    }

    public void DisableAnimator(){
        animator.enabled = false;
    }

    public void ReduceAnimationSpeed(){
        animator.speed = 0.07f;
    }
    public void BackToNormal(){
        animator.speed = 1f;
    }
    public void Fire(){
        SetDefultAnimation(false);
        animator.SetTrigger("Fire");
    }
    public void Duck(){
        SetDefultAnimation(false);
        animator.SetTrigger("Duck");
    }
    public void SetDefultAnimation(bool walk){
        animator.SetBool("Defult",walk);
    }
    
}
