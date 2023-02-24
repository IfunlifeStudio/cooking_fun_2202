using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
public class CustomerAnimationController : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    private const string Idle_Hash = "idle", Discomfort_Hash = "discomfort", Frustrating_Hash = "frustrating";
    private Spine.AnimationState spineAnimationState;
    private Spine.Skeleton skeleton;
    private string currentAnim;
    [SerializeField] private AudioClip appearSfx, happySfx, angrySfx, sadSfx; 
    private float animDuration, animTimeStamp;
    bool isCompleted;
    void Start()
    {
        spineAnimationState = skeletonAnimation.AnimationState;
        spineAnimationState.Start += OnAnimationStart;
        spineAnimationState.Complete += OnAnimationComplete;
        skeleton = skeletonAnimation.Skeleton;
    }
    private void OnAnimationStart(Spine.TrackEntry trackEntry)
    {
        isCompleted = false;
    }
    private void OnAnimationComplete(Spine.TrackEntry trackEntry)
    {
        isCompleted = true;
        if (currentAnim == Idle_Hash)
            if (Random.Range(0, 100) > 75)
            {
                PlayAnimation("idle_nor", false);
            }
    }
    public void PlayAnimation(string anim, bool isLoop)
    {
        currentAnim = anim;
        var playAnim = spineAnimationState.SetAnimation(0, anim, true);
        animTimeStamp = Time.time;
        animDuration = playAnim.TrackTime;
        isCompleted = false;
    }
    public void SetSkeletonState(bool isFlip)
    {
        skeleton.ScaleX = isFlip ? -1 : 1;
    }
    public void UpdateWaitingAnimation(float percentTimeRemain)
    {
        if (Time.time - animTimeStamp > animDuration && isCompleted)
            if (percentTimeRemain > 0.5f)
            {
                if (currentAnim != Idle_Hash)
                {

                    PlayAnimation(Idle_Hash, true);
                }
            }
            else if (percentTimeRemain > 0.25f)
            {
                if (currentAnim != Discomfort_Hash)
                {
                    AudioController.Instance.PlaySfx(angrySfx);
                    PlayAnimation(Discomfort_Hash, true);
                }
            }
            else
            {
                if (currentAnim != Frustrating_Hash)
                {
                    AudioController.Instance.PlaySfx(sadSfx);
                    PlayAnimation(Frustrating_Hash, true);
                }
            }
    }
    public void PlayAppearAudioClip()
    {
        AudioController.Instance.PlaySfx(appearSfx);
    }
    public void PlayHappyAudioClip()
    {
        AudioController.Instance.PlaySfx(happySfx);
    }
}
