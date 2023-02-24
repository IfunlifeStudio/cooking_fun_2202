using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchivementPanel : MonoBehaviour
{
    public Animator Anim;
    public ItemAchivement pfEtemAchivement;
    public List<ItemAchivement> itemAchivements = new List<ItemAchivement>();
    public Transform Conten;
    public void OnShow()
    {
        gameObject.SetActive(true);
        Anim.Play("Appear");
        if (itemAchivements.Count == 0)
            InitItemAchivement();
        else
        {
            for (int i = 0; i < AchivementController.Instance.GetAchivementCount(); i++)
                itemAchivements[i].SetUpData();
        }
        ReSortItemAchivement();
        FindObjectOfType<SandwichPanelController>().OnHideAchivementNotification(false);
    }

    public void ReSortItemAchivement()
    {
        for (int i = 0; i < AchivementController.Instance.GetAchivementCount(); i++)
            itemAchivements[i].ReSortItemAchivement();
    }

    void InitItemAchivement()
    {
        for (int i = 0; i < AchivementController.Instance.GetAchivementCount(); i++)
        {
            var ia = Instantiate(pfEtemAchivement) as ItemAchivement;
            ia.gameObject.name = i.ToString();
            ia.gameObject.transform.SetParent(Conten);
            ia.transform.localScale = Vector3.one;
            ia.SetUpData();
            itemAchivements.Add(ia);
        }
    }

    public void OnHide()
    {
        Anim.Play("Disappear");
        StartCoroutine("IEHide");
    }

    IEnumerator IEHide()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }
}
