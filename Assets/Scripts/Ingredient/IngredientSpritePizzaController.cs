using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpritePizzaController : MonoBehaviour
{
    [SerializeField] private int ingredientId = 1100;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Sprite[] spriCooked;
    private SpriteRenderer foodSprite;
    private Color oriColor;
    //private bool isBurned = false, isCooking = false, isCompleted = false;
    // Start is called before the first frame update

    void Start()
    {
        int ingredientLevel = DataController.Instance.GetIngredientLevel(ingredientId);
        foodSprite = GetComponent<SpriteRenderer>();
        foodSprite.sprite = sprites[ingredientLevel - 1];
        oriColor = foodSprite.color;
    }


    public void IsBurned()
    {
        foodSprite = GetComponent<SpriteRenderer>();
        foodSprite.color = new Color32(227, 89, 89, 255);
    }
    public void IsSecondChange(){
        int ingredientLevel = DataController.Instance.GetIngredientLevel(ingredientId);
        foodSprite = GetComponent<SpriteRenderer>();
        foodSprite.sprite = sprites[ingredientLevel - 1];
        foodSprite.color = oriColor;
    }
    public void IsCooking()
    {
        int ingredientLevel = DataController.Instance.GetIngredientLevel(ingredientId);
        foodSprite = GetComponent<SpriteRenderer>();
        foodSprite.sprite = sprites[ingredientLevel - 1];

    }
    public void IsCompleted()
    {


        int ingredientLevel = DataController.Instance.GetIngredientLevel(ingredientId);
        foodSprite = GetComponent<SpriteRenderer>();
        foodSprite.sprite = spriCooked[ingredientLevel - 1];

    }
    private void OnDisable()
    {
       IsSecondChange();
    }
}
