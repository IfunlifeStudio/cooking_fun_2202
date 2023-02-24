using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    private static SceneController instance;
    public bool isLoading = false;
    public static SceneController Instance
    {
        get { return instance; }
    }
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
            Destroy(gameObject);
    }
    public void LoadScene(string sceneName, bool saveData = true)
    {
        if (saveData)
            DataController.Instance.SaveData(false);
        //UIController.Instance.Clear();
        StartCoroutine(_LoadScene(sceneName));
    }
    private IEnumerator _LoadScene(string sceneName)
    {
        if (isLoading) yield return null;
        isLoading = true;
        var delay = new WaitForSecondsRealtime(0.06f);
        string currentScene = SceneManager.GetActiveScene().name;
        var asyncTask = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        while (!asyncTask.isDone)
            yield return delay;
        LoadingSceneController loadingBar = FindObjectOfType<LoadingSceneController>();
        asyncTask = SceneManager.UnloadSceneAsync(currentScene);
        while (!asyncTask.isDone)
        {
            loadingBar.UpdateLoadingBar(asyncTask.progress / 2);
            yield return delay;
        }
        asyncTask = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!asyncTask.isDone)
        {
            loadingBar.UpdateLoadingBar(Mathf.Min(0.5f + asyncTask.progress / 2, 0.82f));
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        yield return null;
        float currentLoading = 0.82f;
        while (currentLoading < 1f)
        {
            loadingBar.UpdateLoadingBar(currentLoading);
            currentLoading += 0.06f;
            yield return new WaitForSecondsRealtime(0.06f);
        }
        loadingBar.UpdateLoadingBar(1);
        yield return new WaitForSecondsRealtime(0.25f);
        if (!(Application.internetReachability == NetworkReachability.NotReachable))
        {
            ScreenDataTracking screenTracking = new ScreenDataTracking(sceneName);
            string request = JsonUtility.ToJson(screenTracking);
            APIController.Instance.PostData(request, Url.ScreenTracking);
        }
        SceneManager.UnloadSceneAsync("Loading");
        isLoading = false;
    }
}