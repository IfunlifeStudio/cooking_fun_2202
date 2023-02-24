using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateSpriteController : MonoBehaviour
{
    [SerializeField] private int id = 1100;
    [SerializeField] private SpriteRenderer PlateSprite;
    [SerializeField] private Sprite[] sprites;

    // Start is called before the first frame update
    void Start()
    {
        int ingredientLevel = DataController.Instance.GetMachineLevel(id);
        PlateSprite.sprite = sprites[ingredientLevel - 1];
    }
}
