using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangePlate : ServePlateController
{
    protected  void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.enabled = false;
    }
}
