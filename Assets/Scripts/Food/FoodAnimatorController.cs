using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodAnimatorController : MonoBehaviour
{
    private Animator anim;
    private void OnEnable()
    {
        anim = GetComponent<Animator>();
    }
    private void OnDisable(){
        anim.Play("Idle");//reset to idle state
    }
    public void Play(string animationState)
    {
        anim.Play(animationState);
    }
}
