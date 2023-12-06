using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This Scriptable Object is used to store what the following:
/// -what the enemy looks like
/// -what sound does it make
/// -a function where all these variables are givin to the enemy mob.
/// </summary>
/// 

[CreateAssetMenu(fileName = "Enemy", menuName = "Create Enemy/Enemy Mob", order = 1)]
public class EnemySO : ScriptableObject
{
    [SerializeField] private Sprite sprite;
    //bullet patter goes here
    [SerializeField] private AudioClip shootSound;

    [SerializeField] public List<GameObject> spawnedObjects = new List<GameObject>();

    //any function used to initialize to create copy.
    public void Initialization(GameObject reference) 
    {
        reference.GetComponent<SpriteRenderer>().sprite = sprite;
        reference.GetComponent<AudioSource>().clip = shootSound;
    }
}
