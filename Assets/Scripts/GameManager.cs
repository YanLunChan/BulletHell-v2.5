using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [System.Serializable]
    internal struct EnemyVolley 
    {
        [Header("Delays")]
        //should be called in game manager
        public float startDelay;
        //should be called in enemy class
        public float baseWaitDelay;
        public float incremWaitDelay;

        [Header("Number of enemy per wave")]
        public int volleyNum;

        [Header("Enemy shooting settings")]
        public EnemySetting settings;

        [Header("Percentage on spline where enemy call attack")]
        [Range(0, 1)] public float[] baseActPer;
        [Range(-1, 1)] public float incremActPer;

        [Header("Spline Variable")]
        public float speed;
        [Header("Health Ammount")]
        public int health;
        [System.Serializable]
        public struct EnemySetting
        {
            [Header("Parent so you can get enemy from pool")]
            public GameObject enemyParentRef;
            public GameObject bulletParentRef;
            [Header("How do the enemy shoot?")]
            public BulletVariables[] enemySetting;
            public float shootNum;
            public float shootDelay;
            //public float baseWaitDelay;
            public bool shootStop;
            public float angleOffset;

            [Header("Where to spawn")]
            public BezierSpline spline;
            public SplineWalkerMode mode;
        }
    }
    [SerializeField] private EnemyVolley[] volley;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject bossHP;
    private IEnumerator Start()
    {
        float bossTimerCache = 0f; 
        //set initial delay
        for (int i = 0; i < volley.Length; i++) 
        {
            //inital delay per wave (if zero then )
            float cache = 0f;
            while (cache < volley[i].startDelay)
            {
                cache += Time.deltaTime;
                yield return null;
            }
            //---DEBUG---
            Debug.Log("Initial delay is done staring spawn");
            //---DEBUG---
            for (int j = 0; j < volley[i].volleyNum; j++)
            {
                //take bullet variable and get patterns from manager and attach to enemy reference...
                //Cache Enemy
                Enemy enemyRef = EnemyManager.GetInstance().GetObject(volley[i].settings.enemyParentRef);
                enemyRef.transform.position = volley[i].settings.spline.GetPoint(0);
                enemyRef.gameObject.SetActive(true);
                //Set Pattern
                enemyRef.SetPatterns(volley[i].settings.bulletParentRef, volley[i].settings.enemySetting);
                //Set Spline
                enemyRef.walker.Progress = 0;
                enemyRef.walker.spline = volley[i].settings.spline;
                enemyRef.walker.speed = volley[i].speed;
                enemyRef.walker.SetMode(volley[i].settings.mode);
                //set shooting settings
                enemyRef.numShots = volley[i].settings.shootNum;
                enemyRef.shootStop = volley[i].settings.shootStop;
                enemyRef.shootDelay = volley[i].settings.shootDelay;
                enemyRef.indexShot = j;

                enemyRef.health = volley[i].health;
                //start wait-time coroutine
                enemyRef.StartCoroutine(enemyRef.WaitFor(volley[i].baseWaitDelay + (volley[i].incremWaitDelay * j), volley[i].baseActPer, volley[i].incremActPer));
            }
        }
        while(bossTimerCache < 10f) 
        {
            bossTimerCache += Time.deltaTime;
            yield return null;
        }
        boss.SetActive(true);
        bossHP.SetActive(true);
        yield return null;
    }
}
