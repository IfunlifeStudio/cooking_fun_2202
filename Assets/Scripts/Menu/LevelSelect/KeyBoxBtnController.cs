using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using TMPro;
using DigitalRuby.Tween;
public class KeyBoxBtnController : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic boxSkeleton, keySkeleton;
    [SerializeField] private TextMeshProUGUI keyCountText;
    [SerializeField] private KeyPanelController keyPanelPrefab;
    [SerializeField] private MysteryBoxTutorialPanel mysteryBoxTutorialPrefab;
    [SerializeField] private Animator KeyCountPanel;
    [SerializeField] private GameObject keyPropPrefab, openBoxEffectPrefab, finishLevelEffectPrefab, hand;
    [SerializeField] private AudioClip keyReceivedAudio;
    private LevelSelectorController levelSelector;
    private int chapter;
    private int totalLevels, totalKeys, totalLevelRestaurant;
    private bool hasClickOpenBox, isCollectingKey;
    private float instructionTime;
    private bool canInstruction = false;
    private string ABtesting = "";

    // Start is called before the first frame update
    void Start()
    {
        instructionTime = Time.time;
        ABtesting = PlayerPrefs.GetString("ABtesting_data", "0");
        levelSelector = FindObjectOfType<LevelSelectorController>();
        chapter = DataController.Instance.currentChapter;//total key need to unlock
        int[] levelProgressUnlock = DataController.Instance.GetLevelProgressUnlock(chapter);
        totalLevelRestaurant = levelProgressUnlock[levelProgressUnlock.Length - 1];
        int lastestLevel = DataController.Instance.GetHighestLevel(chapter);
        int index = 0;
        do
        {
            totalLevels = levelProgressUnlock[index];
            index++;
        }
        while (lastestLevel > totalLevels && index < levelProgressUnlock.Length);
        var lastestPassedLevel = LevelDataController.Instance.lastestPassedLevel;
        totalKeys = 0;
        for (int i = 1; i < totalLevels + 1; i++)
        {
            if (LevelDataController.Instance.GetLevelData(chapter, i).IsKeyGranted())
                totalKeys++;
        }
        if (lastestPassedLevel == null && totalKeys == totalLevels)
            totalLevels = levelProgressUnlock[Mathf.Min(index, levelProgressUnlock.Length - 1)];
            if (DataController.Instance.ExtraJobData.IsExtraJob(chapter, lastestLevel) && PlayerPrefs.GetInt("extra_key" + chapter + lastestLevel, 0) != 1)
            {
                PlayerPrefs.SetInt("extra_key" + chapter + lastestLevel, 1);
                StartCoroutine(CollectKey());
            }
            else
            {
                StartCoroutine(CollectKey());
            }    
    }
    private void Update()
    {
        if (Time.time - instructionTime >= 10 && canInstruction && ABtesting == "1")
        {
            canInstruction = false;
            hand.SetActive(true);
        }
        if (Input.GetMouseButtonDown(0))
        {
            instructionTime = Time.time;
        }
    }
    private IEnumerator CollectKey()
    {
        isCollectingKey = true;
        var lastestPassedLevel = LevelDataController.Instance.lastestPassedLevel;
        yield return new WaitForSeconds(0.25f);//waiting for UI for init
        if (lastestPassedLevel != null && lastestPassedLevel.chapter==DataController.Instance.currentChapter)
        {
            if (lastestPassedLevel.IsJustGrantKey())
            {
                keyCountText.text = totalKeys - 1 + "/" + totalLevels;
                LevelBtnController lastLevel = FindObjectOfType<LevelSelectorController>().GetLevelBtn(lastestPassedLevel.chapter, lastestPassedLevel.id); //get the last win level btn
                GameObject go = Instantiate(finishLevelEffectPrefab, lastLevel.transform.position, Quaternion.identity);
                Destroy(go, 1);
                GameObject keyProp = Instantiate(keyPropPrefab, lastLevel.key.transform.position, Quaternion.identity, transform.parent);//spawn a key prop and move to box key
                keyProp.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "FX_key", false);
                yield return new WaitForSeconds(1);
                keyProp.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "idle", true);
                levelSelector.FocusBoxKey();//scroll the level select menu to box key
                Vector3 startPos = keyProp.transform.position;
                float startTime = Time.time;
                while (Vector3.Distance(keyProp.transform.position, keySkeleton.transform.position) > 0.1f)
                {
                    keyProp.transform.position = Vector3.Lerp(startPos, keySkeleton.transform.position, (Time.time - startTime) * 4);
                    yield return null;
                }
                keyProp.transform.position = keySkeleton.transform.position;
                AudioController.Instance.PlaySfx(keyReceivedAudio);
                Destroy(keyProp);
                keySkeleton.AnimationState.SetAnimation(0, "FX_key_2", false);
                yield return new WaitForSeconds(1f); 
                keySkeleton.AnimationState.SetAnimation(0, "idle", false);
                KeyCountPanel.Play("KeyText"); 
                keyCountText.text = totalKeys + "/" + totalLevels;
                yield return new WaitForSeconds(0.75f);
                
                if (totalKeys == totalLevels)
                {
                    FindObjectOfType<MainMenuController>().setIndexTab(5);
                    if (PlayerPrefs.GetInt("key_box", 0) == 1)//handle old data
                    {
                        if (!DataController.Instance.GetTutorialData().Contains(6000))
                            DataController.Instance.GetTutorialData().Add(6000);
                    }
                    if (!DataController.Instance.GetTutorialData().Contains(6000))
                    {
                        DataController.Instance.GetTutorialData().Add(6000);
                        mysteryBoxTutorialPrefab.Spawn(transform.position);
                    }
                    else
                    {
                        canInstruction = true;
                    }
                }
                else
                {
                    levelSelector.FocusCurrentLevel();//scroll the level select menu to box key
                }
            }
        }
        keyCountText.text = totalKeys + "/" + totalLevels;
        isCollectingKey = false;
        LevelDataController.Instance.lastestPassedLevel = null;
        LevelDataController.Instance.collectedGold = 0;
    }
    public void Spawn(Vector3 localSpawnPos, Transform parent)
    {
        GameObject box = Instantiate(gameObject, parent);
        box.transform.localPosition = localSpawnPos;
    }
    public void OnClickBox()
    {
        if (isCollectingKey) return;
        if (totalKeys < totalLevels)//open warning pop up
        {
            Transform parent = GameObject.Find("CanvasCamera").transform;
            keyPanelPrefab.Spawn(levelSelector, parent, totalKeys, totalLevels);
        }
        else
        {
            if (!hasClickOpenBox)
            {
                hasClickOpenBox = true;
                hand.SetActive(false);
                boxSkeleton.AnimationState.SetAnimation(0, "click_open", false);
                StartCoroutine(DelayOpenBox());//unlock new level after click box, instance new level btn container
            }
        }
    }
    public bool CanUnboxClaimRestaurantReward()
    {
        if(!hasClickOpenBox && totalKeys == totalLevels)
        {
            hasClickOpenBox = true;
            boxSkeleton.AnimationState.SetAnimation(0, "click_open", false);
            FindObjectOfType<LevelSelectorController>().OnClickOpenLevelBox();
            Destroy(gameObject,0.2f);
            return true;
        }
        return false;
    }
    private IEnumerator DelayOpenBox()
    {
        GameObject go = Instantiate(openBoxEffectPrefab, transform.position, Quaternion.identity);
        Destroy(go, 1);
        if (totalKeys != totalLevelRestaurant)
        {
            yield return new WaitForSeconds(0.8f);
            FindObjectOfType<RewardPanelController>().Init(0, 10, 0, new int[1] { LevelDataController.Instance.GetItemRandom() }, new int[1] {1}, false, "default",
                () => { FindObjectOfType<LevelSelectorController>().OnClickOpenLevelBox(); });
        }
        else
        {
            yield return new WaitForSeconds(0.8f);
            FindObjectOfType<LevelSelectorController>().OnClickOpenLevelBox();
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
