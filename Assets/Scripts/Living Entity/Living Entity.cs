using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer),typeof(Rigidbody2D),typeof(BoxCollider2D))]
public abstract class LivingEntity : MonoBehaviour
{

    protected Rigidbody2D body;
    protected Action attack;
    protected Animator anim;
    [SerializeField] protected AudioSource attackSound;
    protected virtual void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        body.gravityScale = 0f;
    }

    //abstract functions
    protected abstract void OnDeath();
}
