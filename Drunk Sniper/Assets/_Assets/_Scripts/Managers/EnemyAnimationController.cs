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


    public void Fire(){
        setDefultAnimation(false);
        animator.SetTrigger("Fire");
    }
    public void Duck(){
        setDefultAnimation(false);
        animator.SetTrigger("Duck");
    }
    public void setDefultAnimation(bool walk){
        animator.SetBool("Defult",walk);
    }
    
}
