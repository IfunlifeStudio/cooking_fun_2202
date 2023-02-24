using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
using UnityEngine.Events;

public class DishPropController : MonoBehaviour
{
    private int foodId = 0;
    public void Spawn(int foodId, Vector3 position, Transform target, UnityAction action = null)
    {
        GameObject go = Instantiate(gameObject);
        go.GetComponent<DishPropController>().Init(go,foodId, position, target,action);
    }
    public void Init(GameObject go,int foodId, Vector3 position, Transform target = null,UnityAction action=null)
    {
        gameObject.SetActive(true);
        this.foodId = foodId;
        transform.position = position;
        if (target != null)
        {
            StartCoroutine(IMove(go, target.transform.position, 1f,action));
        }
        else
            Invoke("Dispose", 1);
    }
    public void Dispose()
    {
        //FindObjectOfType<GameController>().OnFoodServed(foodId);
        PropFactory.Instance.DisposeDish(this);
        gameObject.SetActive(false);
    }
    public IEnumerator IMove(GameObject gameObject, Vector2 pos, float speed,UnityAction action = null)
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
