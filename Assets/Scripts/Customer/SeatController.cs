using UnityEngine;

public class SeatController : MonoBehaviour
{
    public Transform[] productsPos = default;
    [SerializeField] private Transform spriteMask;
    public GameObject orderBoard;
    public GameObject heartIcon;
    public GameObject tickIcon;
    public Animator orderAnimator;
    public Sprite[] frontBars;
    [SerializeField] private SpriteRenderer frontBar;
    public void Play(string anim)
    {
        orderAnimator.Play(anim);
    }
    public void UpdateWaitBar(float fillAmount)
    {
        if (fillAmount > 0.5f)
            frontBar.sprite = frontBars[0];
        else if (fillAmount > 0.2f)
            frontBar.sprite = frontBars[1];
        else
            frontBar.sprite = frontBars[2];
        spriteMask.localScale = new Vector3(1, fillAmount, 1);
    }
}
