using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonFoodTutorialPanel : FoodTutorialPanel
{
    public override void Spawn(Vector3 maskPos)
    {
        UIController.Instance.CanBack = false;
        GameObject go = Instantiate(gameObject);
        go.GetComponent<CommonFoodTutorialPanel>().Init(maskPos);
    }
    public void Init(Vector3 maskPos)
    {
        AudioController.Instance.PlaySfx(popUpClip);
        mask.position = new Vector3(maskPos.x, maskPos.y, mask.position.z);
        Time.timeScale = 0;
    }
    public void OnClick()
    {
        UIController.Instance.CanBack = true;
        Time.timeScale = 1;
        Destroy(gameObject, 0.1f);
    }
}
