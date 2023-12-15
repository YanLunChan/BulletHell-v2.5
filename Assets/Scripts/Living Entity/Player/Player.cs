using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : LivingEntity
{
    [SerializeField] protected HealthManager hManager;
    [SerializeField] private float speed;
    [SerializeField] private ParticleSystem[] muzzle;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private float attackCool;
    //attack variable
    private bool attackReady = true;
    private bool attackPressed = false;
    private float attackCache = 0f;

    private const float MAX_SPEED = 10f;
    private const float INVINCIBILITY_COOL = 5;
    private bool invincible = false;
    private Vector2 axis;

    //Monobehavour classes
    protected override void Start()
    {
        base.Start();
        attack = () => { foreach (ParticleSystem ps in muzzle) ps.Emit(1); };
        attackSound = GetComponent<AudioSource>();
        pauseMenu?.SetActive(false);

    }
    private void Update()
    {
        CallAttack();
    }
    private void FixedUpdate()
    {
        body.velocity = axis * speed;
    }
    private void OnEnable()
    {
        hManager.loseLife += () => StartCoroutine(Invincibility());
        hManager.Death += OnDeath;
    }
    private void OnDisable()
    {
        hManager.Death -= OnDeath;
    }

    //Controls
    public void OnMove(InputAction.CallbackContext context) => axis = context.ReadValue<Vector2>();
    public void OnFocus(InputAction.CallbackContext context) => speed = context.performed ? MAX_SPEED / 2 : MAX_SPEED;
    public void OnShoot(InputAction.CallbackContext context) =>  attackPressed = context.performed;
    //----------DEBUG----------
    public void OnPause(InputAction.CallbackContext context) 
    {
        if (Time.timeScale != 0)
        {
            Time.timeScale = 0;
            pauseMenu?.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pauseMenu?.SetActive(false);
        }
    }
    
    //Collision
    protected virtual void OnParticleCollision(GameObject other)
    {
        if(!invincible)
            hManager.TakeDamage();
    }

    //Abstract Functions
    protected override void OnDeath()
    {
        //disable gameobject
        gameObject.SetActive(false);
    }

    //Custom Functions
    public void CallAttack() 
    {
        if (attackReady && attackPressed)
        {
            attack?.Invoke();
            attackSound.Play();
            attackReady = false;
        }
        else
        {
            if (attackCache <= attackCool)
            {
                attackCache += Time.deltaTime;
            }
            else
            {
                attackReady = true;
                attackCache = 0;
            }
        }
    }
    public IEnumerator Invincibility() 
    {
        invincible = true;
        int layerIndex = anim.GetLayerIndex("Hurt");
        //return to initial location (0,-10,0)
        transform.position = new Vector3(0, -8);
        //play animation
        anim.SetLayerWeight(layerIndex, 1);
        float cache = 0f;
        while(cache < INVINCIBILITY_COOL) 
        {
            cache += Time.deltaTime;
            yield return null;
        }
        //stop animation
        anim.SetLayerWeight(layerIndex, 0);
        invincible = false;
    }
}
