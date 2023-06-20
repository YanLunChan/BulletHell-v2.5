using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SplineWalker),typeof(AudioSource))]
public class Enemy : LivingEntity
{
    public float debugoffset;
    //object pool variable
    public GameObject parentRef;
    public int health;
    [Header("Shooting Variables")]
    //scriptable object
    public BulletObject bObject;
    public GameObject targetRef;
    public float angleOffset;

    public int indexShot;
    private int currentShot;
    public float numShots;
    public float shootDelay;
    public bool shootStop;

    [Header("Movement")]
    public SplineWalker walker;

    //monobehaviour functions
    private void Awake()
    {
        walker = GetComponent<SplineWalker>();
        attackSound = GetComponent<AudioSource>();
    }
    protected override void Start()
    {
        base.Start();
        gameObject.layer = LayerMask.NameToLayer("Enemy Layer");
    }
    protected virtual void OnDisable()
    {
        attack = null;
        EnemyManager.GetInstance().ReQueue(parentRef, this);
    }

    //inherited functions
    protected override void OnDeath()
    {
        //ensure no coroutines are running before deactivation
        StopAllCoroutines();
        CancelInvoke();
        walker.Unlink();
        currentShot = 0;
        gameObject.SetActive(false);
        attackSound.Stop();
    }

    //custom functions
    public void SetPatterns(GameObject patternParentRef, BulletVariables[] value)
    {
        foreach (BulletVariables bv in value)
        {
            SetProperty(patternParentRef, bv);
        }
    }
    public void SetOwner(GameObject parent) => parentRef = parent;

    protected void SetProperty(GameObject patternParentRef, BulletVariables value)
    {
        float step = value.angleSmall / value.numAngle;

        for (int i = 0; i < value.iterationSmall; i++)
        {
            for (int j = 0; j < value.numAngle; j++)
            {
                //cache nececary variables
                BulletPattern cache = BulletManager.GetInstance().GetObject(patternParentRef);
                ParticleSystem system = cache.GetComponent<ParticleSystem>();
                //attach pattern to enemy
                cache.transform.position = transform.position;
                cache.transform.parent = transform;

                //cache angle to be used later...
                cache.angle = (step * j) + (value.angleLarge * i) + angleOffset;
                cache.spin_speed = value.spin_speed;

                //set added speed based on points and acceleration to it...
                float addedSpeed = value.speed + value.accelPoint * Mathf.Abs(Mathf.Sin(value.numPoints * cache.angle * Mathf.PI / value.angleSmall));
                cache.SetSpeedPoint(addedSpeed);
                //set particlesystem values
                value.bulletObject.SetCharacter(system);

                //set sound
                attackSound.clip = value.bulletObject.attackSound;
                ////set index
                cache.index = j;
                //set delegate dependant if character is attacking...
                if (value.targetPlayer)
                {
                    attack += () => cache.EmitTarget(targetRef, value.angleSmall * value.numAngle + (value.angleLarge * value.numAngle - 1));
                }
                else
                {
                    attack += cache.EmitNonTarget;
                }
            }
        }
    }
    private void AttackNTimes()
    {
        //take in increment and stuff...
        if (currentShot++ < numShots)
        {
            attack?.Invoke();
            //play sound
            GetComponent<AudioSource>().Play();
        }
        else
        {
            CancelInvoke();
            walker.go = true;
            currentShot = 0;
        }

    }
    //Coroutine
    public IEnumerator WaitFor(float ammount, float[] attackPos, float attackIncre)
    {
        float cache = 0f;
        while (cache < ammount)
        {
            cache += Time.deltaTime;
            yield return null;
        }
        walker.go = true;

        //run a second coroutine where it check if enemy is in the right spot to attack.
        StartCoroutine(AttackVolley(attackPos, attackIncre));
        yield return null;
    }
    private IEnumerator AttackVolley(float[] attackPos, float attackIncre)
    {
        for (int i = 0; i < attackPos.Length; i++)
        {
            while (walker.Progress < attackPos[i] + (attackIncre * indexShot))
            {
                yield return null;
            }
            //check if the enemy needs to stop to shoot?
            walker.go = !shootStop;
            //attack rewrite this portion so it uses invoke repeating...
            InvokeRepeating(nameof(AttackNTimes), 0f, shootDelay);

        }
        //disable gameobject when destination is reached.
        while(walker.Progress < 1) 
        {
            yield return null;
        }
        gameObject.SetActive(false);
        yield return null;
    }
    private void OnParticleCollision(GameObject other)
    {
        if (--health <= 0)
            OnDeath();
    }
}
