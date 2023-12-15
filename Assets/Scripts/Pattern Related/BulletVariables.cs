using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this component will be set in the game manager and apply it to the particlesystem and gameobject transforms
[System.Serializable]
public class BulletVariables
{
    //Scriptable Object ("shared values" pattern base)
    [Header("Shared Values")]
    public BulletObject bulletObject;
    
    [Header("GameObject Values")]
    //values focus on inital set up for patterns
    public int numAngle;
    public bool targetPlayer;

    // iteration variables
    [Min(1)] public int iterationSmall;
    [Range(1, 360)] public float angleSmall;
    [Min(0)] public float angleLarge;
    //point variables
    public int numPoints;
    public float spin_speed;
    //number of times to shoot
    public int shootNum;
    //delay per shot
    public float shootDelay;

    [Header("Particle system Values")]
    //speed adjustment
    public float speed;
    public float accelPoint;
}
