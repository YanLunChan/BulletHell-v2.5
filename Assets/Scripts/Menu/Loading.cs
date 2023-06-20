using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

public class Loading : MonoBehaviour
{
    private AsyncOperation async;
    [SerializeField] private Image filledImage;
    [SerializeField] private Text textPresent;

    [SerializeField] private bool waitForUserInput = false;
    private bool ready = false;
    private bool anyKey = false;
    [SerializeField] private float delay = 0;
    [SerializeField] private string loadSceneByName = "";
    [SerializeField] private int sceneToLoad = -1; //pro hacking...

    public void OnAnyKey(InputAction.CallbackContext context)
    {
        anyKey = context.performed;
        print("pressed");
    }
    // Start is called before the first frame update
    void Start()
    {

        Time.timeScale = 1.0f;
        Input.ResetInputAxes();
        System.GC.Collect(); // Clear the memory of the unused items
        Scene currentScene = SceneManager.GetActiveScene();
        if (loadSceneByName != "")
            async = SceneManager.LoadSceneAsync(loadSceneByName);
        else if (sceneToLoad < 0)
            async = SceneManager.LoadSceneAsync(currentScene.buildIndex + 1);
        else
            async = SceneManager.LoadSceneAsync(sceneToLoad);

        async.allowSceneActivation = false;
        if (!waitForUserInput)
        {
            Invoke("Activate", delay);
        }
    }
    public void Activate()
    {
        ready = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (waitForUserInput && anyKey)
            if (async.progress >= 0.9f && SplashScreen.isFinished)
                ready = true;
        if (filledImage)
            filledImage.fillAmount = async.progress + 0.1f;
        if (textPresent)
            textPresent.text = ((async.progress + 0.1f) * 100).ToString("F2") + "%";
        if (async.progress >= 0.9f && SplashScreen.isFinished && ready)
        {
            async.allowSceneActivation = true;
        }
    }
}
