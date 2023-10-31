using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
/// <summary>
/// Bullet Compont will be responsable for spawning bullets for enemy mobs.
/// It will be used to cache angels and emit 1 bullet every x second for certain amount of time.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class PatternComponent : MonoBehaviour
{
    [SerializeField] private GameObject target;
    //cache aangle
    [SerializeField] private float cacheAngle;
    //Reference to where to go after death (manager or boss)
    [SerializeField] private GameObject returnTo;

    private ParticleSystem particleRef;

    private void Awake()
    {
        particleRef = GetComponent<ParticleSystem>();
        
        InitialSetUp();
    }

    public void InitialSetUp() 
    {
        //set up object's rotation so sprite is shown
        transform.eulerAngles = new Vector3(0f, 90, 0f);

        //cache the main
        var main = particleRef.main;

        //turn loop off
        main.loop = false;

        //set rotaion
        main.startRotation3D = true;
        main.startRotationX = 90f * Mathf.Deg2Rad;
        main.startRotationY = 180f * Mathf.Deg2Rad;

        //set life time
        main.startLifetime = 100f;

        //set simulated space to world
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        //make sure it doesn't play on awake
        main.playOnAwake = false;

        //turn off emission
        var emission = particleRef.emission;
        emission.enabled = false;

        //change shape to sprite
        var shape = particleRef.shape;
        shape.shapeType = ParticleSystemShapeType.Sprite;

        //turn on texture and change to sprite
        var sprite = particleRef.textureSheetAnimation;
        sprite.enabled = true;
        sprite.mode = ParticleSystemAnimationMode.Sprites;

        //add collision
        var col = particleRef.collision;
        col.enabled = true;
        col.type = ParticleSystemCollisionType.World;
        col.mode = ParticleSystemCollisionMode.Collision2D;
        col.maxKillSpeed = 10000;
        col.sendCollisionMessages = true;
        col.bounce = 0;
        col.lifetimeLoss = 1;
        //optional things to unlock so later on it can be used
        var velocityLife = particleRef.velocityOverLifetime;
        velocityLife.enabled = true;

        //set render alignment to velocity so bullet will rotate base on where it travels.
        ParticleSystemRenderer render = GetComponent<ParticleSystemRenderer>();
        render.alignment = ParticleSystemRenderSpace.Velocity;
    }

    public void PatternConfig(BulletVariables newSetting, float angle) 
    {
        //This function is mostly used to configur the Particle System.
        //cache angle only set when ready to shoot.

        this.cacheAngle = angle;
        
        transform.eulerAngles = new Vector3(angle, 90f, 0f);

        //Cache particle system var here
        var main = particleRef.main;
        var collision = particleRef.collision;
        var texture = particleRef.textureSheetAnimation;
        var velocity = particleRef.velocityOverLifetime;
        var render = particleRef.gameObject.GetComponent<ParticleSystemRenderer>();

        //Set acceleration points to create points 
        float addedSpeed = newSetting.projSpeed + newSetting.accelSpeed * Mathf.Abs(Mathf.Sin(newSetting.pointNum * angle * Mathf.PI/newSetting.angleSmall));
        main.startSpeed = addedSpeed;
        main.startSize = newSetting.projSize;
        //Set Collision
        collision.enabled = true;
        collision.collidesWith = newSetting.layerMask;
        collision.type = ParticleSystemCollisionType.World;
        collision.mode = ParticleSystemCollisionMode.Collision2D;
        collision.bounce = 0f;
        collision.lifetimeLoss = 1f;
        collision.sendCollisionMessages = true;

        texture.mode = ParticleSystemAnimationMode.Sprites;
        texture.AddSprite(newSetting.sprite);

        render.material = newSetting.projMat;

        LivingEntity pattern = GetComponentInParent<LivingEntity>();
        //Add function for target or non target
        if (!newSetting.target)
        {
            pattern.Attack += () => StartCoroutine(NonTargetAttackTimes(newSetting));
        }
        else
        {
            pattern.Attack += () => StartCoroutine(TargetAttackTimes(newSetting));
        }
        //add functions to delegate in enemyMob to trigger attacks

        //Set delegate depending if there is aiming or not. (Call function that starts coroutine)

        //        //cache angle to be used later...
        //        cache.angle = (step * j) + (value.angleLarge * i) + angleOffset;
        //        cache.spin_speed = value.spin_speed;

        //        //set added speed based on points and acceleration to it...
        //        float addedSpeed = value.speed + value.accelPoint * Mathf.Abs(Mathf.Sin(value.numPoints * cache.angle * Mathf.PI / value.angleSmall));
        //        cache.SetSpeedPoint(addedSpeed);
        //        //set particlesystem values
        //        value.bulletObject.SetCharacter(system);
        //        //set delay
        //        cache.delay = value.shootDelay;

        //        //set sound
        //        attackSound.clip = value.bulletObject.attackSound;
        //        ////set index
        //        //cache.index = j;

        //        //set delegate dependant if character is attacking...
        //        if (value.targetPlayer)
        //        {
        //            attack += () => cache.EmitTarget(targetRef, value.angleSmall * value.numAngle + (value.angleLarge * value.numAngle - 1), value.shootNum);
        //        }
        //        else
        //        {
        //            attack += () => cache.EmitNonTarget(value.shootNum);
        //        }
    }
    // set the variables that enemy needs
    public void SetVariables(GameObject returnTo, GameObject target)
    {
        //Since the Pattern Component will not disable when entity dies it needs to return to pool but not UnEnable itself.
        this.returnTo = returnTo;
        //Set target when theere's is a target on screen
        this.target = target;
    }
    //Invoke x times non target
    public IEnumerator NonTargetAttackTimes(BulletVariables value) 
    {
        for(int i = 0; i < value.shootNum; i++) 
        {
            float cache = 0;
            //set the angle and shoot
            transform.eulerAngles = new Vector3(this.cacheAngle, 90f, 0f);
            particleRef.Emit(1);
            cacheAngle += value.spinSpeed;

            print($"angle: {this.cacheAngle}, midpoint of the angle: {value.angleLarge * (value.numAngle - 1) * 0.5f} angle range {value.angleLarge * (value.numAngle - 1)}");
            while(cache < value.delayPerShot) 
            {
                cache += Time.deltaTime;
                yield return null;
            }

        }
        yield return null;
    }
    public IEnumerator TargetAttackTimes(BulletVariables value)
    {

        for (int i = 0; i < value.shootNum; i++) 
        {
            float cache = 0;
            //get vector between target and user and convert it to an angle
            Vector3 vector2player = this.target.transform.position - transform.parent.position;
            float offset = Mathf.Atan2(vector2player.y, vector2player.x) * 180f / Mathf.PI;

            //midpoint (find max angle and divide it by 2)
            float midPoint = (value.angleLarge * (value.numAngle - 1) + value.angleSmall) * 0.5f;
            //set the angle and shoot
            transform.eulerAngles = new Vector3(this.cacheAngle - offset - midPoint, 90f, 0f);
            cacheAngle += value.spinSpeed;
            particleRef.Emit(1);
            while (cache < value.delayPerShot)
            {
                cache += Time.deltaTime;
                yield return null;
            }
        }
        yield return null;
    }
}
