using System;
using UnityEngine;
public enum SplineWalkerMode
{
    Once,
    Loop,
    PingPong
}
/// <summary>
/// This script is used purely for walking on the spline. 
/// </summary>
public class SplineWalker : MonoBehaviour
{
    public BezierSpline spline;

    //go should be off until spline has a reference or else null exception error.
    public bool go = false;
    public bool lookForward;
    public float offset;
    public float speed;

    private float progress;
    private bool goingForward;

    private Action destinationAction;
    private void Update()
    {
        if (!go)
            return;
        //movement
        if (goingForward)
        {
            //progress += Time.deltaTime / duration;
            //might change to progress += duration, because it would make the speed more constant when going on curves ()
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
            transform.rotation = Quaternion.Euler(0f, 0f, angle - offset);
        }
    }
    public void SetObject()
    {
        transform.position = spline.GetPoint(progress);
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
            destinationAction += Unlink;
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
    public void Unlink() 
    {
        //reset destination action so when enemy despawn / dies, set it to null, needs to be set everytime.

        for(int i = transform.childCount - 1; i >= 0; i--) 
        {
            if (transform.GetChild(i).GetComponent<BulletPattern>())
                transform.GetChild(i).GetComponent<BulletPattern>().UnLink();
        }
        destinationAction = null;
    }
}
