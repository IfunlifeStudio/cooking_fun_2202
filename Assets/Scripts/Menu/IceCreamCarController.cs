using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IceCreamCarController : UIView
{
    public SkeletonGraphic EffectCar, EffectHuman;
    public Image imgEffectLevel;
    public Sprite[] sprEffectLevel;
    public void Init(int iceCreamLevel)
    {
        UIController.Instance.PushUitoStack(this);
        EffectCar.Skeleton.SetSkin(iceCreamLevel.ToString());//todo: init the object before change skin
        EffectCar.AnimationState.SetAnimation(0, "idle-claim", true);
        EffectHuman.AnimationState.SetAnimation(0, "idle", true);
        imgEffectLevel.sprite = sprEffectLevel[DataController.Instance.IceCreamTimeLevel];
        EffectCar.AnimationState.SetAnimation(0, "click_claim", false);
        EffectHuman.AnimationState.SetAnimation(0, "idle3", true);
        StartCoroutine(DelayHide());
    }
    IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(3f);
        UIController.Instance.PopUiOutStack();
        Destroy(gameObject);
    }
    //public override void OnHide()
    //{
    //    UIController.Instance.PopUiOutStack();
    //    Destroy(gameObject, 0.2f);
    //}
}
