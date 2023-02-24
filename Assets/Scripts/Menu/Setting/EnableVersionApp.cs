using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnableVersionApp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtVersion;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);
        if (DataController.Instance == null)
            txtVersion.text = "Ver " + Application.version;
        else
        {
            string paying_type = PlayerPrefs.GetString("paying_type", "f1");
            if (txtVersion != null)
                txtVersion.text = "Ver " + Application.version + "_" + paying_type;
        }
    }
}
