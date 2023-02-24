using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpriteController : MonoBehaviour
{
    [SerializeField] private int ingredientId = 1100;
    [SerializeField] private Sprite[] sprites;
    private SpriteRenderer foodSprite;
    // Start is called before the first frame update
    void Start()
    {
        int ingredientLevel = DataController.Instance.GetIngredientLevel(ingredientId);
        foodSprite = GetComponent<SpriteRenderer>();
        foodSprite.sprite = sprites[ingredientLevel - 1];
    }
}
