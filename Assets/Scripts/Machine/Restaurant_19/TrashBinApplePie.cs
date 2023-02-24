using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBinApplePie : MonoBehaviour
{
    private GameController gameController;
    [SerializeField] private AudioClip binOpenSound;
    [SerializeField]
    private GameObject Open, Close;
   private float tossTimeStamp;

    private void Start()
    {
        Close.SetActive(true);
        Open.SetActive(false);
        gameController = FindObjectOfType<GameController>();
        tossTimeStamp = Time.time;
    }
    public void TossTrash(int foodId)
    {
        if (LevelDataController.Instance.currentLevel.ConditionType() == 2 && FindObjectOfType<GameController>().isPlaying && Time.time - tossTimeStamp > 0.25f)
        {
            tossTimeStamp = Time.time;
            AudioController.Instance.Vibrate();
            MessageManager.Instance.SendMessage(
                    new Message(CookingMessageType.OnConditionViolate,
                    new object[] { LevelDataController.Instance.currentLevel.ConditionType(), transform.position, this }));
        }
        AudioController.Instance.PlaySfx(binOpenSound);
        gameController.OnPlayerThrowFood(foodId);
        StartCoroutine(TrashProcess());
    }
    private IEnumerator TrashProcess()
    {
        Open.SetActive(true);
        Close.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        Open.SetActive(false);
        Close.SetActive(true);
    }
}
