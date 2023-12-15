using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Boss : LivingEntity
{
    [System.Serializable]
    internal struct Phase
    {
        public int max_health;
        public GameObject parentBulletRef;
        public BulletVariables[] settings;
        public BezierSpline spline;
        public int numShots;
        public SplineWalkerMode mode;
        public float speed;
        public float shootDelay;

        //duration and location to next phase
        public Vector3 startLocal;
        public float duration;
    }
    [SerializeField] private Phase[] phases;
    [SerializeField] private GameObject targetRef;
    [SerializeField] private Image healthBar;
    private int currentHealth = 0;
    private int currentPhase = 0;
    private bool takeDamage = false;
    private SplineWalker walker;
    // Monobehaviour classes
    protected override void Start()
    {
        base.Start();
        walker = GetComponent<SplineWalker>();
        attackSound = GetComponent<AudioSource>();
        //at spawn move to location before setting the phase.
        StartCoroutine(GoTo(new Vector3(0f, 6f, 0f), 0.5f));
    }
    protected void OnDisable()
    {
        attack = null;
    }
    
    //inherited classes
    protected override void OnDeath()
    {
        if (takeDamage)
        {
            walker.Unlink();
            if (++currentPhase < phases.Length)
                StartCoroutine(GoTo(phases[currentPhase].startLocal, phases[currentPhase].duration));
            else
            {
                CancelInvoke();
                gameObject.SetActive(false);
                GetComponent<LoadScene>().BtnLoadScene();
            }
        }
    }

    //custom function
    private void SetPhase(int index) 
    {
        CancelInvoke();
        attack = null;
        //set pattern
        SetPatterns(phases[index].parentBulletRef, phases[index].settings);
        //set spline if there is any
        if (phases[index].spline) 
        {
            walker.SetMode(phases[index].mode);
            walker.spline = phases[index].spline;
            walker.go = true;
            walker.speed = phases[index].speed;
        }
        currentHealth = phases[index].max_health;
        InvokeRepeating("Attack", 0.1f, phases[index].shootDelay);
        takeDamage = true;
        healthBar.fillAmount = 1;
    }
    public void Attack() 
    {
        attack?.Invoke();
        attackSound.Play();
    }
    public void SetPatterns(GameObject patternParentRef, BulletVariables[] value)
    {
        foreach (BulletVariables bv in value)
        {
            SetProperty(patternParentRef, bv);
        }
    }
    private void SetProperty(GameObject patternParentRef, BulletVariables value)
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

                //make sure patterns gameobject scale is (1,1,1)
                cache.transform.localScale = Vector3.one;
                //cache angle to be used later...
                cache.angle = (step * j) + (value.angleLarge * i);
                cache.spin_speed = value.spin_speed;

                //set added speed based on points and acceleration to it...
                float addedSpeed = value.speed + value.accelPoint * Mathf.Abs(Mathf.Sin(value.numPoints * cache.angle * Mathf.PI / value.angleSmall));
                cache.SetSpeedPoint(addedSpeed);
                //set particlesystem values
                value.bulletObject.SetCharacter(system);

                ////set index
                //cache.index = j;
                //set delegate dependant if character is attacking...
                if (value.targetPlayer)
                {
                    attack += () => cache.EmitTarget(targetRef, value.angleSmall * value.numAngle + (value.angleLarge * value.numAngle - 1), phases[currentPhase].numShots);
                }
                else
                {
                    attack += () =>cache.EmitNonTarget(phases[currentPhase].numShots);
                }
            }
        }
    }
    private void OnParticleCollision(GameObject other)
    {

        if (currentHealth <= 0)
        {
            OnDeath();
        }
        else
        {
            healthBar.fillAmount = (float) (--currentHealth) / phases[currentPhase].max_health;
        }
    }

    public IEnumerator GoTo(Vector3 distination, float duration) 
    {
        Vector3 startPos = transform.position;
        float cache = 0f;
        takeDamage = false;
        while(cache <= 1f) 
        {
            yield return null;
            transform.position = Vector3.Lerp(startPos, distination, cache / duration);
            cache += Time.deltaTime;
        }
        SetPhase(currentPhase);
        yield return null;
    }
}
