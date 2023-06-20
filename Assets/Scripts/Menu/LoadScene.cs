using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadScene : MonoBehaviour
{
    private AsyncOperation async; // Data that has been loaded...
    public void BtnLoadScene() //no parameter == next scene...(overflow...)
    {
        if (async != null)
            return;
        Scene currentScene = SceneManager.GetActiveScene(); // return active scene
        async = SceneManager.LoadSceneAsync(currentScene.buildIndex + 1); // load next scene...
    }
    public void BtnLoadScene(int i) //i == scene number...(overflow...)
    {
        if (async != null)
            return;
        async = SceneManager.LoadSceneAsync(i); // load scene i
    }
    public void BtnLoadScene(string s) //s == name scene...(overflow...)
    {
        if (async != null)
            return;
        async = SceneManager.LoadSceneAsync(s); // load scene 's'...
    }
}
