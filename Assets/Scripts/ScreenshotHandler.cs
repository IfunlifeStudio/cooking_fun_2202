using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenshotHandler : MonoBehaviour
{
    private Camera myCamera;
    private bool takeScreenshotOnNextFrame;
    private static AndroidJavaObject _activity;
    // Start is called before the first frame update
    void Start()
    {
        myCamera = GetComponent<Camera>();
    }
    public IEnumerator DelayTakeScreenshot(Action<string> callback = null, bool hasSaveImage = false)
    {
        int resWidth = Screen.width;
        int resHeight = Screen.height;
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        myCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        myCamera.Render();
        yield return new WaitForEndOfFrame();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        screenShot.Apply();
        myCamera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        if (hasSaveImage)
        {
            NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(screenShot, "CookingLove", "cer_screenshot.png", null);
        }
        string filename = "";
        if (callback != null)
            filename = Path.Combine(Application.temporaryCachePath, "/cer_screenshot.png");
        System.IO.File.WriteAllBytes(filename, bytes);
        yield return new WaitForSeconds(0.1f);
        if (callback != null)
            callback.Invoke(filename);
        Destroy(screenShot);
    }
    public void TakeScreenshot()
    {
        takeScreenshotOnNextFrame = true;
    }
}
