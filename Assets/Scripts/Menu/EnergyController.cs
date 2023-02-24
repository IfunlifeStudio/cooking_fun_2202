using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DigitalRuby.Tween;
public class EnergyController : MonoBehaviour
{
    private const int ENERGY_REPLENISH_TIME = 1800, MAX_ENERGY = 5;
    [SerializeField] private TextMeshProUGUI energyCount, timeCountDown;
    [SerializeField] private EnergyPanelController buyEnergyPanelPrefab;
    [SerializeField] private Image energyIcon;
    [SerializeField] private Sprite[] imageEneryIcons;
    [SerializeField] private GameObject energyPropPrefab, unlimitedEnergyPropPrefab;
    [SerializeField] private Transform cameraCanvas;
    [SerializeField] private Button buyEnergyBtn;
    [SerializeField] private MainMenuController mainMenuController;
    private int currentEnergy;
    private void Start()
    {
        DataController.Instance.Energy += CalculateGainEnergy();
        energyCount.text = DataController.Instance.Energy.ToString();
        currentEnergy = DataController.Instance.Energy;
        buyEnergyBtn.gameObject.SetActive(DataController.Instance.Energy == 0 && DataController.Instance.UnlimitedEnergyDuration <= 0);
        StopAllCoroutines();
        StartCoroutine(ReplenishEnergy());
    }
    private IEnumerator ReplenishEnergy()
    {
        var delay = new WaitForSecondsRealtime(1);
        double saveTimeStamp = DataController.Instance.EnergyTimeStamp;
        double deltaTime;
        int remainTime;
        while (true)
        {
            DataController.Instance.UnlimitedEnergyDuration -= CalculateTimePassUnlimitedEnergy();
            if (DataController.Instance.UnlimitedEnergyDuration > 0)
                energyIcon.sprite = imageEneryIcons[1];
            else
                energyIcon.sprite = imageEneryIcons[0];
            energyCount.gameObject.SetActive(DataController.Instance.UnlimitedEnergyDuration <= 0);
            if (DataController.Instance.UnlimitedEnergyDuration > 0)
            {
                if (DataController.Instance.UnlimitedEnergyDuration < 0) DataController.Instance.UnlimitedEnergyDuration = 0;
                DataController.Instance.UnlimitedEnergyTimeStamp = DataController.ConvertToUnixTime(DateTime.UtcNow);
                timeCountDown.text = System.String.Format("{0:D2}:{1:D2}", DataController.Instance.UnlimitedEnergyDuration / 60, DataController.Instance.UnlimitedEnergyDuration % 60);
            }
            deltaTime = DataController.ConvertToUnixTime(DateTime.UtcNow) - saveTimeStamp;
            if (deltaTime < -1800)
            {
                DataController.Instance.EnergyTimeStamp = DataController.ConvertToUnixTime(DateTime.UtcNow);//user hack time
                deltaTime = 0;
            }
            if (deltaTime >= ENERGY_REPLENISH_TIME)
            {
                saveTimeStamp = DataController.ConvertToUnixTime(DateTime.UtcNow);
                DataController.Instance.Energy++;//add a energy
                currentEnergy = DataController.Instance.Energy;
                if (DataController.Instance.UnlimitedEnergyDuration <= 0)
                {
                    buyEnergyBtn.gameObject.SetActive(DataController.Instance.Energy == 0);
                    energyCount.text = DataController.Instance.Energy.ToString();
                }
            }
            if (DataController.Instance.Energy < MAX_ENERGY)//calculate the remain time and convert to minute and second
            {
                remainTime = (int)(ENERGY_REPLENISH_TIME - deltaTime);
                if (remainTime < 0) remainTime = 0;
                if (DataController.Instance.UnlimitedEnergyDuration <= 0)
                    timeCountDown.text = System.String.Format("{0:D2}:{1:D2}", remainTime / 60, remainTime % 60);
            }
            else
            {
                if (DataController.Instance.UnlimitedEnergyDuration <= 0)
                {
                    energyIcon.sprite = imageEneryIcons[0];
                    timeCountDown.text = Lean.Localization.LeanLocalization.GetTranslationText("energy_full");
                }
            }
            yield return delay;
        }
    }
    private int CalculateTimePassUnlimitedEnergy()
    {
        double deltaTime = DataController.ConvertToUnixTime(DateTime.UtcNow) - DataController.Instance.UnlimitedEnergyTimeStamp;
        return Mathf.Max(0, (int)deltaTime);
    }
    private int CalculateGainEnergy()//this function calculate the offline energy of player
    {
        double deltaTime = DataController.ConvertToUnixTime(DateTime.UtcNow) - DataController.Instance.EnergyTimeStamp;
        return Mathf.Min((int)(deltaTime / ENERGY_REPLENISH_TIME), MAX_ENERGY);
    }
    public void OpenBuyEnergyPanel()
    {
        buyEnergyPanelPrefab.Spawn(cameraCanvas, PlayIncreaseEnergyEffect);
        mainMenuController.setIndexTab(5);
    }
    public void IncreaseOneEnergy()
    {
        DataController.Instance.IncreaseOneEnergy();
        energyCount.text = DataController.Instance.Energy.ToString();
    }
    public void PlayIncreaseEnergyEffect()
    {
        energyCount.text = DataController.Instance.Energy.ToString();
        int amount = DataController.Instance.Energy - currentEnergy;
        currentEnergy = DataController.Instance.Energy;
        energyCount.text = DataController.Instance.Energy.ToString();
        buyEnergyBtn.gameObject.SetActive(DataController.Instance.Energy == 0);
        for (int i = 0; i < amount; i++)
        {
            GameObject energyProp = Instantiate(energyPropPrefab, cameraCanvas.position + new Vector3(0, 0, -0.1f), Quaternion.identity, cameraCanvas);
            StartCoroutine(IMove(energyProp, energyIcon.transform.position, 1));
            //Vector3 target = energyProp.transform.position + new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.5f, 1.5f), 0);//generate a random pos
            //System.Action<ITween<Vector3>> updatePropPos = (t) =>
            //    {
            //        energyProp.transform.position = t.CurrentValue;
            //    };
            //System.Action<ITween<Vector3>> completedPropMovement = (t) =>
            //{
            //    Destroy(energyProp);
            //};
            //TweenFactory.Tween("energy" + i + Time.time, energyProp.transform.position, target, 0.5f + i * 0.06f, TweenScaleFunctions.QuinticEaseOut, updatePropPos)
            //.ContinueWith(new Vector3Tween().Setup(target, energyIcon.transform.position, 0.25f + i * 0.06f, TweenScaleFunctions.QuadraticEaseIn, updatePropPos, completedPropMovement));
        }
    }
    public IEnumerator IMove(GameObject gameObject, Vector2 pos, float speed)
    {
        float time = 0;
        Vector2 midlePos = new Vector2((gameObject.transform.position.x + pos.x) / 2f + (UnityEngine.Random.Range(-6f, 0f)), (gameObject.transform.position.y + pos.y) / 3f);
        Vector2 tempPos = gameObject.transform.position;
        while (Vector2.Distance(gameObject.transform.position, pos) > 0.3f)
        {
            gameObject.transform.position = CalculateQuadraticBezierPoint(time, tempPos, midlePos, pos);
            time += Time.deltaTime * speed * 2;
            yield return null;
        }
        DG.Tweening.DOVirtual.DelayedCall(0.05f, () =>
        {
            Destroy(gameObject);
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
    public void PlayUnlimitedEnergyEffect(Transform parent = null)
    {
        if (parent == null) parent = cameraCanvas;
        buyEnergyBtn.gameObject.SetActive(false);
        GameObject energyProp = Instantiate(unlimitedEnergyPropPrefab, parent.position + new Vector3(0, 0, -1f), Quaternion.identity, parent);
        Vector3 target = energyProp.transform.position + new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.5f, 1.5f), 0);//generate a random pos
        System.Action<ITween<Vector3>> updatePropPos = (t) =>
            {
                energyProp.transform.position = t.CurrentValue;
            };
        System.Action<ITween<Vector3>> completedPropMovement = (t) =>
        {
            StopAllCoroutines();
            DataController.Instance.UnlimitedEnergyTimeStamp = DataController.ConvertToUnixTime(DateTime.UtcNow);
            DataController.Instance.UnlimitedEnergyDuration -= CalculateTimePassUnlimitedEnergy();
            if (DataController.Instance.UnlimitedEnergyDuration > 0)
            {
                energyIcon.sprite = imageEneryIcons[1];
                StartCoroutine(ReplenishEnergy());
            }
            else
                energyIcon.sprite = imageEneryIcons[0];
            energyCount.gameObject.SetActive(DataController.Instance.UnlimitedEnergyDuration <= 0);
            Destroy(energyProp);
        };
        TweenFactory.Tween("energy" + Time.time, energyProp.transform.position, target, 0.5f, TweenScaleFunctions.QuinticEaseOut, updatePropPos)
        .ContinueWith(new Vector3Tween().Setup(target, energyIcon.transform.position, 0.5f, TweenScaleFunctions.QuadraticEaseIn, updatePropPos, completedPropMovement));
    }
}
