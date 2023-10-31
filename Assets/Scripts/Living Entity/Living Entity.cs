using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer),typeof(Rigidbody2D),typeof(BoxCollider2D))]
public abstract class LivingEntity : MonoBehaviour
{

    protected Rigidbody2D body;
    protected Animator anim;
    protected Action attack;
    protected virtual void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        body.gravityScale = 0f;
    }

    public Action Attack { get { return attack; } set { attack = value; } }

    //abstract functions
    protected abstract void OnDeath();
}
