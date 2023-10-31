using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Base enemy class. 
/// </summary>
public class Enemy_Mob : LivingEntity
{
    private SplineWalker walker;

    public SplineWalker Walker { get { return walker; } }

    private void Awake()
    {
        //initialize walker reference so it's not null
        walker = GetComponent<SplineWalker>();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    //inherited class
    protected override void OnDeath()
    {
        gameObject.SetActive(false);
        GetComponent<AudioSource>().Stop();
    }

    //reference of spline to walker
    public void SetSplinetoWalker(BezierSpline spline)
    {
        walker.Spline = spline;
    }
}
