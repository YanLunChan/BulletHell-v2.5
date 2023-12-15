using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : ObjectPool<Enemy>
{
    //set target to player since enemy will always be dependent to enemy manager
    public GameObject player;
    public Sprite sprite;

    protected override Enemy InitializeObjectPool(GameObject parent)
    {
        //create new gameobject
        GameObject cache = new GameObject();
        cache.name = "Enemy";
        cache.transform.parent = parent.transform;
        //game script's gameobject
        cache.SetActive(false);
        Enemy enemy = cache.AddComponent<Enemy>();
        enemy.SetOwner(this.gameObject);
        enemy.targetRef = player;
        SpriteRenderer spriteR = enemy.GetComponent<SpriteRenderer>();
        spriteR.sprite = this.sprite;
        spriteR.sortingOrder = 1;
        enemy.GetComponent<BoxCollider2D>().size = Vector2.one;

        //adding audio
        AudioSource audioSetup = enemy.AddComponent<AudioSource>();
        audioSetup.playOnAwake = false;
        return enemy;
    }
}
