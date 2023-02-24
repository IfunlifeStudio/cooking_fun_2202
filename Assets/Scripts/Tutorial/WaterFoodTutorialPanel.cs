using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WaterFoodTutorialPanel : FoodTutorialPanel
{
    public override void Spawn(Vector3 maskPos)
    {
        GameObject go = Instantiate(gameObject);
        go.GetComponent<WaterFoodTutorialPanel>().Init(maskPos);
    }
    public void Init(Vector3 maskPos)
    {
        AudioController.Instance.PlaySfx(popUpClip);
        mask.position = new Vector3(maskPos.x, maskPos.y, mask.position.z);
        Time.timeScale = 0;
    }
    public void OnClick()
    {
        Time.timeScale = 1;
        StartCoroutine(DelayCall());
    }
    private IEnumerator DelayCall()
    {
        yield return new WaitForSeconds(0.1f);
        FindObjectOfType<MultiWaterMachine>().ServeFood();
        Destroy(gameObject, 0.1f);
    }
}
