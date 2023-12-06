using System;
using UnityEngine;
/// <summary>
/// Base enemy class. 
/// </summary>
public class Enemy_Mob : LivingEntity
{
    private int health;
    private SplineWalker walker;
    private Action deathReturn;
    public SplineWalker Walker { get { return walker; } }
    public Action DeathReturn { get { return deathReturn; } set { deathReturn = value; } }
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
        GetComponent<AudioSource>().Stop();
        //call both invoke for pattern and enemy mob (need to use for due to how foreach does not work in this situation)
        for(int i = transform.childCount - 1; i > -1; i--) 
        {
            PatternComponent pc = transform.GetChild(i).GetComponent<PatternComponent>();
            pc.StopAllCoroutines();
            pc.ReturnTo?.Invoke();
        }
        DeathReturn?.Invoke();
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    //reference of spline to walker
    public void OnSetup(BezierSpline spline, int health)
    {
        //set which spline/curve to go on
        walker.Spline = spline;

        //empty out action
        Attack = null;
        
        //set Health
        this.health = health;

    }

    private void OnParticleCollision(GameObject other)
    {
        //taking damage from character
        if(--health < 0) 
        {
            OnDeath();
        }
    }


}
