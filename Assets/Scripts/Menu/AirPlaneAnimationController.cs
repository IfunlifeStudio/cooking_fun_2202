using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
public class AirPlaneAnimationController : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic skeletonAnimation;
    private Spine.AnimationState spineAnimationState;
    private string currentAnim;
    private float animDuration, animTimeStamp, originTimeScale;
    private bool isVisible = false;
    private float timeStamp;
    void Start()
    {
        currentAnim = "idle";
        spineAnimationState = skeletonAnimation.AnimationState;
    }
    void Update()
    {
        if (isVisible)
        {
            if (currentAnim == "idle" && Time.time - timeStamp > 5)
            {
                PlayAnimation("on", false);
                timeStamp = Time.time;
            }
            else if (currentAnim == "on" && Time.time - timeStamp > 12)
            {
                PlayAnimation("off", false);
                timeStamp = Time.time;
            }
            else if (currentAnim == "off" && Time.time - timeStamp > 7)
            {
                PlayAnimation("idle", false);
                timeStamp = Time.time;
            }
        }
    }
    public void PlayAnimation(string anim, bool isLoop)
    {
        currentAnim = anim;
        var playAnim = spineAnimationState.SetAnimation(0, anim, isLoop);
        animTimeStamp = Time.time;
        animDuration = playAnim.TrackTime;
    }
    public void Pause()
    {
        spineAnimationState.TimeScale = 0;
    }
    public void Resume()
    {
        spineAnimationState.TimeScale = originTimeScale;
    }
    public void OnScrollValueChange(Vector2 value)
    {
        if (value.x > 0.6)
        {
            isVisible = true;
            timeStamp = Time.time;
        }
        else
            isVisible = false;
    }
}
