using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MachineStates
{
    Standby = 0,
    Working = 1,
    Completed = 2,
    Burned = 3
}
public abstract class Machine : MonoBehaviour, IMessageHandle
{
    public int id = 100, foodId;
    [HideInInspector]public float workDuration = 5f, timeRemain = 5f, clickTimeStamp = 0, burnTime;
    protected bool isPlaying = true, canPlayAudio;
    public MachineStates state = MachineStates.Standby;
    [SerializeField] protected AudioClip processSfx, completeSfx, burnSfx;
    [SerializeField] protected MachineAnimationController animationController;
    [SerializeField] protected AudioSource audioSource;
    public abstract bool FillIngredent();
    public abstract bool ServeFood();
    public abstract void TossFood();
    public abstract void BackToWorkingStatus();
    public abstract void SetupSkin();
    public abstract void ShowUpgradeProcessAgain();
    public abstract void ShowUpgradeProcess();
    public virtual void PlayAudio(AudioClip audioClip, bool isLoop = false)
    {
        if (canPlayAudio && audioClip !=null)
        {
            audioSource.Stop();
            audioSource.clip = audioClip;
            audioSource.loop = isLoop;
            audioSource.Play();
        }
    }
    public void Handle(Message message)
    {
        isPlaying = false;
    }
    protected void OnEnable()
    {
        canPlayAudio = AudioController.Instance.SFX;
        MessageManager.Instance.AddSubcriber(CookingMessageType.OnGameEnd, this);
        animationController = GetComponent<MachineAnimationController>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    protected void OnDisable()
    {
        MessageManager.Instance.RemoveSubcriber(CookingMessageType.OnGameEnd, this);
    }
}
