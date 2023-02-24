using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MecanimAnimationController : MachineAnimationController
{
    [SerializeField] private Animator animator;
    [SerializeField] private SkeletonMecanim skeletonMecanim;
    void Start()
    {

    }
    public override void SetMachineSkin(string skinName)
    {
        skeletonMecanim.skeleton.SetSkin(skinName);
    }
    public override void PlayAnimation(string anim, bool isLoop = true)
    {
        currentAnim = anim;
        animator.Play(anim);
        animTimeStamp = Time.time;
    }
    public override void Pause()
    {
        animator.enabled = false;
    }
    public override void Resume()
    {
        animator.enabled = true;
    }
}
