using UnityEngine;
public class Rotate : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.back, Time.deltaTime * 100);
    }
}
