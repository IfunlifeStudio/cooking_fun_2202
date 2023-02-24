using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ComboBarController : MonoBehaviour
{
    public const int COMBO_COOLDOWN = 3;
    private int combo = 0;
    private float comboTimeStamp = 0;
    [SerializeField] private bool isTut = false;
    [SerializeField] private ComboTextController goldBonusEffect;
    [SerializeField] private GameObject combo3Effect, combo4Effect, combo5Effect;
    [SerializeField] private int[] combosBonus = new int[] { 5, 10, 20 };//combo gold will be add directly to the gameplay gold
    [SerializeField] private AudioClip[] comboSoundSfx;
    [SerializeField] private Image comboCountDown;
    [SerializeField] private Image[] comboLevels;
    public int Combo
    {
        get { return combo; }
        set
        {
            combo = value;
            if (combo > 5) combo = combo - 5;
            if (combo > 2)
            {
                AudioController.Instance.PlaySfx(comboSoundSfx[combo - 3]);
                int goldBonus = combosBonus[combo - 3];
                gameController.IncreaseGold(goldBonus, true);
                //goldBonusEffect.Spawn(comboLevels[combo-1].transform.position + Vector3.forward, comboLevels[combo-1].transform, "+ " + goldBonus);
                //if (combo == 3)
                //    MessageManager.Instance.SendMessage(new Message(CookingMessageType.OnCombo3));
                //else if (combo == 4)
                //    MessageManager.Instance.SendMessage(new Message(CookingMessageType.OnCombo4));
                //else if (combo == 5)
                //    MessageManager.Instance.SendMessage(new Message(CookingMessageType.OnCombo5));
                if (combo == 5 && !LevelDataController.Instance.IsExtraJob)
                    gameController.OnDoCombo5();
                //AchivementController.Instance.OnDoCombo(DataController.Instance.currentChapter);
                isTut = false;//turn off tut mode after grant first combo
            }
            comboTimeStamp = Time.time;
            for (int i = comboLevels.Length - 1; i > -1; i--)
            {
                if (i < combo)
                    comboLevels[i].gameObject.SetActive(true);
                else
                    comboLevels[i].gameObject.SetActive(false);
            }
            if (combo == 0) comboCountDown.fillAmount = 0;
        }
    }
    private GameController gameController;
    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        Combo = 0;
    }
    private void Update()
    {
        if (isTut) return;
        float timeSinceLastCombo = Time.time - comboTimeStamp;
        if (timeSinceLastCombo > COMBO_COOLDOWN)
            Combo = 0;
        else
        if (combo > 0)
            comboCountDown.fillAmount = 1 - timeSinceLastCombo / COMBO_COOLDOWN;
    }
    public void SpawnComboEffect(Vector3 spawnPos, int combo)
    {
        Transform wordCanvas = gameController.canvasWorld;
        //goldBonusEffect.Spawn(spawnPos, wordCanvas.transform, combo.ToString());
        //if (combo >= 3 && combo <= 5)
        //    goldBonusEffect.Spawn(spawnPos, wordCanvas, "+" + combosBonus[combo - 3]);
        if (combo == 3)
        {
            GameObject go = Instantiate(combo3Effect, spawnPos, Quaternion.identity, wordCanvas.transform);
            goldBonusEffect.Spawn(spawnPos, wordCanvas, "+" + combosBonus[combo - 3]);
            Destroy(go, 2.6f);
        }
        else if (combo == 4)
        {
            GameObject go = Instantiate(combo4Effect, spawnPos, Quaternion.identity, wordCanvas.transform);
            goldBonusEffect.Spawn(spawnPos, wordCanvas, "+" + combosBonus[combo - 3]);
            Destroy(go, 2.6f);
        }
        else if (combo == 5)
        {
            GameObject go = Instantiate(combo5Effect, spawnPos, Quaternion.identity, wordCanvas.transform);
            goldBonusEffect.Spawn(spawnPos, wordCanvas, "+" + combosBonus[combo - 3]);
            Destroy(go, 2.6f);
        }
    }
}
