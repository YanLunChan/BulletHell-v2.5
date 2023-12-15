using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BulletPattern : MonoBehaviour
{
    public GameObject parentRef;
    [SerializeField] private ParticleSystem system;

    //general variables 
    //[HideInInspector] public int index;
    public float angle;
    public float spin_speed;
    public float delay;

    public bool patternEnd;
    private void Awake()
    {
        system = GetComponent<ParticleSystem>();
    }
    private void Start()
    {
        //set up initial values for particle system to work in 2d enviroment
        InitialSetUp();
    }
    //default set that particlesystem need to function in 2d world
    private void InitialSetUp()
    {
        //set up object's rotation so sprite is shown
        transform.eulerAngles = new Vector3(0f, 90, 0f);

        //cache the main
        var main = system.main;

        //turn loop off
        main.loop = false;

        //set rotaion
        main.startRotation3D = true;
        main.startRotationX = 90f * Mathf.Deg2Rad;
        main.startRotationY = 180f * Mathf.Deg2Rad;

        //set simulated space to world
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        //make sure it doesn't play on awake
        main.playOnAwake = false;

        //turn off emission
        var emission = system.emission;
        emission.enabled = false;

        //change shape to sprite
        var shape = system.shape;
        shape.shapeType = ParticleSystemShapeType.Sprite;

        //turn on texture and change to sprite
        var sprite = system.textureSheetAnimation;
        sprite.enabled = true;
        sprite.mode = ParticleSystemAnimationMode.Sprites;

        //add collision
        var col = system.collision;
        col.enabled = true;
        col.type = ParticleSystemCollisionType.World;
        col.mode = ParticleSystemCollisionMode.Collision2D;
        col.maxKillSpeed = 10000;
        col.sendCollisionMessages = true;
        col.bounce = 0;
        col.lifetimeLoss = 1;
        //optional things to unlock so later on it can be used
        var velocityLife = system.velocityOverLifetime;
        velocityLife.enabled = true;

        //set render alignment to velocity so bullet will rotate base on where it travels.
        ParticleSystemRenderer render = GetComponent<ParticleSystemRenderer>();
        render.alignment = ParticleSystemRenderSpace.Velocity;
    }

    public void EmitNonTarget(int numShots) 
    {
        for (int i = 0; i < numShots; i++)
        {
            //set angle
            transform.eulerAngles = new Vector3(angle, 90f, 0);
            angle += spin_speed;
            angle %= 360f;
            //emit target
            system.Emit(1);
            StartCoroutine(AttackDelay());
        }

    }
    public void EmitTarget(GameObject target, float midPoint, int numShots) 
    {
        for (int i = 0; i < numShots; i++)
        {
            //get vector between owner and target
            Vector3 vector2player = target.transform.position - transform.parent.position;
            //get new angle location
            float offset = Mathf.Atan2(vector2player.y, vector2player.x) * 180f / Mathf.PI;

            transform.eulerAngles = new Vector3(angle - offset + midPoint, 90f, 0);

            system.Emit(1);
            StartCoroutine(AttackDelay());
        }
    }
    public void SetSpeedPoint(float addedSpeed) 
    {
        var main = system.main;
        main.startSpeed = addedSpeed;
    }

    public void UnLink() 
    {
        StopAllCoroutines();
        transform.parent = parentRef.transform;
        transform.localScale = Vector3.one;
        BulletManager.GetInstance().ReQueue(parentRef, this);
    }
    //coroutine for indenpendent delay
    private IEnumerator AttackDelay() 
    {
        float cache = 0;
        while(cache < delay) 
        {
            cache += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
}


////
//Vector3 vector2player = target.transform.position - transform.parent.position;
////check if num is odd or even and if even do v and odd do line
//bool even = numAngle % 2 == 0;
//float newAngle = 0f;
//////need -1 due to rotationof y being 90
//if (even)
//{
//    newAngle = (-1 * Mathf.Atan2(vector2player.y, vector2player.x) * 180f / Mathf.PI) + (Mathf.Pow(-1, index) * (((index / 2) + 1) * angleSmall));
//}
//else if (!even && index > 0)
//{
//    newAngle = (-1 * Mathf.Atan2(vector2player.y, vector2player.x) * 180f / Mathf.PI) + (Mathf.Pow(-1, index) * ((((index - 1) / 2) + 1) * angleSmall));
//}
//else
//{
//    newAngle = (-1 * Mathf.Atan2(vector2player.y, vector2player.x) * 180f / Mathf.PI);
//}
//transform.eulerAngles = new Vector3(newAngle, 90f, 0);
////