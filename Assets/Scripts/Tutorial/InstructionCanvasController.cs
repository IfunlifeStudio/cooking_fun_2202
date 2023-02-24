using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionCanvasController : MonoBehaviour
{
    public GameObject Hand;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }
    public void InitUI(Vector2 position, Transform parent = null)
    {
        if (parent != null)
            gameObject.transform.SetParent(parent);
        Hand.transform.position = position;
        gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }
}
