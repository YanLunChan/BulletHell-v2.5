using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct Phase 
{
    public AttackMoves[] attacks;
    public int MAX_HEALTH;

    //phase transition settings
    public float phaseLerpSpeed;
    public float delayStart;
    public Vector3 phaseStart;

    [Serializable]
    public struct AttackMoves 
    {
        public BulletVariables setting;
        public PatternComponent[] patterns;
    };
    
}
public class Boss : LivingEntity
{
    //Variables goes here
    [SerializeField] private Slider heathBar;
    [SerializeField] private Phase[] phases;

    private LoadScene nextLevel;
    private int currentHealth = 0;
    private int currentPhase = 0;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        nextLevel = GetComponent<LoadScene>();

        anim.SetInteger("Phase", currentPhase);
        ToggleAnimation();

        anim.applyRootMotion = true;

        //coroutine
        StartCoroutine(TransistionPhase());
    }

    public void StartNextPhase(int phase) 
    {
        anim.SetInteger("Phase", phase);
        currentHealth = phases[phase].MAX_HEALTH;
        SetPhaseUp();
        ToggleAnimation();


    }

    private void ToggleAnimation() 
    {
        if (anim.speed == 1)
            anim.speed = 0;
        else
            anim.speed = 1;
    }

    private void SetPhaseUp() 
    {
        foreach(Phase.AttackMoves am in phases[currentPhase].attacks) 
        {
            SetPattern(am);
        }
    }

    private void SetPattern(Phase.AttackMoves attackMove) 
    {
        int countTotal = 0;
        float step = attackMove.setting.angleSmall / (attackMove.setting.iterationSmall - 1f);
        for (int i = 0; i < attackMove.setting.numAngle; i++) 
        {
            for (int j = 0; j < attackMove.setting.iterationSmall; j++)
            {
                //put the settings from bullet variables into place.
                float angle = (step * j) + (attackMove.setting.angleLarge * i) + (attackMove.setting.angleSmall * i) + attackMove.setting.offsetAngle;
                attackMove.patterns[countTotal++].PatternConfig(attackMove.setting, angle);
            }
        }
    }
    //coroutine
    private IEnumerator TransistionPhase() 
    {
        float cache = 0f;
        Vector3 start = transform.position;
        //Lerp to Phase 1
        while (cache < phases[currentPhase].phaseLerpSpeed)
        {
            transform.position = Vector3.Lerp(start, phases[currentPhase].phaseStart, cache / phases[currentPhase].phaseLerpSpeed);
            cache += Time.fixedDeltaTime;
            yield return null;
        }
        //delay before phase start to fight
        cache = 0f;
        while (cache < phases[currentPhase].delayStart)
        {
            cache += Time.fixedDeltaTime;
            yield return null;
        }
        anim.applyRootMotion = false;
        //Start Phase
        StartNextPhase(currentPhase);
        yield return null;
    }
    private IEnumerator LerpSpinSpeed(int index)
    {
        float cacheTime = 0f;
        float[] startSpin = new float[phases[currentPhase].attacks[index].patterns.Length];
        //adding start spin elemetns to array
        for (int i = 0; i < startSpin.Length; i++)
            startSpin[i] = phases[currentPhase].attacks[index].patterns[i].SpinSpeed;
        //after changing all spin speed in 
        while (cacheTime < 1)
        {
            cacheTime += Time.fixedDeltaTime;
            print(cacheTime);
            for (int i = 0; i < startSpin.Length; i++) 
            {
                //Lerp spin speed
                phases[currentPhase].attacks[index].patterns[i].SpinSpeed = Mathf.Lerp(startSpin[i], -1 * startSpin[i], cacheTime);
            }
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
    private void OnParticleCollision(GameObject other)
    {
        //take damage and reduce health into check state.
        heathBar.value = (float) (--currentHealth) / phases[currentPhase].MAX_HEALTH;
        print(currentPhase);
        if(currentHealth < 0) 
        {
            OnDeath();
        }
    }

    protected override void OnDeath()
    {
        //Stop all variables from shooting
        foreach (Phase.AttackMoves am in phases[currentPhase].attacks)
            foreach (PatternComponent pc in am.patterns)
                pc.StopAllCoroutines();
        if (currentPhase < phases.Length - 1)
        {
            StartNextPhase(++currentPhase);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void AnimationEventAttack() 
    {
        print("invoking");
        attack?.Invoke();
    }
     
    public void AnimationEventChangeSpin(int index) 
    {

        StartCoroutine(LerpSpinSpeed(index));
    }
}
