using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum HandState { TurnOn, TurnOff }
public class InstructionController : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    public HandState handState = HandState.TurnOff;
    Vector3 originScale;
    // Start is called before the first frame update
    void Start()
    {
        originScale = transform.localScale;
        gameObject.SetActive(false);
    }
    public void Init(Vector2 position,string animName, Transform parent=null)
    {
        Vector3 pos = new Vector3(position.x, position.y, -0.5f);
        gameObject.transform.position = pos;
        gameObject.transform.localScale = originScale;
        handState = HandState.TurnOn;
        skeletonAnimation.AnimationState.SetAnimation(0, animName, true);
        gameObject.SetActive(true);

    }
    public void TurnOff()
    {
        handState = HandState.TurnOff;
        gameObject.transform.parent = null;
        gameObject.SetActive(false);
    }
}
