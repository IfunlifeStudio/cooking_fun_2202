using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyScrollView;
using EasingCore;
using System;
public class IslandPanelController : MonoBehaviour
{
    [SerializeField] FancyScrollView.Scroller scroller = default;
    [SerializeField, Range(float.Epsilon, 1f)] protected float cellInterval = 0.2f;
    [SerializeField, Range(0f, 1f)] protected float scrollOffset = 0.5f;
    [SerializeField] protected bool loop = false;
    [SerializeField] private GameObject rootZone;
    public List<IslandElement> pool =
        new List<IslandElement>();

    float currentPosition;
    bool isQuit;
    protected Context Context { get; } = new Context();
    void Awake()
    {
        Context.OnCellClicked = SelectCell;
    }
    void Start()
    {
        rootZone.GetComponent<Animator>().Play("Appear");
        scroller.OnValueChanged(UpdatePosition);
        scroller.OnSelectionChanged(UpdateSelection);
        scroller.SetTotalCount(5);
        foreach (var island in pool)
            island.SetupContext(Context);
        int zoneIndex = PlayerPrefs.GetInt(MainMenuController.ZONE_INDEX, -1);
        if (zoneIndex == -1)
            zoneIndex = 0;
        SelectCell(zoneIndex);
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !isQuit)
        {
            isQuit = true;
            SceneController.Instance.LoadScene("Mainmenu");
        }
    }
    /// <summary>
    /// Updates the scroll position.
    /// </summary>
    /// <param name="position">Position.</param>
    protected void UpdatePosition(float position) => UpdatePosition(position, false);
    /// <summary>
    /// Refreshes the cells.
    /// </summary>
    protected void Refresh() => UpdatePosition(currentPosition, true);
    void UpdatePosition(float position, bool forceRefresh)
    {
        currentPosition = position;
        var p = position - scrollOffset / cellInterval;
        var firstIndex = Mathf.CeilToInt(p);
        var firstPosition = (Mathf.Ceil(p) - p) * cellInterval;
        UpdateCells(firstPosition, firstIndex, false);
    }
    void UpdateCells(float firstPosition, int firstIndex, bool forceRefresh)
    {
        for (var i = 0; i < pool.Count; i++)
        {
            var index = firstIndex + i;
            var position = firstPosition + i * cellInterval;
            var cell = pool[CircularIndex(index, pool.Count)];
            if (loop)
                index = CircularIndex(index, pool.Count);
            //if (index < 0 || index >= pool.Count || position > 1f)
            //{
            //    cell.SetVisible(false);
            //    continue;
            //}
            if (forceRefresh || cell.Index != index || !cell.IsVisible)
            {
                cell.Index = index;
                cell.SetVisible(true);
            }
            cell.UpdatePosition(position);
        }
    }
    int CircularIndex(int i, int size) => size < 1 ? 0 : i < 0 ? size - 1 + (i + 1) % size : i % size;
    void UpdateSelection(int index)
    {
        if (Context.SelectedIndex == index)
            return;
        Context.SelectedIndex = index;
        Refresh();
    }
    public void SelectCell(int index)
    {
        if (index < 0 || index >= pool.Count)
            return;
        if (index == Context.SelectedIndex)
        {
            if (index == 1 && DataController.Instance.GetTotalKeyGranted() < 125) return;
            if (index == 2 && DataController.Instance.GetTotalKeyGranted() < 295) return;
            if (index == 3 && DataController.Instance.GetTotalKeyGranted() < 435) return;
            StartCoroutine(DelayClose());
            return;
        }
        UpdateSelection(index);
        scroller.ScrollTo(index, 0.35f, Ease.OutCubic);
    }
    private IEnumerator DelayClose()
    {
        rootZone.GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        DataController.Instance.currentChapter = 0;
        LevelDataController.Instance.lastestPassedLevel = null;
        PlayerPrefs.SetInt(MainMenuController.ZONE_INDEX, Context.SelectedIndex);
        SceneController.Instance.LoadScene("MainMenu");
    }
}
