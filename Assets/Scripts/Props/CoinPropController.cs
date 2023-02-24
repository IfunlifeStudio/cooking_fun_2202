using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
using UnityEngine.Events;

public class CoinPropController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer doubleCoinSprite;
    [SerializeField] private Sprite[] sprites;
    private int value = 0;
    public void Spawn(int value, Vector3 position, Transform target = null, UnityAction action = null)
    {
        GameObject go = Instantiate(gameObject);
        go.GetComponent<CoinPropController>().Init(go, value, position, target,action);
    }
    public void Init(GameObject go, int value, Vector3 position, Transform target = null, UnityAction action = null)
    {
        if (LevelDataController.Instance.CheckItemActive((int)ItemType.Double_Coin))
            doubleCoinSprite.sprite = sprites[0];
        else
            doubleCoinSprite.sprite = sprites[1];
        gameObject.SetActive(true);
        this.value = value;
        transform.position = position;
        PlayCoinDropAnim(go, target,action);
    }
    private void PlayCoinDropAnim(GameObject go, Transform target, UnityAction action = null)
    {
        if (target != null)
            StartCoroutine(IMove(go, target.transform.position, 1f,action));
        else
            Invoke("Dispose", 1);
    }
    public void Dispose()
    {
        PropFactory.Instance.DisposeCoin(this);
        gameObject.SetActive(false);
    }
    public IEnumerator IMove(GameObject gameObject, Vector2 pos, float speed, UnityAction action = null)
    {
        float time = 0;
        Vector2 midlePos = new Vector2((gameObject.transform.position.x + pos.x) / 2f + (Random.Range(-6f, 0f)), (gameObject.transform.position.y + pos.y) / 3f);
        Vector2 tempPos = gameObject.transform.position;
        yield return new WaitForSeconds(0.2f);
        while (Vector2.Distance(gameObject.transform.position, pos) > 0.3f)
        {
            gameObject.transform.position = CalculateQuadraticBezierPoint(time, tempPos, midlePos, pos);
            time += Time.deltaTime * speed * 3;
            yield return null;
        }
        if (action != null)
            action.Invoke();
        DG.Tweening.DOVirtual.DelayedCall(0.05f, () =>
        {
            Dispose();
        });
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
