using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotImageController : MonoBehaviour
{
    [SerializeField] private RawImage screenshot;
    public void Init(string path)
    {
        gameObject.SetActive(false);
        if (File.Exists(path))
        {
            var fileData = File.ReadAllBytes(path);
            var tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            screenshot.texture = tex;
        }
    }
}
