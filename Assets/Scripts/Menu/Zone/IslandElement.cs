using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DigitalRuby.Tween;
public class IslandElement : MonoBehaviour
{
    [SerializeField] private GameObject keyCount;
    [SerializeField] Animator animator = default;
    public static readonly int Scroll_Hash = Animator.StringToHash("scroll");
    /// <summary>
    /// Gets or sets the index of the data.
    /// </summary>
    /// <value>The index of the data.</value>
    public int Index { get; set; } = -1;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:FancyScrollView.FancyScrollViewCell`2"/> is visible.
    /// </summary>
    /// <value><c>true</c> if is visible; otherwise, <c>false</c>.</value>
    public bool IsVisible => gameObject.activeSelf;

    /// <summary>
    /// Gets the context.
    /// </summary>
    /// <value>The context.</value>
    protected Context Context { get; private set; }

    /// <summary>
    /// Setup the context.
    /// </summary>
    /// <param name="context">Context.</param>
    public void SetupContext(Context context) => Context = context;

    /// <summary>
    /// Sets the visible.
    /// </summary>
    /// <param name="visible">If set to <c>true</c> visible.</param>
    public void SetVisible(bool visible) => gameObject.SetActive(visible);

    /// <summary>
    /// Updates the position.
    /// </summary>
    /// <param name="position">Position.</param>
    public void UpdatePosition(float position)
    {
        currentPosition = position;
        animator.Play(Scroll_Hash, -1, position);
        animator.speed = 0;
    }
    float currentPosition = 0;
    public Button button;
    [SerializeField] private Material[] islandMats;
    [SerializeField] private int unlockKeyThreshold = 0;
    [SerializeField] private GameObject[] imageElements;
    private int totalKeyGranted;
    private IslandPanelController IslandPanelController;
    void OnEnable() => UpdatePosition(currentPosition);
    void Start()
    {
        button.onClick.AddListener(OnClick);
        button.onClick.AddListener(() => Context.OnCellClicked?.Invoke(Index));
        totalKeyGranted = DataController.Instance.GetTotalKeyGranted();
        if (keyCount != null)
        {
            if (totalKeyGranted >= unlockKeyThreshold)
                keyCount.SetActive(false);
            else
                keyCount.GetComponentInChildren<TextMeshProUGUI>().text = totalKeyGranted + "/" + unlockKeyThreshold;
        }
        if (totalKeyGranted < unlockKeyThreshold)
        {
            for(int i = 0; i < imageElements.Length; i++)
            {
                var image = imageElements[i].GetComponent<Image>();
                if (image != null)
                    image.material = islandMats[1];
                else
                    imageElements[i].GetComponent<Spine.Unity.SkeletonGraphic>().material = islandMats[1];
            }                
        }
        else
        {
            for (int i = 0; i < imageElements.Length; i++)
            {
                var image = imageElements[i].GetComponent<Image>();
                if (image != null)
                    image.material = islandMats[0];
                else
                    imageElements[i].GetComponent<Spine.Unity.SkeletonGraphic>().material = islandMats[0];
            }
        }
    }
    private void OnClick()
    {
        if (Context.SelectedIndex == Index && totalKeyGranted < unlockKeyThreshold)
        {
            System.Action<ITween<Vector3>> updateIslandScale = (t) =>
            {
                transform.localScale = t.CurrentValue;
            };
            TweenFactory.Tween("island" + Time.time, transform.localScale, new Vector3(1.1f, 1.1f, 1), 0.1f, TweenScaleFunctions.QuinticEaseOut, updateIslandScale)
            .ContinueWith(new Vector3Tween().Setup(new Vector3(1.1f, 1.1f, 1), new Vector3(0.95f, 0.95f, 1), 0.12f, TweenScaleFunctions.QuinticEaseInOut, updateIslandScale))
            .ContinueWith(new Vector3Tween().Setup(new Vector3(0.95f, 0.95f, 1), new Vector3(1f, 1f, 1f), 0.1f, TweenScaleFunctions.QuinticEaseIn, updateIslandScale))
            ;
        }
    }
}
