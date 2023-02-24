using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Coffee : Food
{
    // Start is called before the first frame update
    void Start()
    {
    }
    public override bool OnClick()
    {
        return true;
    }
    public override Food MergeFood(int id)
    {
        return null;
    }
}
