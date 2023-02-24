using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovementController : MonoBehaviour
{
    [SerializeField] private float speed;
    void Update()
    {
        if (transform.position.x > -15)
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        else
            transform.position = new Vector3(15, transform.position.y, transform.position.z);
    }
}
