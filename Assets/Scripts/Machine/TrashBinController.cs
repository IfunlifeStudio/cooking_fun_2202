using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using MoreMountains.NiceVibrations;

public class TrashBinController : MonoBehaviour
{
    private GameController gameController;
    [SerializeField] private AudioClip binOpenSound;
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState spineAnimationState;
    private float tossTimeStamp;
    private void Start()
    {
        spineAnimationState = skeletonAnimation.AnimationState;
        gameController = FindObjectOfType<GameController>();
        tossTimeStamp = Time.time;
    }
    public void TossTrash(int foodId)
    {
        if (LevelDataController.Instance.currentLevel.ConditionType() == 2 && FindObjectOfType<GameController>().isPlaying && Time.time - tossTimeStamp > 0.25f)
        {
            tossTimeStamp = Time.time;
            AudioController.Instance.Vibrate();
            MessageManager.Instance.SendMessage(
                    new Message(CookingMessageType.OnConditionViolate,
                    new object[] { LevelDataController.Instance.currentLevel.ConditionType(), transform.position, this }));
        }
        AudioController.Instance.PlaySfx(binOpenSound);
        gameController.OnPlayerThrowFood(foodId);
        spineAnimationState.SetAnimation(0, "on", false);
    }
}
