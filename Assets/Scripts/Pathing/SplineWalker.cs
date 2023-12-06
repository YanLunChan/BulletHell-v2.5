using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public enum SplineWalkerMode
{
    Once,
    Loop,
    PingPong
}
/// <summary>
/// This script is used purely for walking on the spline. Currently it is uses for enemy mob to move along the spline curve.
/// </summary>
public class SplineWalker : MonoBehaviour
{
    private BezierSpline spline;

    //go should be off until spline has a reference or else null exception error.
    [SerializeField] private bool go = false;
    [SerializeField] private bool lookForward;
    [SerializeField] private float offsetAngle;
    [SerializeField] private float speed;

    private float progress = 0;
    private bool goingForward = true;

    private Action destinationAction;

    public BezierSpline Spline{ get { return spline; }  set { spline = value; } }
    private void Update()
    {
        if (!go)
            return;
        //movement
        if (goingForward)
        {
            //progress += Time.deltaTime / duration;
            progress += speed * Time.deltaTime;
            if (progress > 1f)
            {
                destinationAction?.Invoke();
            }
        }
        else
        {
            //going backwards
            progress -= speed * Time.deltaTime;
            if (progress < 0f)
            {
                progress = -progress;
                goingForward = true;
            }
        }
        //setting possition
        Vector3 position = spline.GetPoint(progress);
        transform.position = position;

        //setting rotation face
        if (lookForward)
        {
            float angle = goingForward ? Mathf.Atan2(spline.GetDirection(progress).y, spline.GetDirection(progress).x) * Mathf.Rad2Deg :
                                        Mathf.Atan2(spline.GetDirection(progress).y, spline.GetDirection(progress).x) * Mathf.Rad2Deg - 180f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - offsetAngle);
        }
    }
    public float Progress
    {
        get => progress;
        set => progress = value;
    }

    public void SetMode(SplineWalkerMode mode) 
    {
        //depending on the mode, select the delegate as to what happens when duration hits 1f
        if (mode == SplineWalkerMode.Once)
        {
            destinationAction = () => progress = 1;
        }
        else if (mode == SplineWalkerMode.Loop)
            destinationAction = () => progress -= 1;
        else
            destinationAction = () =>
            {
                progress = 2f - progress;
                goingForward = false;
            };
    }


    //Coroutine for pausing movement

    public IEnumerator Cycle(Enemy_Settings settings, int index, TupleInspector<float>[] duration) 
    {
        //Setting the Cycle
        speed = settings.speed;
        //Initial wait time.
        float cache = 0f;
        while (cache < (settings.increWait * index))
        {
            cache += Time.deltaTime;
            yield return null;
        }
        go = true;
        
        foreach (TupleInspector<float> pause in duration)
        {

            if (goingForward)
            {
                while (progress < pause.destination + (pause.incrDes * index))
                {
                    yield return null;
                }
                go = !go;
                //enemy attack goes here.
                gameObject.GetComponent<LivingEntity>().Attack?.Invoke();
                //wait for a while
                yield return new WaitForSeconds(pause.time + pause.incrTime * index);
                go = true;
            }
            else
            {
                while (progress > pause.destination + (pause.incrDes * index))
                {
                    yield return null;
                }
                //check if the enemy needs to stop to shoot?
                go = !go;
                //enemy attack goes here.
                gameObject.GetComponent<LivingEntity>().Attack?.Invoke();
                //wait for a while
                yield return new WaitForSeconds(pause.time + pause.incrTime * index);
                go = true;

            }
        }

    }
}
