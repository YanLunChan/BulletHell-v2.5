using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

[CreateAssetMenu(fileName = "Bullet", menuName = "Variables")]
public class BulletObject : ScriptableObject
{
    //variables to set that I know won't change
    public float size;
    public float lifetime;
    public Color color;
    
    public LayerMask layerMask;

    public Sprite sprite;

    public Material mat;

    public AudioClip attackSound;
    public void SetCharacter(ParticleSystem system) 
    {
        //set size and lifetime
        var main = system.main;
        main.startSize = size;
        main.startLifetime = lifetime;
        main.startColor = color;

        //set layermask
        var collision = system.collision;
        collision.enabled = true;
        collision.collidesWith = layerMask;
        collision.type = ParticleSystemCollisionType.World;
        collision.mode = ParticleSystemCollisionMode.Collision2D;
        collision.bounce = 0f;
        collision.lifetimeLoss = 1f;
        collision.sendCollisionMessages = true;

        //set sprite
        var tex = system.textureSheetAnimation;
        tex.mode = ParticleSystemAnimationMode.Sprites;
        tex.AddSprite(sprite);

        //enable velocity over time
        var velocity = system.velocityOverLifetime;
        velocity.enabled = true;

        var render = system.gameObject.GetComponent<ParticleSystemRenderer>();
        render.material = mat;
    }
}
