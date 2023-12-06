using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WaveManagement 
{
    public float initialWait;
    public VolleySettings[] settings;
}
[Serializable]
public struct VolleySettings 
{
    public BezierSpline spline;
    public float delayTime;
}
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private WaveManagement waveManagement;

    [SerializeField] private GameObject BossHeathUI;
    [SerializeField] private GameObject Boss;
    //In charge of the start of level spawn.
    private IEnumerator Start()
    {
        float initialCache = 0f;
        while(initialCache < waveManagement.initialWait) 
        {
            initialCache += Time.deltaTime;
            yield return null;
        }
        //repeat cycle starts here
        foreach(VolleySettings vs in waveManagement.settings) 
        {
            float delayCache = 0f;
            while(delayCache < vs.delayTime) 
            {
                delayCache += Time.deltaTime;
                yield return null;
            }
            //call bezier function to spawn stuff
            vs.spline.StartBezierSpline();
        }
        //after 5 seconds spawn boss
        initialCache = 0f;
        while(initialCache < 5f) 
        {
            initialCache += Time.deltaTime;
            yield return null;
        }
        BossHeathUI.SetActive(true);
        Boss.SetActive(true);
        yield return null;
    }
}
