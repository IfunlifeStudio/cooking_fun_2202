using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodTossTutorialPanel : MonoBehaviour
{
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] protected Transform mask, focusBtn;
    private float timeStamp;
    private Machine burnMachine;
    public void Spawn(Machine burnMachine)
    {
        GameObject go = Instantiate(gameObject);
        go.GetComponent<FoodTossTutorialPanel>().Init(burnMachine);
    }
    public void Init(Machine burnMachine)
    {
        AudioController.Instance.PlaySfx(popUpClip);
        this.burnMachine = burnMachine;
        mask.position = new Vector3(burnMachine.transform.position.x, burnMachine.transform.position.y, mask.position.z);
        focusBtn.position = mask.position;
        Time.timeScale = 0;
    }
    public void OnClick()
    {
        if (Time.realtimeSinceStartup - timeStamp > 0.5f)
        {
            timeStamp = Time.realtimeSinceStartup;
        }
        else
        {
            Time.timeScale = 1;
            burnMachine.TossFood();
            Destroy(gameObject);
        }
    }
}
