using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
public class MachineAnimationController : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState spineAnimationState;
    protected string currentAnim;
    protected float animDuration, animTimeStamp, originTimeScale;
    // Start is called before the first frame update
    void Start()
    {
        spineAnimationState = skeletonAnimation.AnimationState;
    }
    public virtual void SetMachineSkin(string skinName)
    {
        skeletonAnimation.skeleton.SetSkin(skinName);
    }
    public virtual void PlayAnimation(string anim, bool isLoop)
    {
        currentAnim = anim;
        var playAnim = spineAnimationState.SetAnimation(0, anim, isLoop);
        animTimeStamp = Time.time;
        animDuration = playAnim.TrackTime;
    }
    public virtual void Pause()
    {
        spineAnimationState.TimeScale = 0;
    }
    public virtual void Resume()
    {
        spineAnimationState.TimeScale = originTimeScale;
    }
    public virtual void PlayAnimation(string anim, bool isLoop,float timeScale)
    {
        currentAnim = anim;
        var playAnim = spineAnimationState.SetAnimation(0, anim, isLoop);
        animTimeStamp = Time.time;
        playAnim.TimeScale = timeScale;
    }
}
