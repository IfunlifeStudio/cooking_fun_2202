using DigitalRuby.Tween;
using Spine.Unity;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullPiggyBankPanelController : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 PiggyIcon;
    [SerializeField] private SkeletonGraphic pigAnim;
    [SerializeField] private Image progressImg;
    [SerializeField] private GameObject ConfettiBlast;
    [SerializeField] private AudioClip IncreaseGoldAudio;
    System.Action Callback;
    public void Init(Vector3 piggyIcon, System.Action callback)
    {
        PiggyIcon = piggyIcon;
        StartCoroutine(PlayAnim());
        Callback = callback;
    }
    IEnumerator PlayAnim()
    {
        pigAnim.AnimationState.SetAnimation(1, "jumpin_x", false);
        yield return new WaitForSeconds(1.3f);
        pigAnim.AnimationState.SetAnimation(1, "suckindiamond_x", false);
        yield return new WaitForSeconds(0.3f);
        progressImg.transform.parent.gameObject.SetActive(true);
        float tmp = 0;
        System.Action<ITween<Vector3>> updateProgress = (t) =>
        {
            progressImg.rectTransform.sizeDelta = t.CurrentValue;
        };
        TweenFactory.Tween("PiggyIcon" + Time.time, progressImg.rectTransform.sizeDelta, new Vector2(275, 42), 2, TweenScaleFunctions.QuinticEaseOut, updateProgress);
        var delay = new WaitForSeconds(0.04f);
        while (tmp < 1.2f)
        {
            tmp += 0.05f;
            AudioController.Instance.PlaySfx(IncreaseGoldAudio);
            yield return delay;
        }
        yield return new WaitForSeconds(0.3f);
        ConfettiBlast.SetActive(true);
        pigAnim.AnimationState.SetAnimation(1, "jumpin_loop", false);
        progressImg.transform.parent.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        ConfettiBlast.SetActive(false);
        StartCoroutine(IMove(pigAnim.gameObject));
    }
    public IEnumerator IMove(GameObject gameObject)
    {
        Vector3 target = PiggyIcon -new Vector3(0, 0.25f, 0);//generate a random pos
        Vector3 scaletarget = new Vector3(0.3f, 0.3f, 1);
        System.Action<ITween<Vector3>> updatePiggyPos = (t) =>
        {
            gameObject.transform.position = t.CurrentValue;
        };
        System.Action<ITween<Vector3>> completedPiggyMovement = (t) =>
        {
            Callback.Invoke();
            Destroy(this.gameObject);
        };
        TweenFactory.Tween("PiggyIcon" + Time.time, gameObject.transform.position, target, 0.75f, TweenScaleFunctions.QuinticEaseOut, updatePiggyPos).ContinueWith(new Vector3Tween().Setup(target, new Vector3(0, 0, 0), 0.25f, TweenScaleFunctions.QuadraticEaseIn, null, completedPiggyMovement));
        yield return null;
        System.Action<ITween<Vector3>> updatePiggyScale = (t) =>
        {
            gameObject.transform.localScale = t.CurrentValue;
        };
        TweenFactory.Tween("PiggyIcon" + Time.time, gameObject.transform.localScale, scaletarget, 0.75f, TweenScaleFunctions.QuinticEaseOut, updatePiggyScale);

        //while (Vector2.Distance(gameObject.transform.position, pos) > 1)
        //{
        //    gameObject.transform.position = CalculateQuadraticBezierPoint(time, tempPos, midlePos, pos);
        //    time += Time.deltaTime * speed * 1.2f;
        //    yield return null;
        //}
        //DG.Tweening.DOVirtual.DelayedCall(0.05f, () =>
        //{
        //    Callback.Invoke();
        //    Destroy(gameObject);
        //});
    }
    public Vector3 CalculateQuadraticBezierPoint(float t1, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t1;
        float tt = t1 * t1;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t1 * p1;
        p += tt * p2;
        return p;
    }
}
