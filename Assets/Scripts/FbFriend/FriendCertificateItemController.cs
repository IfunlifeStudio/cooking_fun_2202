using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendCertificateItemController : MonoBehaviour
{
    [SerializeField] int resID;
    [SerializeField] Image coverImg, coverIconImg;
    [SerializeField] Material material;
    // Start is called before the first frame update
    public void Init(int winrate)
    {
        gameObject.SetActive(true);
        coverImg.material = material;
        SetCertiIconColor(winrate);
    }
    private void SetCertiIconColor(int winrate)
    {
        if (winrate != 0)
        {
            Color color;
            if (winrate >= 90)
                color = new Color32(255, 255, 0, 255);
            else if (winrate >= 80)
                color = new Color32(171, 56, 237, 255);
            else if (winrate >= 70)
                color = new Color32(29, 126, 225, 255);
            else
                color = new Color32(4, 177, 0, 255);
            coverIconImg.color = color;
        }
    }
}
